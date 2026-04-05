using System;
using UnityEngine;

public class PlayerMeleeAttack : MeleeAttack
{
    protected PlayerController pc => bc as PlayerController;
    protected int comboIndex = 0;
    [SerializeField] protected Vector2 hitShake;
    [SerializeField] protected Vector2 attackShake;
    [SerializeField] protected Vector2 impactScaleAndDuration;
    [SerializeField] protected Vector2 zoomSizeAndDuration;
    [SerializeField] protected float stopTime;
    [SerializeField] protected bool isAttackShake = true;
    [SerializeField] protected bool isHitShake = true;
    [SerializeField] protected bool isHitStop = true;
    public override void Attack()
    {

        if (isAttackShake)
        {
            float multifle = 1f + comboIndex * 0.2f;
            GameManager.Instance.CameraShakeController.ShakeCam(attackShake.x * multifle, attackShake.y);
        }
        base.Attack();
    }
    protected override void AttackReady()
    {
        float multifle = 1f + comboIndex * 0.25f;

        // "맞았을 때" 1회만 실행되는 연출
        if (isHitShake)
            GameManager.Instance.CameraShakeController.ShakeCam(hitShake.x * multifle, hitShake.y);

        if (isHitStop)
            GameManager.Instance.CombatFeedback.HitStopByStrength(GetComboHitStrenght());
        
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
          
        if (hit.TryGetComponent<BaseController>(out var target))
        {
            target.Damage(CS.FinalAttack, CS.NockbackForce, transform);

            PlayImpactSFX();
        }
    }
    protected CombatFeedback.HitStrength GetComboHitStrenght()
    {
        switch(comboIndex)
        {
            case 0:
            case 1:
                return CombatFeedback.HitStrength.Light;
            case 2:
                return CombatFeedback.HitStrength.Medium;
            default:
                return CombatFeedback.HitStrength.Heavy;
        }
    }

}
