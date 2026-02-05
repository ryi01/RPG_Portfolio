using UnityEngine;

public class EnemyController : BaseController<EnemyStatComponent>
{
    private EnemyState currentState;
    [SerializeField] private EnemyState[] enemyStates;
    private GameObject player;
    public GameObject Player { get => player; set => player = value; }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        TransactionToState(EnumTypes.STATE.IDLE);
    }
    private void Update()
    {
        currentState?.UpdateState();
    }
    public override void Damage(float damage, float force)
    {

    }
    public void TransactionToState(EnumTypes.STATE state, object data = null)
    {
        if (currentState == enemyStates[(int)EnumTypes.STATE.DEATH]) return;
        currentState?.ExitState();
        currentState = enemyStates[(int)state];
        currentState?.EnterState(state, data);
    }
    public float GetPlayerDis()
    {
        return Vector3.Distance(transform.position, Player.transform.position);
    }

    public void SetAnimatorState(EnumTypes.STATE state)
    {
        Animator.SetInteger("State", (int)state);
    }
}
