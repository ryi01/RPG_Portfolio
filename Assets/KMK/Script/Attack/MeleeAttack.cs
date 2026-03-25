using UnityEngine;

public class MeleeAttack : CommonAttack
{
    protected CharacterStatComponent CS { get => bc.GetStat; }
    protected float Force { get => CS.NockbackForce; }
    public virtual float CurrentRadius => CS.AttackRadius;
    [SerializeField] protected ParticleSystem hitEffectPrefab;

    public override void Attack()
    {
        RangeAngleTargetAttack();
    }
    public virtual void PlayEffect()
    {
        if(hitEffectPrefab != null)
        {
            hitEffectPrefab.Play();
        }
    }

    public virtual void StopPlayEffect()
    {
        if (hitEffectPrefab != null)
        {
            hitEffectPrefab.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
    public virtual void RangeAngleTargetAttack(SkillInfo data = null)
    {
        float radius = CurrentRadius;
        if (data != null && radius == CS.AttackRadius)
        {
            radius = data.attackRadius * data.attackMultifle;
        }
        Collider[] hits = Physics.OverlapSphere(attackTransform.position, radius, CS.TargetLayer);
        bool isValidTarget = false;
        
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<CharacterStatComponent>(out CharacterStatComponent stat) && stat.CurrentHP <= 0) continue;
            if (hit == null) continue;
            Vector3 dir = hit.transform.position - transform.position;
            dir = new Vector3(dir.x, transform.position.y, dir.z).normalized;

            float angle = Vector3.Angle(transform.forward, dir);
            float targetAngle = (data != null) ? data.hitAngle : CS.HitAngle;
            if (angle < targetAngle)
            {
                if (!isValidTarget)
                {
                    AttackReady();
                    isValidTarget = true;
                }
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
