using UnityEngine;

public class BossCrossCutSkill : EnemySkillAttack
{
    private void OnEnable()
    {
        owner.BossPhase.OnPhaseTwoStarted += EnterBossPhase;
    }
    private void OnDisable()
    {
        owner.BossPhase.OnPhaseTwoStarted -= EnterBossPhase;
    }

    private void EnterBossPhase()
    {
        AttackRaidusMult = 1.8f;
    }

    public void OnCrossCut()
    {
        Attack();
    }

    public void EffectOn()
    {
        PlayEffect();
        PlaySwingSFX();
        if (cameraEffect == null) return;
        cameraEffect.PlayLightHit();
        GameManager.Instance.CombatFeedback.HitStopByStrength(CombatFeedback.HitStrength.Light);
    }

    public void EffectOff()
    {
        StopPlayEffect();
    }
}
