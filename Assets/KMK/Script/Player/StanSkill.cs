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
        if(hit.TryGetComponent(out EnemyController enemy))
        {
            enemy.ForceStun(2);
        }
    }
}
