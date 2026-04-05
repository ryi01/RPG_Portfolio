using System;
using System.Collections;
using UnityEngine;

public class AoESkill : PlayerSkillAttack
{
    [SerializeField] protected Transform skillEffectPrefabTrans;
    [SerializeField] protected ParticleSystem groundEffect;

    public void OnAoEeffect()
    {
        groundEffect.Play();
    }
    public void OnAoESkill()
    {
        var gm = GameManager.Instance;
        gm.CameraShakeController.PlayMotionBlur(0.35f, 0.1f);

        gm.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
        gm.CameraShakeController.Zoom(zoomSizeAndDuration.x, zoomSizeAndDuration.y, 0.08f);
        gm.CombatFeedback.HitStopThenSlow(stopTime, 0.03f, impactScaleAndDuration.x, impactScaleAndDuration.y);
        
        Attack();
    }
    public void OnAoESkillEnd()
    {
        groundEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        StopPlayEffect();
    }
    public override void RangeAngleTargetAttack(SkillInfo data = null)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, skillInfo.attackRadius, CS.TargetLayer);
        if (hits.Length > 0)
        {
            base.AttackReady();
            Array.Sort(hits, (a, b) =>
            {
                float distA = Vector3.SqrMagnitude(a.transform.position - transform.position);
                float distB = Vector3.SqrMagnitude(b.transform.position - transform.position);
                return distA.CompareTo(distB);
            });
            
            StartCoroutine(DomainRoutine(hits));
        }
    }

    IEnumerator DomainRoutine(Collider[] targets)
    {
        foreach(Collider target in targets)
        {
            if (target == null) continue;
            yield return new WaitForSecondsRealtime(0.03f);
            AttackHit(target);
        }
    }
    
    public void MakeEffect()
    {
        PlayEffect();
        PlayImpactSFX();
    }
}
