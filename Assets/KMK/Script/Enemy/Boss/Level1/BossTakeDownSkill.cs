using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossTakeDownSkill : EnemySkillAttack
{
    protected override void Awake()
    {
        base.Awake();
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
    public void OnEffect()
    {
        PlayEffect();
        if (cameraEffect == null) return;
        cameraEffect.PlayHeavyHit();
        PlaySwingSFX();
        GameManager.Instance.CombatFeedback.HitStopByStrength(CombatFeedback.HitStrength.Heavy);
    }
    public void OnEffectOff()
    {
        StopPlayEffect();
    }
    public void OnTakeDown()
    {
        Attack();
    }

}
