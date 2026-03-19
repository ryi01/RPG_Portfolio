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
        pc.AttackComp.ResetCombo();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    private int lastEventFrame = -1;
    public void SetComboIndex(int n)
    {
        if (lastEventFrame == Time.frameCount) return;
        lastEventFrame = Time.frameCount;
        comboIndex = n;
      
        PlayComboAttackSFX(comboIndex);
    }

    private void PlayComboAttackSFX(int comboIndex)
    {
        /*        switch (comboIndex)
              {
                  case 0:
                      GameManager.Instance.SoundManager.PlayCombatSFX("NormalAttack1", 0.75f);
                      break;
                  case 1:
                      GameManager.Instance.SoundManager.PlayCombatSFX("NormalAttack2", 0.75f);
                      break;
                  case 2:
                      GameManager.Instance.SoundManager.PlayCombatSFX("NormalAttack3", 0.75f);
                      break;
                  case 3:
                      GameManager.Instance.SoundManager.PlayCombatSFX("NormalAttack4", 0.75f);
                      break;
              }*/
    }
}
