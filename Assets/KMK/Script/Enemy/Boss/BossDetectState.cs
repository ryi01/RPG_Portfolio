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
        
        // 4. 아니라면 다른 공격을 랜덤으로 만든다
        if (dis <= boss.SkillList[0].AttackMinRange)
        {
            int rnd = Random.Range(0, 2);
            boss.ExccuteAttack(boss.SkillList[rnd], navMeshAgent);
            return;
        }
        // 2. 일정거리 이상이면
        else if (dis >= boss.SkillList[2].AttackMinRange && dis <= boss.SkillList[2].AttackMaxRange)
        {
            // 3. 대쉬 공격을 하고
            boss.ExccuteAttack(boss.SkillList[2], navMeshAgent);
            return;
        }
        else
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(controller.Player.transform.position);
        }
    }


}
