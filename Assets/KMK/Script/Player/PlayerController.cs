using System.Collections;
using UnityEngine;

public class PlayerController : BaseController<PlayerStatComponent>
{
    public InputMovement MovementComp { get; private set; }
    public InputAttack AttackComp { get; private set; }
    public InputSkill SkillComp { get; private set; }

    private Vector3 moveDir;
    private Vector3 attackDir;
    private Vector3 finalPos;
    private bool isAttack = false;
    private bool isMove = false;
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
    }

    private void HandleInput()
    {
        // 입력 이동 방향
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 입력값에 따라 dir 설정
        moveDir = new Vector3(h, 0, v).normalized;
        AttackComp.UpdateAttackProgress();
        if (SkillComp.IsSkillAnimation(currentSkill)) return;
        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetLayerWeight(1, 1);
            AttackComp.TriggerAttack();
            UpdateAttackDir();
        }
        if (AttackComp.IsAttackAnimation())
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }
    }

    private void HandleMovement()
    {
        if (SkillComp.IsSkillAnimation(currentSkill))
        {
            if(currentSkill == InputSkill.SKILLS.SKILL2)
            {
                if(isMove)
                {
                    MovementComp.Move(moveDir);
                }
                Animator.SetFloat("Move", 0);
            }
            return;
        }
        MovementComp.GravityDown();
        if (isAttack) return;
        MovementComp.Move(moveDir);
        Animator.SetFloat("Move", moveDir.normalized.magnitude);
    }
    private void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        if (SkillComp.IsSkillAnimation(InputSkill.SKILLS.SKILL2))
        {
            if (moveDir != Vector3.zero)
            {
                targetDir = moveDir;
                transform.rotation = Quaternion.LookRotation(targetDir);
            }
            else return;
        }
        else if (isAttack || SkillComp.IsSkillAnimation(currentSkill))
        {
            targetDir = attackDir;
        }
        else if (moveDir != Vector3.zero)
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

        switch(skill)
        {
            case InputSkill.SKILLS.SKILL1:
                StartAnim(1);
                break;
            case InputSkill.SKILLS.SKILL3:
                UpdateAttackDir();
                Animator.SetLayerWeight(1, 0);
                SkillComp.ActiveSkill();
                break;
            case InputSkill.SKILLS.SKILL4:
                SkillComp.ExcuteSkill(InputSkill.SKILLS.SKILL4);
                break;
            default:
                StartAnim();
                break;
        }
    }
    private void StartAnim(int weight = 0)
    {
        UpdateAttackDir();
        Animator.SetLayerWeight(1, weight);
        StartCoroutine(SkillComp.WaitSkill(currentSkill));
        SkillComp.ActiveSkill(currentSkill);
    }
    private void UpdateAttackDir()
    {
        finalPos = (MovementComp.GetMouseWorldPos() - transform.position);
        finalPos.y = 0;
        if(finalPos.sqrMagnitude > 0.001f)
        {
            attackDir = finalPos;
            MovementComp.LookAtInstant(attackDir.normalized);
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
            distance = Mathf.Clamp(finalPos.magnitude, 0f, 5f);
        }
        MovementComp.Push(transform.forward, distance, StatComp.KnckBackTime);
    }
    public void OnIsMove(int value)
    {
        isMove = (value != 0);
    }
}
