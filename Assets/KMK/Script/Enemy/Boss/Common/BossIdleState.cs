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
        if (controller.Player != null) RotateToPlayer();
        if (Time.time < nextAttackTime) return;

        float dis = controller.GetPlayerDis();

        if (dis <= statComp.AttackRange)
        {
            controller.TransitionToState(EnumTypes.STATE.ATTACK);
            return;
        }

        if (dis <= statComp.DetectRange)
        {
            controller.TransitionToState(EnumTypes.STATE.DETECT);
            return;
        }

    }
    private void RotateToPlayer()
    {
        Vector3 dir = controller.Player.transform.position - controller.transform.position;

        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        controller.transform.rotation = Quaternion.LookRotation(dir);
    }
}
