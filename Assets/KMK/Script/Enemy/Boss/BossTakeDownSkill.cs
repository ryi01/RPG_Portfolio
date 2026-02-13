using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossTakeDownSkill : EnemySkillAttack
{
    protected override void AttackReady()
    {
        base.AttackReady();
        if (boss != null && boss.IsPhaseTwo)
        {
            AttackRaidusMult = 1.5f;
        }
    }
    public void OnTakeDown()
    {
        Attack();
    }
    
}
