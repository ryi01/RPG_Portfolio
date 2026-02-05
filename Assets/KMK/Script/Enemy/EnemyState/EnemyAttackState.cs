using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool isAttack = false;
    public bool IsAttack { get => isAttack; set => isAttack = value; }
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        NavigationStop();
    }
    public override void UpdateState()
    {
        if (IsAttack) return;
        if(controller.GetPlayerDis() > fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.RETURN);
            return;
        }
        LookAtTarget();
    }

    protected void LookAtTarget()
    {
        Vector3 dir = (controller.Player.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * fsmInfo.RotSpeed);
    }

}
