using System.Collections;
using UnityEngine;

public class PlayerController : BaseController
{
    public InputMovement MovementComp { get; private set; }
    public InputAttack AttackComp { get; private set; }
    public InputSkill SkillComp { get; private set; }

    private Vector3 moveDir;
    private Vector3 attackDir;
    private bool isAttack = false;

    protected override void Awake()
    {
        base.Awake();
        StatComp = GetComponent<PlayerStatComponent>();
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
        if (SkillComp.IsSkillAnimation(currentSkill)) return;
        MovementComp.GravityDown();
        if (isAttack) return;
        MovementComp.Move(moveDir);
    }
    private void HandleRotation()
    {
        if (SkillComp.IsSkillAnimation(InputSkill.SKILLS.SKILL2)) return;
        Vector3 targetDir = Vector3.zero;
        if (isAttack)
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
    private InputSkill.SKILLS currentSkill;
    private void HandleSkill()
    {
        if (SkillComp.IsSkillAnimation(currentSkill)) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && !SkillComp.CurrentSkillActive(InputSkill.SKILLS.SKILL1))
        {
            currentSkill = InputSkill.SKILLS.SKILL1;
            UpdateAttackDir();
            Animator.SetLayerWeight(1, 1);
            StartAnim();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && !SkillComp.CurrentSkillActive(InputSkill.SKILLS.SKILL2))
        {
            currentSkill = InputSkill.SKILLS.SKILL2;
            Animator.SetLayerWeight(1, 0);
            StartAnim();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && !SkillComp.CurrentSkillActive(InputSkill.SKILLS.SKILL3))
        {
            currentSkill = InputSkill.SKILLS.SKILL3;
            Animator.SetLayerWeight(1, 0);
            StartAnim();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && !SkillComp.CurrentSkillActive(InputSkill.SKILLS.SKILL4))
        {
            currentSkill = InputSkill.SKILLS.SKILL4;
            SkillComp.ExcuteSkill(InputSkill.SKILLS.SKILL4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && !SkillComp.CurrentSkillActive(InputSkill.SKILLS.SKILL5))
        {
            currentSkill = InputSkill.SKILLS.SKILL5;
            Animator.SetLayerWeight(1, 0);
            StartAnim();
        }

    }
    private void StartAnim()
    {
        StartCoroutine(SkillComp.WaitSkill(currentSkill));
        SkillComp.ActiveSkill(currentSkill);
    }
    private void UpdateAttackDir()
    {
        Vector3 diff = (MovementComp.GetMouseWorldPos() - transform.position);
        diff.y = 0;
        if(diff.sqrMagnitude > 0.001f)
        {
            attackDir = diff;
        }
    }
    public override void Damage(float damage, float force)
    {
        StatComp.TakeDamage(damage);
        MovementComp.Push(-transform.forward, force, 0.1f);
    }
    public void OnAttackDash(float distance)
    {
        MovementComp.Push(transform.forward, distance, 0.1f);
    }

}
