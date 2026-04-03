using System;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    protected float time = 0;
    [SerializeField] protected GameObject deathParticle;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);

        time = 0;
        statComp.IsHit = false;
        controller.NavigationStop();
        if (controller.NavMeshAgent != null && controller.NavMeshAgent.enabled) controller.NavMeshAgent.enabled = false;

        if(Anim != null)
        {
            Anim.SetInteger("State", UnityEngine.Random.Range(7, 9));
            Anim.SetBool("Death", true);
        }
        if(!controller.StatComp.IsBoss)
        {
            CreateEffect();
        }
       
        controller.OnDeathEntered(data);

    }
    public void OnDeathEffect()
    {
        CreateEffect();
    }
    private void CreateEffect()
    {
        Vector3 pos = new Vector3(transform.position.x, 1.8f, transform.position.z);
        GameObject vfx = Instantiate(deathParticle, pos, Quaternion.identity);
        Destroy(vfx, 2);
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
