using UnityEngine;

public class BossSummonSkillAttack : EnemySkillAttack
{
    [SerializeField] private bool summonAroundBoss = true;
    [SerializeField] private float summonForwardOffset = 2f;

    public override void Attack()
    {
        if (owner == null || owner.BossSummon == null) return;
        if (!owner.BossSummon.CanSummon()) return;
        Vector3 center = summonAroundBoss ? owner.transform.position : owner.transform.position + owner.transform.forward * summonForwardOffset;

        owner.BossSummon.SummonAround(center);
    }

    public void OnEffectPlay()
    {
        PlayEffect();
        if (cameraEffect == null) return;
        cameraEffect.PlaySpawn();
    }

    public void OnSummon()
    {
        Attack();
        PlaySwingSFX();
    }

    public void OnEffectOff()
    {
        StopPlayEffect();
    }
}
