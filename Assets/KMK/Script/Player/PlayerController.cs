using System.Collections;
using UnityEngine;

// 강제적으로 컴포넌트와 컨트롤러를 세트로 만들어줌
[RequireComponent(typeof(PlayerStatComponent))]
// 추가로 할일
// 조준 보정 => 마우스포인터와 가장 가까이 있는 Enemy로 회전
public class PlayerController : BaseController<PlayerStatComponent>
{
    public InputMovement MovementComp { get; private set; }
    public InputAttack AttackComp { get; private set; }
    public InputSkill SkillComp { get; private set; }

    private Vector3 moveDir;
    private Vector3 targetLookDir;
    private Vector3 offsetToMouse;

    private bool isMove = false;
    private float currentMoveValue;

    private InputSkill.SKILLS currentSkill;
    private KeyCode[] skillKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

    protected override void Awake()
    {
        base.Awake();
        MovementComp = GetComponent<InputMovement>();
        AttackComp = GetComponent<InputAttack>();
        SkillComp = GetComponent<InputSkill>();
    }
    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
        HandleSkill();
        HandleRun();
    }

    private void HandleInput()
    {
        // 입력 이동 방향
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 입력값에 따라 dir 설정
        moveDir = new Vector3(h, 0, v).normalized;
        offsetToMouse = MovementComp.GetMouseWorldPos() - transform.position;
        offsetToMouse.y = 0;
        AttackComp.UpdateAttackProgress();
        if (SkillComp.IsSkillAnimation(currentSkill)) return;
        if (Input.GetMouseButtonDown(0))
        {
            AttackComp.TriggerAttack();
            UpdateAttackDir();
        }
    }

    private void HandleRun()
    {
        // 입력값이 있는지 없는지 확인
        bool isInput = Input.GetKey(KeyCode.LeftShift) && moveDir.magnitude > 0 && StatComp.CurrentST > 0;
        // isST = true
        bool isST = isInput && StatComp.UseST(Time.deltaTime);
        float speedMult = isST ? 2 : 1;
        StatComp.SetSpeedMultifle(speedMult);
        currentMoveValue = moveDir.magnitude > 0 ? speedMult : 0;
        StatComp.ReganST(Time.deltaTime);
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
        if (AttackComp.IsAttackAnimation()) return;
        MovementComp.Move(moveDir);
        Animator.SetFloat("Move", currentMoveValue);
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
        if(skill == InputSkill.SKILLS.SKILL4) SkillComp.ExcuteSkill(InputSkill.SKILLS.SKILL4);
        else
        {
            if (skill == InputSkill.SKILLS.SKILL3)
            {
                SkillComp.ActiveSkill();
            }
            else
            {
                if(skill != InputSkill.SKILLS.SKILL3) StartCoroutine(SkillComp.WaitSkill(currentSkill));
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
    public override void Damage(float damage, float force)
    {
        base.Damage(damage, force);
        MovementComp.Push(-transform.forward, force, 0.1f);
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
}
