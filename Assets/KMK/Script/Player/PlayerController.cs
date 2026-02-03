using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStatComponent StatComp { get; private set; }
    public InputMovement MovementComp { get; private set; }
    public InputAttack AttackComp { get; private set; }
    public Animator Animator { get; private set; }

    private Vector3 moveDir;
    private Vector3 attackDir;
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
        HandleInput();
        HandleMovement();
        HandleRotation();
        
    }
    private void HandleInput()
    {
        // 입력 이동 방향
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 입력값에 따라 dir 설정
        moveDir = new Vector3(h, 0, v).normalized;
        AttackComp.UpdateAttackProgress();
        if (Input.GetMouseButtonDown(0))
        {
            AttackComp.TriggerAttack();
            UpdateAttackDir();
        }
        if (AttackComp.IsAttackAnimation() || Animator.IsInTransition(0))
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
        MovementComp.GravityDown();
        if (isAttack || Animator.IsInTransition(0))
        {
            return;
        }
        MovementComp.Move(moveDir);
    }
    private void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;
        if (isAttack && attackDir != Vector3.zero)
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
    private void UpdateAttackDir()
    {
        Vector3 diff = (MovementComp.GetMouseWorldPos() - transform.position);
        diff.y = 0;
        if(diff.sqrMagnitude > 0.001f)
        {
            attackDir = diff;
        }
    }
    
    public void OnAttackDash(float distance)
    {
        MovementComp.Push(transform.forward, distance, 0.1f);
    }
}
