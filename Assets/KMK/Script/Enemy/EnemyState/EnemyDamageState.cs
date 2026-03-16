using System.Collections;
using UnityEngine;

public class EnemyDamageState : EnemyHitState
{
    [SerializeField] protected ParticleSystem hitParticle;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if(hitParticle != null)
        {
            // 파티클 
            hitParticle.Play();
        }
        Anim.SetInteger("State", (int)state);
        base.EnterState(state, data);
    }

    public override void UpdateState()
    {
        if (fsmInfo.IsHit || IsPlayingHit()) return;
        if(controller.GetPlayerDis() <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
        }
        else controller.TransactionToState(EnumTypes.STATE.DETECT);
    }
    private bool IsPlayingHit()
    {
        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(0);
        // 상태 이름이나 태그로 피격 애니메이션 확인
        return stateInfo.IsTag("Hit");
    }
}
