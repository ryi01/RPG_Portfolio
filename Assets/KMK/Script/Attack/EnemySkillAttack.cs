using UnityEngine;

public class EnemySkillAttack : EnemyMeleeAttack
{
    [SerializeField] protected SkillInfo skillInfo;

    private bool isSkill;
    public bool IsSkill { get => isSkill; set => isSkill = value; }
    public bool IsReady => Time.time - lastUseTime >= skillInfo.coolTime;
    private float lastUseTime = -100f;
    public float WaitSkillTime { get => skillInfo.coolTime; }
    public float AttackRange { get => skillInfo.attackRange; }
    public override void Attack()
    {
        if (IsSkill) return;
        RangeAngleTargetAttack(skillInfo);
    }

    public void SetLastTime()
    {
        lastUseTime = Time.time;
    }
    protected override void AttackHit(Collider hit)
    {
        hit.GetComponent<BaseController>()?.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce);
    }
}
