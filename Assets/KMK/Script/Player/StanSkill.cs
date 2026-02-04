using UnityEngine;

public class StanSkill : PlayerSkillAttack
{
    public void OnStanSkill()
    {
        Attack();
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
        Debug.Log($"스턴 넣을꺼야");
    }
}
