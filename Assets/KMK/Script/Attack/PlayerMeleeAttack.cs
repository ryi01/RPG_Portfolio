using System;
using UnityEngine;

public class PlayerMeleeAttack : MeleeAttack
{
    protected PlayerController pc => bc as PlayerController;
    protected bool isHitShakeSwing = false;
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
        isHitShakeSwing = false;
        if(isAttackShake)
        {
            float multifle = 1f + comboIndex * 0.2f;
            pc.CameraShakeController.ShakeCam(attackShake.x * multifle, attackShake.y);
        }
        base.Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
            
        if (!isHitShakeSwing)
        {
            float multifle = 1f + comboIndex * 0.25f;
            if(isHitShake)pc.CameraShakeController.ShakeCam(hitShake.x * multifle, hitShake.y);
            if (isHitStop) pc.CombatFeedback.HitStopByStrength(GetComboHitStrenght());
            isHitShakeSwing = true;
        }
        if (hit.TryGetComponent<BaseController>(out var target))
        {
            target.Damage(CS.FinalAttack, CS.NockbackForce, transform);
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
