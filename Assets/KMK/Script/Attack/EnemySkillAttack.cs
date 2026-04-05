using UnityEngine;

public class EnemySkillAttack : EnemyMeleeAttack
{
    [SerializeField] protected SkillInfo skillInfo;
    [SerializeField] private bool lockStateDuringSkill = false;
    [SerializeField] protected BossCameraEffectController cameraEffect;
    public bool LockStateDuringSkill => lockStateDuringSkill;

    private float currentRadiusMult = 1.0f;

    public bool IsReady => Time.time - lastUseTime >= skillInfo.coolTime;
    private float lastUseTime = -100f;
    public bool NeedLookAtTarget { get => skillInfo.needLookAtTarget; }
    public bool NeedDash { get => skillInfo.needDash; }
    public float KnockBack { get => skillInfo.nockbackForce; }
    public int NextSkillIndex { get => skillInfo.nextSkillIndex; }
    public bool ChainNextSkill { get => skillInfo.chainNextSkill; }   
    public float WaitSkillTime { get => skillInfo.coolTime; }
    public float AttackMinRange { get => skillInfo.attackMinRange; }
    public float AttackMaxRange { get => skillInfo.attackMaxRange; }
    public int SkillIndex { get => int.Parse(skillInfo.animTrigger); }
    public float AttackRaidusMult { set => currentRadiusMult = value; }
    public float AttackRadius { get => skillInfo.attackRadius; }
    public override float CurrentRadius => skillInfo.attackRadius * currentRadiusMult;

    protected EnemyController owner;
    protected override void Awake()
    {
        base.Awake();
        owner = GetComponent<EnemyController>();
    }
    protected override void AttackReady()
    {
        base.AttackReady();
        if (owner != null && owner.BossPhase != null && owner.BossPhase.IsPhaseTwo)
        {
            CS.attackBuffMultifle = 2f;
        }
        else CS.attackBuffMultifle = 1f;
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
            if (target.GetStat.CurrentHP <= 0) return;
            PlayImpactSFX();
            target?.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce, transform);
        }
    }
}
