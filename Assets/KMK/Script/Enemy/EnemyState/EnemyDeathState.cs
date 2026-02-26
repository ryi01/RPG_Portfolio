using UnityEditor.AnimatedValues;
using UnityEngine;

public class EnemyDeathState : EnemyHitState
{
    protected float time = 0;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (navMeshAgent != null) navMeshAgent.enabled = false;
        
        Anim.SetInteger("State", Random.Range(7, 9));
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
        Destroy(gameObject);
    }
}
