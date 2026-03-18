using System;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class EnemyDeathState : EnemyHitState
{
    protected float time = 0;
    
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (controller.NavMeshAgent != null) controller.NavMeshAgent.enabled = false;
        
        Anim.SetInteger("State", UnityEngine.Random.Range(7, 9));
        Anim.SetBool("Death", true);
    }

    public override void UpdateState()
    {
        time += Time.deltaTime;

        if(time >= fsmInfo.DeathDelayTime)
        {
            ExitState();
        }
    }

    public override void ExitState()
    {
        if(TryGetComponent<BossController>(out BossController boss))
        {
            boss.OnDeath();
        }
        Destroy(gameObject);
    }

}
