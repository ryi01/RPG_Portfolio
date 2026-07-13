using UnityEngine;

public class EnemyMeleeAttack : MeleeAttack
{
    public void OnEnemyAttack()
    {
        Attack();
        PlaySwingSFX();
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);

        if(TryGetComponent<BaseController>(out BaseController target))
        {
            target.Damage(CS.FinalAttack, CS.NockbackForce, transform);
            PlayImpactSFX();
        }
    }
}
