using UnityEngine;

public class BossIdleState : EnemyIdleState
{
    public override void UpdateState()
    {
        time += Time.deltaTime;
        float dis = controller.GetPlayerDis();
        if (dis <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if (time > checkTime)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
        }
    }
}
