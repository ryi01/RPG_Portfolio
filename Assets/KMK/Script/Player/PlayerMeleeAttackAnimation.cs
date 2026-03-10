using UnityEngine;

// 기본 공격
public class PlayerMeleeAttackAnimation : PlayerMeleeAttack
{
    protected override void AttackReady()
    {
        base.AttackReady();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
    }
    public void OnPlayerMeleeAttack()
    {
        Attack();
    }

    public void OnPlayerAttackEnd()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }
}
