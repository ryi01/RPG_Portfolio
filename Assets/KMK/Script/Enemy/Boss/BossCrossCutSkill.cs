using UnityEngine;

public class BossCrossCutSkill : EnemySkillAttack
{
    protected override void AttackReady()
    {
        base.AttackReady();
        if (boss != null && boss.IsPhaseTwo)
        {
            AttackRaidusMult = 1.8f;
        }
    }
    public void OnCrossCut()
    {
        Attack();
    }
}
