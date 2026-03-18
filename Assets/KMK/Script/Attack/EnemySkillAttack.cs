using UnityEngine;

public class EnemySkillAttack : EnemyMeleeAttack
{
    [SerializeField] protected SkillInfo skillInfo;

    private float currentRadiusMult = 1.0f;

    public bool IsReady => Time.time - lastUseTime >= skillInfo.coolTime;
    private float lastUseTime = -100f;
    public bool NeedLookAtTarget { get => skillInfo.needLookAtTarget; }
    public bool NeedDash { get => skillInfo.needDash; }
    public int NextSkillIndex { get => skillInfo.nextSkillIndex; }
    public bool ChainNextSkill { get => skillInfo.chainNextSkill; }   
    public float WaitSkillTime { get => skillInfo.coolTime; }
    public float AttackMinRange { get => skillInfo.attackMinRange; }
    public float AttackMaxRange { get => skillInfo.attackMaxRange; }
    public int SkillIndex { get => int.Parse(skillInfo.animTrigger); }
    public float AttackRaidusMult { set => currentRadiusMult = value; }
    public float AttackRadius { get => skillInfo.attackRadius; }
    public override float CurrentRadius => skillInfo.attackRadius * currentRadiusMult;

    protected BossController boss;
    protected override void AttackReady()
    {
        base.AttackReady();
        if (TryGetComponent<BossController>(out boss))
        {
            if (boss.IsPhaseTwo)
            {
                CS.attackBuffMultifle = 2;
            }
        }
    }
    public override void Attack()
    {
        RangeAngleTargetAttack(skillInfo);
    }

    public void SetLastTime()
    {
        lastUseTime = Time.time;
    }
    protected override void AttackHit(Collider hit)
    {
        var target = hit.GetComponent<BaseController>();
        if (target != null)
        {
            if (target.GetStat().CurrentHP <= 0) return;
            target?.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce, transform);
        }
    }
}
