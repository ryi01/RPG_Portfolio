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
        pc.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
        pc.CameraShakeController.Zoom(zoomSizeAndDuration.x, zoomSizeAndDuration.y, 0.06f);
        pc.CombatFeedback.HitStop(stopTime);
        pc.CombatFeedback.ImpactSlow(impactScaleAndDuration.x, impactScaleAndDuration.y);
        Instantiate(hitEffectPrefab, stunEffectTrans.position, hitEffectPrefab.transform.rotation);
        Attack();
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
        if(hit.TryGetComponent(out EnemyController enemy))
        {
            enemy.ForceStun(2);
        }
    }
}
