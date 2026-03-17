using UnityEngine;

public class RotationSkill : PlayerSkillAttack
{
    public void OnPlayerRotAttack()
    {
        pc.CameraShakeController.ShakeCam(hitShake.x, hitShake.y);
        Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        pc.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
        pc.CombatFeedback.ImpactSlow(impactScaleAndDuration.x, impactScaleAndDuration.y);
        pc.CombatFeedback.HitStop(stopTime);
        base.AttackHit(hit);
    }
}
