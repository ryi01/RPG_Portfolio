using UnityEngine;

public class EnemyStunState : EnemyState
{
    [SerializeField] protected ParticleSystem stunParticle;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        stunParticle.Play();
        NavigationStop();
        Anim.SetInteger("State", (int)state);
        Anim.SetTrigger("Stun");
    }
    public override void UpdateState()
    {
        if(!controller.StatComp.IsStun)
        {
            stunParticle.Stop();
            controller.TransactionToState(EnumTypes.STATE.IDLE);
        }
    }
    public override void ExitState()
    {
        stunParticle.Stop();
        base.ExitState();
    }
}
