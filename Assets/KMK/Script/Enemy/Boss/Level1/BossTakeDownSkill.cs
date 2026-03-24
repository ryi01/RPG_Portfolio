using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossTakeDownSkill : EnemySkillAttack
{
    protected override void Awake()
    {
        base.Awake();
        hitEffectPrefab.SetActive(false);
    }
    protected override void AttackReady()
    {
        base.AttackReady();
        if (owner != null && owner.BossPhase != null && owner.BossPhase.IsPhaseTwo)
        {
            AttackRaidusMult = 1.5f;
        }
        else
        {
            AttackRaidusMult = 1;
        }
    }
    public void OnTakeDown()
    {
        PlayEffect();
        Attack();
    }

}
