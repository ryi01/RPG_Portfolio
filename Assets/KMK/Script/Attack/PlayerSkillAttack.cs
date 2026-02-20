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
    // 스킬 타이머(이미지) 컴포넌트 참조
    [SerializeField] private Sprite skillBtnSprite;
    [SerializeField] private SkillTimer skillTimer;
    public override void Attack()
    {
        RangeAngleTargetAttack(skillInfo);
    }
    public void StartSkill()
    {
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
            if (target.GetStat().CurrentHP <= 0) return;
            target?.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce, transform);
        }
    }

    public void SetSkillIcon()
    {
        skillTimer.SetSkillIcon(skillBtnSprite);
    }
}
