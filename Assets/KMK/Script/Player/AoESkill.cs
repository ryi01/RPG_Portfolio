using System;
using System.Collections;
using UnityEngine;

public class AoESkill : PlayerSkillAttack
{
    public void OnAoESkill()
    {
        Attack();
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
            yield return new WaitForSeconds(0.03f);
            AttackHit(target);
        }
    }
}
