using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EnemyStatComponent))]
public class EnemyController : BaseController<EnemyStatComponent>
{
    protected EnemyState currentState;
    [SerializeField] protected EnemyState[] enemyStates;
    protected GameObject player;
    public GameObject Player { get => player; set => player = value; }

    protected Dictionary<EnumTypes.STATE, EnemyState> stateDict = new();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        foreach(var state in enemyStates)
        {
            if(state != null)
            {
                stateDict[state.StateType] = state;
            }
        }
        TransactionToState(EnumTypes.STATE.IDLE);
    }
    protected virtual void Update()
    {
        currentState?.UpdateState();
        StatComp?.UpdateStunStatus();
    }
    public override void Damage(float damage, float force, Transform attacker)
    {
        if (currentState != null && currentState.StateType == EnumTypes.STATE.DEATH) return;
        base.Damage(damage, force, attacker);

        if (StatComp.CurrentHP <= 0)
        {
            TransactionToState(EnumTypes.STATE.DEATH, force);
            return;
        }
        if(currentState != null && currentState.StateType == EnumTypes.STATE.STUN)
        {
            Animator.SetInteger("State", 5);
            return;
        }
        bool isStun = StatComp.AddGroogy(damage);
        if(isStun)
        {
            TransactionToState(EnumTypes.STATE.STUN);
            return;
        }
        TransactionToState(EnumTypes.STATE.DAMAGE, force);
    }
    public void ForceStun(float duration)
    {
        StatComp.AddGroogy(StatComp.MaxGroogy);
        TransactionToState(EnumTypes.STATE.STUN);
    }
 
    public virtual void TransactionToState(EnumTypes.STATE state, object data = null)
    {
        if (currentState != null && currentState.StateType == EnumTypes.STATE.DEATH) return;
        if (!stateDict.TryGetValue(state, out EnemyState nextState))
        {
            return;
        }
        if (currentState == nextState && data == null) return;
        if (state == EnumTypes.STATE.STUN && currentState == nextState) return;
        currentState?.ExitState();
        currentState = nextState;
        currentState?.EnterState(state, data);
    }
    public float GetPlayerDis()
    {
        return Vector3.Distance(transform.position, Player.transform.position);
    }

}
