using UnityEngine;

public class AttackSMB : AttackSMB_Add
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<EnemyAttackState>().IsAttack = false;
    }
}
