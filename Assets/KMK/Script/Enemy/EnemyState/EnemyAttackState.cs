using UnityEngine;

public class EnemyAttackState : EnemyState
{
    protected bool isAttack = false;
    public bool IsAttack { get => isAttack; set => isAttack = value; }
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        controller.NavigationStop();
        Anim.SetInteger("State", (int)state);
    }
    public override void UpdateState()
    {
        if (CheckDeath()) return;

        if (controller.GetPlayerDis() > statComp.AttackRange)
        {
            controller.TransitionToState(EnumTypes.STATE.DETECT);
            return;
        }
        if (IsAttack) return;
        LookAtTarget();
    }

    public override void ExitState()
    {
        base.ExitState();
        IsAttack = false;
    }

}
