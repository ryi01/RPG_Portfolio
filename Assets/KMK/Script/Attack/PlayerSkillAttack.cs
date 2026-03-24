using System.Collections;
using UnityEngine;

public class PlayerSkillAttack : PlayerMeleeAttack
{
    [SerializeField] protected SkillInfo skillInfo;
    private bool isSkill;
    public bool IsSkill { get => isSkill; set => isSkill = value; }
    public float AttackTime { get => skillInfo.attackTime; }
    public float CoolTime { get => skillInfo.coolTime; }
    public string skillHashName { get => skillInfo.animTrigger; }
    public int UnLockLevel { get => skillInfo.openLevel; }
    public bool IsUnlocked { get; set; } = false;

    // 스킬 타이머(이미지) 컴포넌트 참조
    [SerializeField] private Sprite skillBtnSprite;
    [SerializeField] private SkillTimer skillTimer;

    protected bool isHitOnce = false;

    public override void Attack()
    {
        base.Attack();
        RangeAngleTargetAttack(skillInfo);
    }
    public void StartSkill()
    {
        if (!IsUnlocked)
        {
            return;
        }

        IsSkill = true;
        skillTimer.StartTimer(this, CoolTime);
    }
    public void EndSkill()
    {
        IsSkill = false;
    }
    protected override void AttackHit(Collider hit)
    {
        var target = hit.GetComponent<BaseController>();
        if (target != null)
        {
            if (target.GetStat.CurrentHP <= 0) return;
            target?.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce, transform);
        }
    }
    public void UnLockSkill()
    {
        IsUnlocked = true;
        skillTimer.DeleteMaskImage();
    }
    public void SetSkillIcon()
    {
        skillTimer.SetSkillIcon(skillBtnSprite);
    }
}
