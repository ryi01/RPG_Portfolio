using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStatComponent))]
public class EnemyController : BaseController<EnemyStatComponent>
{
    private EnemyState currentState;
    [SerializeField] private EnemyState[] enemyStates;
    private GameObject player;
    public GameObject Player { get => player; set => player = value; }

    private Dictionary<EnumTypes.STATE, EnemyState> stateDict = new();

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
    private void Update()
    {
        currentState?.UpdateState();
        StatComp?.UpdateStunStatus();
    }
    public override void Damage(float damage, float force)
    {
        base.Damage(damage, force);

        if (StatComp.CurrentHP <= 0)
        {
            TransactionToState(EnumTypes.STATE.DEATH, force);
            return;
        }
        bool isStun = StatComp.AddGroogy(damage);
        if(isStun)
        {
            TransactionToState(EnumTypes.STATE.STUN);
            return;
        }
        if (!stateDict.TryGetValue(EnumTypes.STATE.STUN, out var stunState))
        {
            TransactionToState(EnumTypes.STATE.DAMAGE, force);
        }
    }
    public void ForceStun(float duration)
    {
        StatComp.AddGroogy(StatComp.MaxGroogy);
        TransactionToState(EnumTypes.STATE.STUN);
    }
    public void TransactionToState(EnumTypes.STATE state, object data = null)
    {
        if (stateDict.TryGetValue(EnumTypes.STATE.DEATH, out var deathState))
        {
            if(currentState == deathState) return;
        }
        if (!stateDict.TryGetValue(state, out EnemyState nextState))
        {
            return;
        }
        if (currentState == nextState) return;
        if (state == EnumTypes.STATE.STUN && currentState == nextState) return;
        currentState?.ExitState();
        currentState = enemyStates[(int)state];
        currentState?.EnterState(state, data);
    }
    public float GetPlayerDis()
    {
        return Vector3.Distance(transform.position, Player.transform.position);
    }

}
