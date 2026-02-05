using UnityEngine;

public class EnemyReturnState : EnemyRoamingState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        fsmInfo.SetSpeedMultifle(1.5f);
        navMeshAgent.speed = fsmInfo.MoveSpeed;
        base.EnterState(state, data);
        NewRandDestination();
    }

}
