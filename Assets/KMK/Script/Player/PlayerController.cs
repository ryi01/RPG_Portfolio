using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

// 강제적으로 컴포넌트와 컨트롤러를 세트로 만들어줌
[RequireComponent(typeof(PlayerStatComponent))]
// 추가로 할일
// 조준 보정 => 마우스포인터와 가장 가까이 있는 Enemy로 회전
public class PlayerController : BaseController<PlayerStatComponent>
{
    public InputMovement MovementComp { get; private set; }
    public InputAttack AttackComp { get; private set; }
    public InputSkill SkillComp { get; private set; }
    public InputPickUp PickUpComp { get; private set; }
    public CameraShakeController CameraShakeController { get; private set; }
    private Vector3 moveDir;
    private Vector3 targetLookDir;
    private Vector3 offsetToMouse;

    private bool isMove = false;
    public bool IsDamage { get; set; }
    public bool IsBlink { get; set; }

    private InputSkill.SKILLS currentSkill;
    private KeyCode[] skillKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.D, KeyCode.F };

    private ItemBox openBox;
    [SerializeField] private float interactionDistance = 3f;

    private NPCInteraction currentNPC;

    private InteractionObject targetInteractable;
    protected override void Awake()
    {
        base.Awake();
        MovementComp = GetComponent<InputMovement>();
        AttackComp = GetComponent<InputAttack>();
        SkillComp = GetComponent<InputSkill>();
        PickUpComp = GetComponent<InputPickUp>();
        CameraShakeController = GetComponentInChildren<CameraShakeController>();
        StatComp.OncChangeLevel += SkillComp.OnLockSkill;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsDamage || GameManager.Instance.CurrentState == GameState.Pause || GameManager.Instance.CurrentState == GameState.Dialogue)
        {
            MovementComp.StopMove();
            return;
        }
        
        HandleInput();
        HandleMovement();   
        HandleRotation();
        HandleUseItem();
        if (GameManager.Instance.CurrentState != GameState.Town) HandleSkill();
        CheckInteractionDistance();
        CheckBox();
    }
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

    private void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(1))
        {
            
            int layerMask = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Obstacle"));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layerMask))
            {
                Debug.Log($"{hitInfo.collider.name}");
                InteractionObject interactable = hitInfo.collider.GetComponentInParent<InteractionObject>();
                if (interactable != null)
                {
                    targetInteractable = interactable;
                    MovementComp.FindPath(interactable.GetTransform().position);
                    return;
                }
                targetInteractable = null;
                Vector3 groundPos = hitInfo.point;
                groundPos.y = transform.position.y;
                MovementComp.FindPath(groundPos);
            }
        }
        HandleAttackInput();
    }
    private void CheckInteractionDistance()
    {
        if(targetInteractable != null)
        {
            float dist = Vector3.Distance(transform.position, targetInteractable.transform.position);
            if(dist<= interactionDistance)
            {
                MovementComp.StopMove();
                targetInteractable.Interact(this);
                targetInteractable = null;
            }
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
        currentNPC = null;
    }

    private void HandleMovement()
    {
        if (SkillComp.IsSkillAnimation(currentSkill))
        {
            if(currentSkill == InputSkill.SKILLS.SKILL2)
            {
                if(isMove)
                {
                    MovementComp.Move(offsetToMouse.normalized);
                }
                Animator.SetFloat("Move", 0);
            }
            return;
        }
        MovementComp.GravityDown();
        if (AttackComp.IsAttackAnimation())
        {
            MovementComp.StopMove();
            Animator.SetFloat("Move", 2);
        }
        else
        {
            // 3. 이동 중일 때만 업데이트 (중복 호출 제거됨)
            if (MovementComp.IsMoving)
            {
                MovementComp.UpdateClickMove();
                Animator.SetFloat("Move", 1f);
            }
            else
            {
                Animator.SetFloat("Move", 0f);
            }
        }

        
    }
    private void HandleAttackInput()
    {
        offsetToMouse = MovementComp.GetMouseWorldPos() - transform.position;
        offsetToMouse.y = 0;
        AttackComp.UpdateAttackProgress();
        if (SkillComp.IsSkillAnimation(currentSkill) || GameManager.Instance.CurrentState == GameState.Town) return;
        if (Input.GetMouseButtonDown(0))
        {
            MovementComp.StopMove();
            AttackComp.TriggerAttack(MovementComp.GetMouseWorldPos());
            UpdateAttackDir();
        }
    }
    private void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        if (SkillComp.IsSkillAnimation(InputSkill.SKILLS.SKILL2))
        {
            if (isMove)
            {
                targetDir = offsetToMouse;
            }
        }
        else if (AttackComp.IsAttackAnimation() || SkillComp.IsSkillAnimation(currentSkill))
        {
            targetDir = targetLookDir;
        }
        else if (moveDir.sqrMagnitude > 0)
        {
            targetDir = moveDir;
        }
        if(targetDir != Vector3.zero)
        {
            targetDir.y = 0;
            MovementComp.RotTarget(targetDir.normalized);
        }        
    }
    private void HandleSkill()
    {
        if (SkillComp.IsSkillAnimation(currentSkill)) return;

        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (Input.GetKeyDown(skillKeys[i]))
            {
                InputSkill.SKILLS select = (InputSkill.SKILLS)i;
                if (!SkillComp.CurrentSkillActive(select))
                {
                    ExcuteSkillLogic(select);
                }
                break;
            }
        }

    }
    private void ExcuteSkillLogic(InputSkill.SKILLS skill)
    {
        currentSkill = skill;
        
        UpdateAttackDir();
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

    private void UpdateAttackDir()
    {
        if(offsetToMouse.sqrMagnitude > 0.001f)
        {
            targetLookDir = offsetToMouse.normalized;
            MovementComp.LookAtInstant(targetLookDir);
        }
    }
    public override void Damage(float damage, float force, Transform attacker)
    {
        if (IsBlink || IsDamage) return;
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
            MovementComp.Push(dir, force, 0.1f);
        }
        else
        {
            CameraShakeController.ShakeCam(0.3f, 0.2f);
        }
    }
    public void OnAttackDash(float distance)
    {
        if (currentSkill == InputSkill.SKILLS.SKILL3)
        {
            distance = Mathf.Clamp(offsetToMouse.magnitude, 0f, 5f);
        }
        MovementComp.Push(transform.forward, distance, StatComp.KnckBackTime);
    }
    public void OnIsMove(int value)
    {
        isMove = (value != 0);
    }
    private void OnDestroy()
    {
        StatComp.OncChangeLevel -= SkillComp.OnLockSkill;
    }


}
