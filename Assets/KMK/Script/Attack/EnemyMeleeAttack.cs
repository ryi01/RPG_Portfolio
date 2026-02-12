using UnityEngine;

public class EnemyMeleeAttack : MeleeAttack
{
    public void OnEnemyAttack()
    {
        Attack();
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);

        hit.GetComponent<BaseController>()?.Damage(CS.FinalAttack, CS.NockbackForce, transform);
    }
}
