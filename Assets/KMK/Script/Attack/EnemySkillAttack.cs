using UnityEngine;

public class EnemySkillAttack : EnemyMeleeAttack
{
    [SerializeField] protected SkillInfo skillInfo;

    private float currentRadiusMult = 1.0f;
    private bool isSkill;
    public bool IsSkill { get => isSkill; set => isSkill = value; }
    public bool IsReady => Time.time - lastUseTime >= skillInfo.coolTime;
    private float lastUseTime = -100f;
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
        if (IsSkill) return;
        RangeAngleTargetAttack(skillInfo);
    }

    public void SetLastTime()
    {
        lastUseTime = Time.time;
    }
    protected override void AttackHit(Collider hit)
    {
        Debug.Log($"{CS.FinalAttack}");
        hit.GetComponent<BaseController>()?.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce, transform);
    }
}
