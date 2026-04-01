using UnityEngine;

public class ChargeSkill : PlayerSkillAttack
{
    public void OnChargeAttack()
    {
        isHitOnce = false;
        pc.CameraShakeController.PlayMotionBlur(0.65f, skillInfo.attackTime);
        
        pc.CameraShakeController.ShakeCam(hitShake.x, hitShake.y);
        Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        if(!isHitOnce)
        {
            pc.CombatFeedback.HitStopThenSlow(stopTime, 0.03f, impactScaleAndDuration.x, impactScaleAndDuration.y);
            pc.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
            pc.CameraShakeController.Zoom(zoomSizeAndDuration.x, zoomSizeAndDuration.y, 0.05f);
            isHitOnce = true;
        }
        
        base.AttackHit(hit);
    }
    public void OnPlayEffect()
    {
        PlayEffect();
    }
    public void OnStopEffect()
    {
        StopPlayEffect();
    }
}
