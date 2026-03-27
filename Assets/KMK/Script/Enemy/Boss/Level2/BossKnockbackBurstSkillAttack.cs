using UnityEngine;

public class BossKnockbackBurstSkillAttack : EnemySkillAttack
{
    public override void Attack()
    {
        RangeAngleTargetAttack(skillInfo);
    }

    public void OnBurst()
    {
        PlayEffect();
        Attack();
    }
}
