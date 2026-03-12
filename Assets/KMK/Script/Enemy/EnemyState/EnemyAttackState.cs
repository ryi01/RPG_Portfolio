using UnityEngine;

public class EnemyAttackState : EnemyState
{
    protected bool isAttack = false;
    public bool IsAttack { get => isAttack; set => isAttack = value; }
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        NavigationStop();
        Anim.SetInteger("State", (int)state);
    }
    public override void UpdateState()
    {
        if (CheckDeath()) return;

        if (controller.GetPlayerDis() > fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            return;
        }
        if (IsAttack) return;
        LookAtTarget();
    }
    protected void LookAtTarget()
    {
        Vector3 dir = (controller.Player.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * fsmInfo.RotSpeed);
    }

    public override void ExitState()
    {
        base.ExitState();
        IsAttack = false;
    }

}
