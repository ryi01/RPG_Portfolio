using System.Collections;
using UnityEngine;

public class EnemyDamageState : EnemyHitState
{
    [SerializeField] protected ParticleSystem hitParticle;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if(hitParticle != null)
        {
            // ÆÄÆ¼Å¬ 
            hitParticle.Play();
        }
        Anim.SetInteger("State", (int)state);
        base.EnterState(state, data);
    }

    public override void UpdateState()
    {
        if (fsmInfo.IsHit) return;
        if(controller.GetPlayerDis() <= fsmInfo.AttackRadius)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        controller.TransactionToState(EnumTypes.STATE.DETECT);
    }


}
