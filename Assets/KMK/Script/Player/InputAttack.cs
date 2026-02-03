using UnityEngine;

public class InputAttack : MonoBehaviour
{
    private PlayerController pc;
    // 공격 애니메이션 해시값
    private int hashAttack = Animator.StringToHash("Attack");
    // StateTime 해시 (파라미터 해시)
    private int hashAttackTime = Animator.StringToHash("AttackTime");
    // Combo1 애니메이션 해시 (애니메이션 노드 해시)
    private int hashCombo1 = Animator.StringToHash("Combo1");
    // Combo2 애니메이션 해시 (애니메이션 노드 해시)
    private int hashCombo2 = Animator.StringToHash("Combo2");
    // Combo3 애니메이션 해시 (애니메이션 노드 해시)
    private int hashCombo3 = Animator.StringToHash("Combo3");
    // Combo4 애니메이션 해시 (애니메이션 노드 해시)
    private int hashCombo4 = Animator.StringToHash("Combo4");

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }
    public void ResetTrigger()
    {
        pc.Animator.ResetTrigger(hashAttack);
    }
    public void UpdateAttackProgress()
    {
        pc.Animator.SetFloat(hashAttackTime, Mathf.Repeat(pc.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
        
    }
    public void TriggerAttack()
    {
        pc.SetAnimTrigger(hashAttack);
    }

    public bool IsAttackAnimation()
    {
        AnimatorStateInfo state = pc.Animator.GetCurrentAnimatorStateInfo(0);
        if (state.shortNameHash == hashAttack || state.shortNameHash == hashCombo1 ||
               state.shortNameHash == hashCombo2 || state.shortNameHash == hashCombo3 ||
               state.shortNameHash == hashCombo4)
        {
            return true;
        }
        return false;
    }
}
