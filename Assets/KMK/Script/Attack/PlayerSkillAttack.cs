using System.Collections;
using UnityEngine;

public class PlayerSkillAttack : PlayerMeleeAttack
{
    [SerializeField] protected SkillInfo skillInfo;
    private bool isSkill;
    public bool IsSkill { get => isSkill; set => isSkill = value; }
    public float WaitSkillTime { get => skillInfo.attackTime; }
    public string skillHashName { get => skillInfo.animTrigger; }
    public override void Attack()
    {
        RangeAngleTargetAttack(skillInfo);
    }
    public void StartSkill()
    {
        IsSkill = true;
    }
    public void EndSkill()
    {
        IsSkill = false;
    }
    protected override void AttackHit(Collider hit)
    {
        bc.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce);
    }
}
