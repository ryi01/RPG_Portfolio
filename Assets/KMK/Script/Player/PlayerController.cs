using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

// 강제적으로 컴포넌트와 컨트롤러를 세트로 만들어줌
[RequireComponent(typeof(PlayerStatComponent))]
[RequireComponent(typeof(CombatFeedback))]
// 추가로 할일
// 조준 보정 => 마우스포인터와 가장 가까이 있는 Enemy로 회전
public class PlayerController : BaseController<PlayerStatComponent>
{
    #region 변수 모음
    [SerializeField] private AudioClip footStepClip;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Vector2 interval = new Vector2(0.34f, 0.42f);

    // 입력 및 행동 관련 하위 컴포넌트
    public InputMovement MovementComp { get; private set; }
    public InputAttack AttackComp { get; private set; }
    public InputSkill SkillComp { get; private set; }
    public InputPickUp PickUpComp { get; private set; }
    // 피격 연출 카메라 연출
    public CombatFeedback CombatFeedback { get; private set; }
    public CameraShakeController CameraShakeController { get; private set; }
    // 외부에서 제어하는 상태값
    public bool IsDamage { get; set; }
    public bool IsBlink { get; set; }
    // 마우스 월드 위치
    public Vector3 AimDir => aimDir;
    // 실시간 마우스 방향
    public Vector3 AimPoint => aimPoint;
    // 입력이 들어온 순간 마우스 방향
    public Vector3 LockedAimDir => lockedAimDir;

    // 마우스 조준 방향
    private Vector3 aimDir;
    // 마우스 월드 좌표
    private Vector3 aimPoint;
    // 입력 순간 고정방향
    private Vector3 lockedAimDir;
    // 공격 도중 설정된 방향
    private Vector3 targetLookDir;

