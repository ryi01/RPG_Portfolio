using UnityEngine;

public class EnemyStunState : EnemyState
{
    [SerializeField] protected ParticleSystem stunParticle;

    private float stunTimer;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        stunParticle.Play();
        float duration = (data != null) ? (float)data : 3.0f;
        stunTimer = duration;
        NavigationStop();
        Anim.SetInteger("State", (int)state);
        Anim.SetTrigger("Stun");
    }
    public override void UpdateState()
    {
        stunTimer -= Time.deltaTime;
        if(stunTimer <= 0)
        {
            controller.TransactionToState(EnumTypes.STATE.IDLE);
        }
    }
    public override void ExitState()
    {
        stunParticle.Stop();
        base.ExitState();
    }
}
