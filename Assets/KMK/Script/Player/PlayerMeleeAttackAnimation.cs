using System.Collections.Generic;
using UnityEngine;

// 기본 공격
public class PlayerMeleeAttackAnimation : PlayerMeleeAttack
{
    [SerializeField] private List<AudioClip> comboSFX;
    [SerializeField][Range(0, 1)] private float volume = 0.75f;
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
        pc.AttackComp.ResetCombo();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    private int lastEventFrame = -1;
    public void SetComboIndex(int n)
    {
        if (lastEventFrame == Time.frameCount) return;
        lastEventFrame = Time.frameCount;
        comboIndex = n;
    }

    public void PlayComboAttackSFX()
    {
        GameManager.Instance.SoundManager.PlayCombatSFX(comboSFX[comboIndex], volume);
    }
}
