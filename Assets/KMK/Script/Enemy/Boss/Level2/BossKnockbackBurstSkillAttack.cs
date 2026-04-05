using UnityEngine;

public class BossKnockbackBurstSkillAttack : EnemySkillAttack
{
    public override void Attack()
    {
        RangeAngleTargetAttack(skillInfo);
    }
    public void OnBurstEffect()
    {
        PlayEffect();
    }
    public void OnBurstEffectOff()
    {
        StopPlayEffect();
    }

    public void OnBurst()
    {
        Attack();
    }
}
