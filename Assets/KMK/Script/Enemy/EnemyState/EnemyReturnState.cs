using UnityEngine;

public class EnemyReturnState : EnemyRoamingState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        controller.NavMeshAgent.speed = fsmInfo.MoveSpeed;
        base.EnterState(state, data);
        NewRandDestination();
    }

}
