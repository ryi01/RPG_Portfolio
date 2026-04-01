using UnityEngine;

public class RotationSkill : PlayerSkillAttack
{
    private bool isImpactPlay = false;
    public void OnPlayEffect2()
    {
        PlayEffect();
    }
    public void OnStopEffect2()
    {
        StopPlayEffect();
    }
    public void OnPlayerRotAttack()
    {
        isImpactPlay = false;
        pc.CameraShakeController.PlayMotionBlur(0.45f, 0.12f);
        pc.CameraShakeController.ShakeCam(hitShake.x, hitShake.y);
        Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        if (!isImpactPlay)
        {
            pc.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
            pc.CombatFeedback.HitStopThenSlow(stopTime, 0.03f, impactScaleAndDuration.x, impactScaleAndDuration.y);
            isImpactPlay = true;
        }
        base.AttackHit(hit);
    }
}
