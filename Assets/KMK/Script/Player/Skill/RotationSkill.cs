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

    public void OnPlaySFX()
    {
        PlaySwingSFX();
    }
    public void OnPlayerRotAttack()
    {
        isImpactPlay = false;
        var gm = GameManager.Instance;
        gm.CameraShakeController.PlayMotionBlur(0.45f, 0.12f);
        gm.CameraShakeController.ShakeCam(hitShake.x, hitShake.y);
        Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        if (!isImpactPlay)
        {
            var gm = GameManager.Instance;
            gm.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
            gm.CombatFeedback.HitStopThenSlow(stopTime, 0.03f, impactScaleAndDuration.x, impactScaleAndDuration.y);
            isImpactPlay = true;
        }
        base.AttackHit(hit);
    }
}
