using UnityEngine;

public class BossDetectState : EnemyDetectState
{
    public override void UpdateState()
    {
        float dis = controller.GetPlayerDis();
        BossController boss = controller as BossController;
        if (boss.CheckSkillReady())
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(controller.Player.transform.position);
    }
}
