using UnityEngine;

public class EnemyStunState : EnemyState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        Anim.SetInteger("State", (int)state);
        Anim.SetTrigger("Stun");
    }
    public override void UpdateState()
    {
        if(!controller.StatComp.IsStun)
        {
            controller.TransactionToState(EnumTypes.STATE.IDLE);
        }
    }
}
