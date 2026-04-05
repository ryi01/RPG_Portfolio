using UnityEngine;

public class ChargeSkill : PlayerSkillAttack
{

    public void OnChargeAttack()
    {
        isHitOnce = false;
        var gm = GameManager.Instance;
        PlaySwingSFX();
        gm.CameraShakeController.PlayMotionBlur(0.65f, skillInfo.attackTime);

        gm.CameraShakeController.ShakeCam(hitShake.x, hitShake.y);
        Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        if(!isHitOnce)
        {
            var gm = GameManager.Instance;
            gm.CombatFeedback.HitStopThenSlow(stopTime, 0.03f, impactScaleAndDuration.x, impactScaleAndDuration.y);
            gm.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
            gm.CameraShakeController.Zoom(zoomSizeAndDuration.x, zoomSizeAndDuration.y, 0.05f);
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
