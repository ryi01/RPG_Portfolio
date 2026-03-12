using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState : MonoBehaviour
{
    protected EnemyController controller;

    [SerializeField]private EnumTypes.STATE stateType;
    public EnumTypes.STATE StateType { get => stateType; }

    // 애니메이션 재생속도
    [Range(1f, 2f)]
    [SerializeField] protected float animSpeed;

    protected EnemyStatComponent fsmInfo;
    protected Animator Anim { get => controller.Animator; }

    protected NavMeshAgent navMeshAgent;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        if (controller != null)
        {
            fsmInfo = controller.StatComp;
        }
        navMeshAgent.avoidancePriority = Random.Range(0, 99);
    }
    public virtual void EnterState(EnumTypes.STATE state, object data = null)
    {
        Anim.speed = animSpeed;  
    }

    public abstract void UpdateState();
    public virtual void ExitState() { }

    protected virtual void NavigationStop()
    {
        if(navMeshAgent != null && navMeshAgent.gameObject.activeSelf && navMeshAgent.isOnNavMesh )
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.speed = 0;
        }
        
    }

    protected virtual bool CheckDeath()
    {
        if(controller.StatComp.CurrentHP <= 0)
        {
            controller.TransactionToState(EnumTypes.STATE.DEATH);
            return true;
        }
        return false;   
    }

}
