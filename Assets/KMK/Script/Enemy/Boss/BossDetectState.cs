using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDetectState : EnemyDetectState
{

    public override void UpdateState()
    {
        // 1. 플레이어와 보스의 거리를 측정하고
        float dis = controller.GetPlayerDis();
        BossController boss = controller as BossController;

        EnemySkillAttack dash = boss.SkillList[2];
        if (dis >= dash.AttackMinRange && dis <= dash.AttackMaxRange)
        {
            boss.ExccuteAttack(dash, navMeshAgent);
            return;
        }
        if(dis <= boss.SkillList[0].AttackMaxRange)
        {
            int rnd = Random.Range(0, 2);
            boss.ExccuteAttack(boss.SkillList[rnd], navMeshAgent);
            return;
        }
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(controller.Player.transform.position);
        if (dis > fsmInfo.DetectRange)
        {
            controller.TransactionToState(EnumTypes.STATE.IDLE);
        }
    }


}
