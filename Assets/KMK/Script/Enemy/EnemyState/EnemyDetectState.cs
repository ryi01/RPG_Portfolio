using UnityEngine;

public class EnemyDetectState : EnemyState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;
        controller.NavMeshAgent.speed = fsmInfo.SetSpeedMultifle(1.5f);
        base.EnterState(state, data);
        Anim.SetInteger("State", (int)state);
    }

    public override void UpdateState()
    {
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;
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
        controller.NavMeshAgent.isStopped = false;
        controller.NavMeshAgent.SetDestination(controller.Player.transform.position);
    }

}
