using UnityEngine;

public class EnemyReturnState : EnemyRoamingState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        navMeshAgent.speed = fsmInfo.MoveSpeed;
        base.EnterState(state, data);
        NewRandDestination();
    }

}
