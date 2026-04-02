using UnityEngine;

public class EnemyStunState : EnemyState
{
    [SerializeField] protected ParticleSystem stunParticle;
    [SerializeField] private float defaultStunDuration = 3f;

    private float stunTimer;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        
        stunTimer = data is float stunDuration ? stunDuration : defaultStunDuration;
        controller.NavigationStop();
        if(stunParticle != null) stunParticle.Play();
        if(Anim != null)
        {
            Anim.SetInteger("State", (int)state);
            Anim.SetTrigger("Stun");
        }
    }
    public override void UpdateState()
    {
        if (CheckDeath()) return;
        stunTimer -= Time.deltaTime;
        if(stunTimer <= 0)
        {
            controller.TransitionToState(EnumTypes.STATE.IDLE);
        }
    }
    public override void ExitState()
    {
        if (stunParticle != null)
        {
            stunParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
