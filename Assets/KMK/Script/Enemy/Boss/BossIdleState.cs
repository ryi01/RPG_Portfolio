using UnityEngine;

public class BossIdleState : EnemyIdleState
{
    public override void UpdateState()
    {
        BossController boss = controller as BossController;

        if(boss.CoolTimeAttack)
        {
            return;
        }
        time += Time.deltaTime;
        float dis = controller.GetPlayerDis();
        // 1. 플레이어와 보스의 거리를 측정하고

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
        if (time > checkTime)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            time = 0;
        }
    }
}
