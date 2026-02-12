using UnityEngine;

public class BossIdleState : EnemyIdleState
{
    public override void UpdateState()
    {
        time += Time.deltaTime;
        float dis = controller.GetPlayerDis();
        if (time > checkTime)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            time = 0;
        }
    }
}
