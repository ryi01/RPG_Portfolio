using UnityEngine;

public class EnemyWanderState : EnemyRoamingState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        controller.NavMeshAgent.speed = statComp.MoveSpeed;
        base.EnterState(state, data);
        NewRandDestination();
    }
}
