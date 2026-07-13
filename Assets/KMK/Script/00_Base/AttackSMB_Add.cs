using UnityEngine;

public class AttackSMB_Add : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<EnemyAttackState>().IsAttack = true;
    }
}
