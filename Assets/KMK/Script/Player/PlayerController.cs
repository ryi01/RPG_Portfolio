using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStatComponent StatComp { get; private set; }
    public InputMovement MovementComp { get; private set; }
    public InputAttack AttackComp { get; private set; }
    public Animator Animator { get; private set; }

    private Vector3 attackDir;
    private Vector3 dir;
    private bool isAttack = false;

    private void Awake()
    {
        StatComp = GetComponent<PlayerStatComponent>();
        MovementComp = GetComponent<InputMovement>();
        AttackComp = GetComponent<InputAttack>();
        Animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleAttack();
        HandleRotation();
        
    }

    private void HandleMovement()
    {
        MovementComp.GravityDown();
        // 입력 이동 방향
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 입력값에 따라 dir 설정
        dir = new Vector3(h, 0, v).normalized;
        MovementComp.Move(dir);
    }
    private void HandleAttack()
    {
        AttackComp.ResetTrigger();
        if (Input.GetMouseButtonDown(0))
        {
            attackDir = (MovementComp.GetMouseWorldPos() - transform.position);
            attackDir.y = 0;
            isAttack = true;
            AttackComp.TriggerAttack();
        }
        AttackComp.UpdateAttackProgress();
        if (!AttackComp.IsAttackAnimation())
        {
            isAttack = false;
        }

    }
    private void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        if (isAttack)
        {
            targetDir = attackDir;
        }
        else if (dir != Vector3.zero)
        {
            targetDir = dir;
        }

        targetDir.y = 0;
        
        MovementComp.RotTarget(targetDir.normalized);
    }
    public void SetAnimFloat(string animString, float amount)
    {
        Animator.SetFloat(animString, amount);
    }
    public void SetAnimTrigger(int animIndex)
    {
        Animator.SetTrigger(animIndex);
    }
    public void SetAnimBool(int animIndex, bool value)
    {
        Animator.SetBool(animIndex, value);
    }
}
