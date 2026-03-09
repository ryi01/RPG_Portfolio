using UnityEngine;

public class BossIdleState : EnemyIdleState
{
    public override void UpdateState()
    {
        BossController boss = controller as BossController;
        float dis = controller.GetPlayerDis();

        if (boss.CoolTimeAttack) return;

        if (dis <= fsmInfo.DetectRange)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
        }
        EnemySkillAttack availableSkill = boss.GetAvailableSkill(dis);
        if (availableSkill != null)
        {
            boss.ExccuteAttack(availableSkill, navMeshAgent);
            return;
        }
    }

}
