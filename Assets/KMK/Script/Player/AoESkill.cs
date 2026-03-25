using System;
using System.Collections;
using UnityEngine;

public class AoESkill : PlayerSkillAttack
{
    [SerializeField] protected Transform skillEffectPrefabTrans;
    [SerializeField] protected GameObject groundEffect;
    private GameObject effect;

    public void OnAoEeffect()
    {
        if (groundEffect != null) groundEffect.SetActive(true);
    }
    public void OnAoESkill()
    {
        pc.CameraShakeController.PlayMotionBlur(0.35f, 0.1f);

        pc.CameraShakeController.ShakeCam(attackShake.x, attackShake.y);
        pc.CameraShakeController.Zoom(zoomSizeAndDuration.x, zoomSizeAndDuration.y, 0.08f);
        pc.CombatFeedback.HitStopThenSlow(stopTime, 0.03f, impactScaleAndDuration.x, impactScaleAndDuration.y);
        
        Attack();
    }
    public void OnAoESkillEnd()
    {
        groundEffect.SetActive(false);
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
    }
}
