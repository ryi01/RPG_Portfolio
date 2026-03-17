using UnityEngine;

public class ChargeSkill : PlayerSkillAttack
{

    public void OnChargeAttack()
    {
        isHitOnce = false;
        pc.CameraShakeController.ShakeCam(hitShake.x, hitShake.y);
        Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        if(!isHitOnce)
        {
            pc.CombatFeedback.HitStop(stopTime);
            pc.CombatFeedback.ImpactSlow(impactScaleAndDuration.x, impactScaleAndDuration.y);
            pc.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
            pc.CameraShakeController.Zoom(zoomSizeAndDuration.x, zoomSizeAndDuration.y, 0.05f);
            isHitOnce = true;
        }
        base.AttackHit(hit);
    }

}
