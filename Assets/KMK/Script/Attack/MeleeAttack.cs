using UnityEngine;

public class MeleeAttack : CommonAttack
{
    protected CharacterStatComponent CS { get => bc.StatComp; }
    protected float Force { get => bc.StatComp.NockbackForce; }
    public override void Attack()
    {
        RangeAngleTargetAttack();
    }

    public virtual void RangeAngleTargetAttack(SkillInfo data = null)
    {
        float radius = (data != null) ? data.attackRadius : CS.AttackRadius;
        if(data == null)
        {
            Debug.Log($"data ¾øÀ½");
        }
        Collider[] hits = Physics.OverlapSphere(attackTransform.position, radius, CS.TargetLayer);
        if(hits.Length > 0)
        {
            AttackReady();
        }
        foreach(Collider hit in hits)
        {
            Vector3 dir = hit.transform.position - transform.position;
            dir = new Vector3(dir.x, transform.position.y, dir.z).normalized;

            float angle = Vector3.Angle(transform.forward, dir);
            float targetAngle = (data != null) ? data.hitAngle : CS.HitAngle;
            if(angle < targetAngle)
            {
                AttackHit(hit);
            }
        }
    }

    protected virtual void AttackReady()
    {

    }   
    protected virtual void AttackHit(Collider hit)
    {

    }
}
