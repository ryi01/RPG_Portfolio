using UnityEngine;

public class BossCrossCutSkill : EnemySkillAttack
{
    private void Start()
    {
        if (TryGetComponent<TrailRenderer>(out TrailRenderer trail))
        {
            trail.emitting = false;
        }
    }
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
