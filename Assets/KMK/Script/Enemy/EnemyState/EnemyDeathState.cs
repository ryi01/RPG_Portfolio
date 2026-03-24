using System;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    protected float time = 0;
    [SerializeField] protected ParticleSystem deathParticle;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        time = 0;
        statComp.IsHit = false;
        controller.NavigationStop();
        if (controller.NavMeshAgent != null && controller.NavMeshAgent.enabled) controller.NavMeshAgent.enabled = false;
        if (deathParticle != null) deathParticle.Play();
        
        if(Anim != null)
        {
            Anim.SetInteger("State", UnityEngine.Random.Range(7, 9));
            Anim.SetBool("Death", true);
        }

        controller.OnDeathEntered(data);

    }

    public override void UpdateState()
    {
        time += Time.deltaTime;

        if(time >= statComp.DeathDelayTime)
        {
            ExitState();
        }
    }

    public override void ExitState()
    {
        Destroy(gameObject);
    }

}