    private bool isMove = false;
    // 발소리 타이머
    private float stepTimer = 0;
    private float currentStepInterval = 0.4f;
    // 현재 사용중인 스킬
    private InputSkill.SKILLS currentSkill;
    private KeyCode[] skillKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.D, KeyCode.F };

    // 아이템 관련 변수들
    private ItemBox openBox;
    private InteractionObject targetInteractable;
    private bool isMoveToInteraction = false;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        
        MovementComp = GetComponent<InputMovement>();
        AttackComp = GetComponent<InputAttack>();
        SkillComp = GetComponent<InputSkill>();
        PickUpComp = GetComponent<InputPickUp>();
        CameraShakeController = GetComponentInChildren<CameraShakeController>();
        StatComp.OncChangeLevel += SkillComp.OnLockSkill;
        CombatFeedback = GetComponent<CombatFeedback>();        
        if(trail != null) trail.emitting = false;
        currentStepInterval = UnityEngine.Random.Range(interval.x, interval.y);
    }
    // Update is called once per frame
    void Update()
    {
        if (IsDamage || GameManager.Instance.CurrentState == GameState.Pause || GameManager.Instance.CurrentState == GameState.Dialogue)
        {
            MovementComp.StopMove();
            return;
        }
        // 마우스 위치와 방향 갱신
        UpdateAimData();
        // 공격 중 버퍼 체크
        UpdateAttackLogic();
        // 일반 입력
        HandleInput();

        if (GameManager.Instance.CurrentState != GameState.Town) HandleSkill();
        HandleMovement();   
        HandleRotation();
        HandleUseItem();
        CheckInteractionDistance();
        CheckBox();
    }
    /// <summary>
    /// 현재 마우스 월드 위치와 실시간 방향 갱신
    /// </summary>
    private void UpdateAimData()
    {
        // 마우스 위치
        aimPoint = MovementComp.GetMouseWorldPos();
        // 플레이어 -> 마우스 방향
        aimDir = aimPoint - transform.position;
        aimDir.y = 0;
        if (aimDir.sqrMagnitude > 0.001f) aimDir.Normalize();
        else aimDir = Vector3.zero;
    }
    /// <summary>
    /// 공격 / 스킬 입력시 고정된 방향
    /// </summary>
    private bool LockAimDir()
    {
        UpdateAimData();
        if (aimDir == Vector3.zero) return false;
        lockedAimDir = aimDir;
        return true;
    }
    /// <summary>
    /// 공격 중이고 버퍼가 있다면 로직 갱신
    /// </summary>
    private void UpdateAttackLogic()
    {
        if (AttackComp.IsAttackAnimation() || AttackComp.IsBufferActive()) AttackComp.UpdateAttackProgress();
    }
    /// <summary>
    /// 마우스 입력 처리, UI 위라면 입력거부
    /// </summary>
    private void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        HandleMoveAndInteractInput();
        HandleAttackInput();
    }
    #region 아이템 관련
    private void HandleUseItem()
    {
        for(int i = 0; i < 9; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                GameManager.Instance.InventroySystem.UseItem(i);
            }
        }
    }

    private void HandleMoveAndInteractInput()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        int layerMask = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Obstacle"));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100, layerMask).OrderBy(h => h.distance).ToArray();

        InteractionObject foundInteraction = null;
        RaycastHit groundHit = default;
        bool isGround = false;
        foreach (var hit in hits)
        {
            var interactable = hit.collider.GetComponentInParent<InteractionObject>();
            if (interactable != null)
            {
                foundInteraction = interactable;
                break;
            }
            groundHit = hit;
            isGround = true;
        }

        if (foundInteraction != null)
        {
            targetInteractable = foundInteraction;
            isMoveToInteraction = true;
            MovementComp.FindPath(foundInteraction.GetTransform().position);
        }
        else if (isGround)
        {
            targetInteractable = null;
            isMoveToInteraction = false;
            if (GameManager.Instance.CurrentState == GameState.Town) MovementComp.FindPath(groundHit.point, false);
            else MovementComp.FindPath(groundHit.point);
        }

    }
    private void CheckInteractionDistance()
    {
        if (targetInteractable == null || !isMoveToInteraction) return;
        if (CheckAttackSkillDamage()) return;

        float dist = Vector3.Distance(transform.position, targetInteractable.transform.position);
        if (dist <= interactionDistance)
        {
            MovementComp.StopMove();
            targetInteractable.Interact(this);
            targetInteractable = null;
            isMoveToInteraction = false;
        }
    }
    public void OpenBox(ItemBox box)
    {
        if (openBox != null) PickUpComp.CloseUI();
        openBox = box;
        PickUpComp.OpenItemBox(box);
    }
    private void CheckBox()
    {
        if (openBox == null) return;
        float dist = Vector3.Distance(transform.position, openBox.transform.position);
        if(dist > interactionDistance)
        {
            PickUpComp.CloseUI();
            openBox = null;
        }
    }
    public void TryInteract(NPCInteraction npc)
    {
        if (npc == null) return;
        GameManager.Instance.ChangeState(GameState.Dialogue);
        npc.Interact();
    }
    #endregion
    #region 이동관련
    private void HandleMovement()
    {
        if (SkillComp.IsSkillAnimation(currentSkill))
        {
            if(currentSkill == InputSkill.SKILLS.SKILL2)
            {
                if(isMove)
                {
                    Vector3 moveAimDir = GetMoveAimDir();
                    if (moveAimDir != Vector3.zero) MovementComp.Move(moveAimDir);
                }
                Animator.SetFloat("Move", 0);
            }
            return;
        }
        MovementComp.GravityDown();
        if (AttackComp.IsAttackAnimation())
        {
            MovementComp.StopMove();
            Animator.SetFloat("Move", 1);
            return;
        }
        // 3. 이동 중일 때만 업데이트 (중복 호출 제거됨)
        if (MovementComp.IsMoving)
        {
            MovementComp.UpdateClickMove();
            Animator.SetFloat("Move", 1f);
            HandleFootStepTiming();
        }
        else
        {
            Animator.SetFloat("Move", 0f);
            stepTimer = 0;
        }
    }
    private void HandleFootStepTiming()
    {
        if (CheckAttackSkillDamage()) return;

        stepTimer += Time.deltaTime;

        if (stepTimer < currentStepInterval) return;
        stepTimer = 0;
        currentStepInterval = UnityEngine.Random.Range(interval.x, interval.y);

        GameManager.Instance.SoundManager.PlaySFXWithCooldown(footStepClip, 0.08f, 0.8f);
    }
    private void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        if (SkillComp.IsSkillAnimation(InputSkill.SKILLS.SKILL2))
        {
            if (isMove)
            {
                targetDir = GetMoveAimDir();
            }
        }
        else if (AttackComp.IsAttackAnimation() || SkillComp.IsSkillAnimation(currentSkill))
        {
            targetDir = targetLookDir;
        }
        if (targetDir == Vector3.zero) return;
        targetDir.y = 0;
        if (targetDir.sqrMagnitude > 0.001f) MovementComp.RotTarget(targetDir.normalized);
    }
    #endregion
    #region 공격 / 스킬 관련
    private void HandleAttackInput()
    {
        if (SkillComp.IsSkillAnimation(currentSkill) || GameManager.Instance.CurrentState == GameState.Town) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (!LockAimDir()) return;

        ClearTarget();
        MovementComp.StopMove();

        SetAttackLookDir(lockedAimDir);
        RotateToAttackDir();

        AttackComp.RequestAttack(aimPoint, lockedAimDir);
    }

    private void HandleSkill()
    {
        if (SkillComp.IsSkillAnimation(currentSkill)) return;

        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (!Input.GetKeyDown(skillKeys[i])) continue;
            InputSkill.SKILLS select = (InputSkill.SKILLS)i;
            if (SkillComp.CurrentSkillActive(select)) break;
            if (!LockAimDir()) break;

            ClearTarget();

            SetAttackLookDir(lockedAimDir);
            RotateToAttackDir();

            ExcuteSkillLogic(select);
            break;
        }

    }
    private void ExcuteSkillLogic(InputSkill.SKILLS skill)
    {
        currentSkill = skill;
        
        RotateToAttackDir();
        if(skill == InputSkill.SKILLS.SKILL5) SkillComp.ExcuteSkill(InputSkill.SKILLS.SKILL5);
        else if(skill == InputSkill.SKILLS.SKILL6) SkillComp.ExcuteSkill(InputSkill.SKILLS.SKILL6);
        else
        {
            if (skill == InputSkill.SKILLS.SKILL3)
            {
                SkillComp.ActiveSkill();
            }
            else
            {
                if (skill != InputSkill.SKILLS.SKILL3) StartCoroutine(SkillComp.WaitSkill(currentSkill));
                SkillComp.ActiveSkill(currentSkill);
            }
        }
    }
    public void SetAttackLookDir(Vector3 dir)
    {
        dir.y = 0f;
        if(dir.sqrMagnitude > 0.001f)
        {
            targetLookDir = dir.normalized;
        }
    }
    /// <summary>
    /// skill2인 경우, 현재 마우스 방향에 위치에 따라 이동해야함
    /// 현재 바우스 위치 방향이 있다면 마우스 위치로 아니라면 마지막 방향으로
    /// </summary>
    public Vector3 GetMoveAimDir()
    {
        if (aimDir.sqrMagnitude > 0.001f) return aimDir;
        if (lockedAimDir.sqrMagnitude > 0.001f) return lockedAimDir;
        return Vector3.zero;
    }

    private void RotateToAttackDir()
    {
        if(targetLookDir.sqrMagnitude > 0.001f)
        {
            MovementComp.LookAtInstant(targetLookDir);
        }
    }
    public override void Damage(float damage, float force, Transform attacker)
    {
        if (IsBlink || IsDamage) return;
        TrailOff();
        base.Damage(damage, force, attacker);
        if(SkillComp.IsSkillAnimation(currentSkill)) return;
        bool isBoss = false;
        if (attacker == null) return;
        var attackerStat = attacker.GetComponentInChildren<BaseController>() as EnemyController;
        if (attackerStat != null && attackerStat.StatComp.IsBoss) isBoss = true;
        if(isBoss)
        {
            Vector3 dir = (transform.position - attacker.position).normalized;
            dir.y = 0;
            Animator.SetTrigger("Damage");
            transform.forward = -dir;
            CameraShakeController.PlayMotionBlur(0.3f, 0.07f);
            CameraShakeController.ShakeCamDirectional(dir, 1.2f, 0.08f, 3.4f, true, 1.7f);
            CombatFeedback.HitStopByStrength(CombatFeedback.HitStrength.Boss);
            MovementComp.Push(dir, force, 0.08f);
        }
        else
        {
            CameraShakeController.GenerateImpulse(0.5f);
            CameraShakeController.ShakeCam(0.3f, 0.07f, 2.4f);
            CombatFeedback.HitStopByStrength(CombatFeedback.HitStrength.Light);
        }
        
    }
    public void OnAttackDash(float distance)
    {
        MovementComp.Push(transform.forward, distance, StatComp.KnckBackTime);
    }
    public void OnSkillAttack()
    {
        if (lockedAimDir == Vector3.zero) return;
        float dist = Vector3.Distance(transform.position, aimPoint);
        dist = Mathf.Clamp(dist, 0, 5);

        MovementComp.Push(lockedAimDir, dist, StatComp.KnckBackTime, true);
    }
    public void OnIsMove(int value)
    {
        isMove = (value != 0);
    }
    #endregion
    public void TrailOn()
    {
        trail.emitting = true;
    }
    public void TrailOff()
    {
        trail.emitting = false;
    }

    private void ClearTarget()
    {
        targetInteractable = null;
        isMoveToInteraction = false;
    }

    private bool CheckAttackSkillDamage()
    {
        return AttackComp.IsAttackAnimation() || SkillComp.IsSkillAnimation(currentSkill) || IsDamage;
    }

    private void OnDestroy()
    {
        StatComp.OncChangeLevel -= SkillComp.OnLockSkill;
    }


}
