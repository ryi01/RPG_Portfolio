using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossTakeDownSkill : EnemySkillAttack
{
    protected override void Awake()
    {
        base.Awake();
        hitEffectPrefab.Stop();
    }
    private void OnEnable()
    {
        AttackRaidusMult = 1;
        owner.BossPhase.OnPhaseTwoStarted += EnterBossPhase;
    }
    private void OnDisable()
    {
        owner.BossPhase.OnPhaseTwoStarted -= EnterBossPhase;
    }

    private void EnterBossPhase()
    {
        AttackRaidusMult = 1.5f;
    }

    public void OnTakeDown()
    {
        PlayEffect();
        Attack();
    }

}
