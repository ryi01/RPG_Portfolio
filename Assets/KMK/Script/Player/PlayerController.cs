using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private float autoCloseDistance = 3;

    private bool isInteraction = false;
    private NPCInteraction currentNPC;
    

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
        if (IsDamage) return;
        HandleInput();
        HandleMovement();
        HandleRotation();
        HandleSkill();
        HandleInteraction();
        CheckBoxDistance();
    }
    public void HandleInteraction()
    {
        if (isInteraction == false) return;
        if(Input.GetKeyDown(KeyCode.G))
        {
            currentNPC.Interact();
        }
    }

    private void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(1))
        {
            int layerMask = (1 << LayerMask.NameToLayer("Environment")) | (1 <<LayerMask.NameToLayer("Item"));

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layerMask))
            {
                if (hitInfo.collider.CompareTag("Item"))
                {
                    ItemBox box = hitInfo.collider.GetComponent<ItemBox>();
                    Vector3 targetPos = box.transform.position;
                    targetPos.y = transform.position.y;
                    
                    MovementComp.SetTarget(targetPos);
                    // 람다식 사용 이유 : 상자에 도착시에 오픈하는 단발성 이벤트임
                    // 람다식 : 이름없는 함수 => 짧은 기능이나 콜백 등의 용도로 사용됨
                    // OnArrival = (매개변수) => {식 / 함수 몸체}
                    // 매개변수를 입력받아 오른쪽처럼 행동해라
                    MovementComp.OnArrival = () =>
                    {
                        if (box != null)
                        {
                            openBox = box;
                            PickUpComp.OpenItemBox(box);
                        }
                    };
                    return;
                }
                else
                {
                    MovementComp.OnArrival = null;
                    MovementComp.SetTarget(MovementComp.GetMouseWorldPos());
                }
            }
            
        }

        offsetToMouse = MovementComp.GetMouseWorldPos() - transform.position;
        offsetToMouse.y = 0;
        AttackComp.UpdateAttackProgress();
        if (SkillComp.IsSkillAnimation(currentSkill)) return;
        if (Input.GetMouseButtonDown(0))
        {
            MovementComp.StopMove();
            AttackComp.TriggerAttack(MovementComp.GetMouseWorldPos());
            UpdateAttackDir();
        }
    }
    private void CheckBoxDistance()
    {
        if (openBox == null) return;
        Vector3 playerPos = transform.position;
        playerPos.y = 0;
        Vector3 boxPos = openBox.transform.position;
        boxPos.y = 0;
        float dis = Vector3.Distance(playerPos, boxPos);
        if(dis > autoCloseDistance)
        {
            PickUpComp.CloseUI();
            openBox = null;
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
                    MovementComp.Move(offsetToMouse.normalized);
                }
                Animator.SetFloat("Move", 0);
            }
            return;
        }
        MovementComp.GravityDown();
        float animMoveValue;
        if (AttackComp.IsAttackAnimation())
        {
            animMoveValue = 2.0f;
            MovementComp.StopMove();
        }
        else
        {
            animMoveValue = MovementComp.IsMoving ? 1f : 0f;
            MovementComp.UpdateClickMove();
        }

        Animator.SetFloat("Move", animMoveValue);
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NPC"))
        {
            isInteraction = true;
            currentNPC = other.GetComponent<NPCInteraction>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isInteraction = false;
        }
    }
}
