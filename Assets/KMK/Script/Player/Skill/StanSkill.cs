using UnityEngine;

public class StanSkill : PlayerSkillAttack
{
    [SerializeField] private Transform stunEffectTrans;
    protected override void Awake()
    {
        base.Awake();
        isAttackShake = false;
        isHitStop = false;
        isHitShake = false;
    }
    public void OnStanSkill()
    {
        var gm = GameManager.Instance;
        gm.CameraShakeController.GenerateImpulseDirection(pc.LockedAimDir, 1.6f);
        gm.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
        gm.CameraShakeController.Zoom(zoomSizeAndDuration.x, zoomSizeAndDuration.y, 0.06f);
        gm.CombatFeedback.HitStopThenSlow(stopTime, 0.03f, impactScaleAndDuration.x, impactScaleAndDuration.y);
        PlaySwingSFX();
        Attack();
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);

        if(hit.TryGetComponent(out EnemyController enemy))
        {
            PlayImpactSFX();
            enemy.ForceStun(2);
        }
    }
    public void OnStunEffect()
    {
        PlayEffect();
    }
    public void OnStopStunEffect()
    {
        StopPlayEffect();
    }
}
