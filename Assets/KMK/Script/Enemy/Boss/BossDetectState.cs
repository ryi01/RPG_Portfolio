using NUnit.Framework;
using System.Collections.Generic;
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
            ExccuteAttack(boss, boss.SkillList[rnd]);
            return;
        }
        // 2. 일정거리 이상이면
        else if (dis >= boss.SkillList[2].AttackMinRange && dis <= boss.SkillList[2].AttackMaxRange)
        {
            // 3. 대쉬 공격을 하고
            ExccuteAttack(boss, boss.SkillList[2]);
            return;
        }
        else
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(controller.Player.transform.position);
        }
    }

    private void ExccuteAttack(BossController boss, EnemySkillAttack skill)
    {
        if(skill.SkillIndex == 2)
        {
            if(boss.TryGetComponent<BossAttackState>(out BossAttackState bossAttack))
            {
                Vector3 pPos = boss.Player.transform.position;
                Vector3 bPos = transform.position;
                pPos.y = 0;
                bPos.y = 0;
                Vector3 dir = (pPos - bPos).normalized;
                bossAttack.DashDir = dir == Vector3.zero ? transform.forward : dir;
            }
        }
        navMeshAgent.isStopped = true;
        boss.TransactionToState(EnumTypes.STATE.ATTACK, skill);
    }
}
