using UnityEngine;

public class BossDetectState : EnemyDetectState
{
    public override void UpdateState()
    {
        float dis = controller.GetPlayerDis();
        if (dis <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(controller.Player.transform.position);
    }
}
