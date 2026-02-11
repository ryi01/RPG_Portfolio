using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : EnemyAttackState
{
    public Vector3 DashDir { get; set; }
    private EnemySkillAttack currentSkill;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        currentSkill = (controller as BossController).CurrentSkill;
        if (currentSkill is BossDashSkillAttack dashAttack)
        {
            controller.StatComp.SetSpeedMultifle(4);
            navMeshAgent.speed = fsmInfo.MoveSpeed;

            Vector3 targetPos = transform.position + (DashDir * 10f);
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(targetPos);
        }
        
    }
    public override void UpdateState()
    {
        if (IsAttack)
        {
            return;
        }
        controller.StatComp.SetSpeedMultifle(1);
        navMeshAgent.speed = fsmInfo.MoveSpeed;
        LookAtTarget();
        controller.TransactionToState(EnumTypes.STATE.IDLE);
    }


}
