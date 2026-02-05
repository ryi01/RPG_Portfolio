using UnityEngine;

public class EnemyDetectState : EnemyState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
    }

    public override void UpdateState()
    {
        float dis = controller.GetPlayerDis();
        if(dis <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if(dis > fsmInfo.DetectRange)
        {
            controller.TransactionToState(EnumTypes.STATE.RETURN);
            return;

        }
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(controller.Player.transform.position);
    }

}
