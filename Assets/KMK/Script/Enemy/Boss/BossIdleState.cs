using UnityEngine;

public class BossIdleState : EnemyIdleState
{
    private float nextAttackTime = 0f;
    private const float ATTACK_COOLDOWN = 1;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        nextAttackTime = Time.time + ATTACK_COOLDOWN;
    }
    public override void UpdateState()
    {
        if (Time.time < nextAttackTime) return;

        float dis = controller.GetPlayerDis();
        if (dis <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if (dis <= fsmInfo.DetectRange)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            return;
        }


    }

}
