using System.Collections;
using UnityEngine;

public class PlayerSkillAttack : PlayerMeleeAttack
{
    [SerializeField] protected SkillInfo skillInfo;
    private bool isSkill;
    public bool IsSkill { get => isSkill; set => isSkill = value; }
    public float WaitSkillTime { get => skillInfo.coolTime; }
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
        skillTimer.StartTimer(this, WaitSkillTime);
    }
    public void EndSkill()
    {
        IsSkill = false;
    }
    protected override void AttackHit(Collider hit)
    {
        hit.GetComponent<BaseController>()?.Damage(CS.FinalAttack * skillInfo.attackMultifle, skillInfo.nockbackForce);
    }

    public void SetSkillIcon()
    {
        skillTimer.SetSkillIcon(skillBtnSprite);
    }
}
