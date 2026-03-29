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
    private void Start()
    {
        if (TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            trail.emitting = false;
        }
    }

    private void EnterBossPhase()
    {
        AttackRaidusMult = 1.8f;
    }

    public void OnCrossCut()
    {
        Attack();
    }

    public void TrailOn()
    {
        if(TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            trail.emitting = true;  
        }
    }

    public void TrailOff()
    {
        if (TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            trail.emitting = false;
        }
    }
}
