using System.Collections;
using UnityEngine;

public class EnemyDamageState : EnemyHitState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        // ÆÄÆ¼Å¬ 
    }

    public override void UpdateState()
    {
        if (fsmInfo.IsHit) return;
        if(controller.GetPlayerDis() <= fsmInfo.AttackRadius)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        controller.TransactionToState(EnumTypes.STATE.DETECT);
    }


}
