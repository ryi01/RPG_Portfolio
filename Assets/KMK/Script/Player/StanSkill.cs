using UnityEngine;

public class StanSkill : PlayerSkillAttack
{
    [SerializeField] private Transform stunEffectTrans;
    public void OnStanSkill()
    {
        Instantiate(hitEffectPrefab, stunEffectTrans.position, hitEffectPrefab.transform.rotation);
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
