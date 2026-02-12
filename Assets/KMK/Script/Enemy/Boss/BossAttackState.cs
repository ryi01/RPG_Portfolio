using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : EnemyAttackState
{
    public Vector3 DashDir { get; set; }
    private EnemySkillAttack currentSkill;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (data != null) currentSkill = (EnemySkillAttack)data;

        if (currentSkill != null)
        {
            LookAtTarget();
            
            if (currentSkill.SkillIndex == 2)
            {
                StartCoroutine(WaitDash());
            }
            else
            {
                Anim.SetInteger("State", 3);
                Anim.SetInteger("Skill", currentSkill.SkillIndex);
            }
        }
    }
    public override void UpdateState()
    {

        if (!IsAttack && currentSkill.SkillIndex != 2)
        {
            controller.TransactionToState(EnumTypes.STATE.IDLE);
            return;
        }

    }
    public override void ExitState()
    {
        StopAllCoroutines();
        base.ExitState();
        controller.StatComp.SetSpeedMultifle(1);
        navMeshAgent.speed = fsmInfo.MoveSpeed;
        navMeshAgent.acceleration = 8f;
        LookAtTarget();
    }

    IEnumerator WaitDash()
    {

        Anim.SetInteger("State", 3);
        Anim.SetInteger("Skill", currentSkill.SkillIndex);
        NavigationStop();
        yield return new WaitForSeconds(2);
        // 대쉬기능
        // 애니메이션을 3으로 세팅해주고
        Anim.SetBool("Run", true);
        // 멈춘걸 푼 다음 속도 조절
        navMeshAgent.isStopped = false;
        controller.StatComp.SetSpeedMultifle(4);
        navMeshAgent.speed = fsmInfo.MoveSpeed;
        navMeshAgent.acceleration = 1000f;
        // 최종 위치 결정
        Vector3 targetPos = transform.position + (DashDir * 10f);

        navMeshAgent.SetDestination(targetPos);
        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.5f)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.5f) break;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        isAttack = false;
        Anim.SetBool("Run", false);
        controller.TransactionToState(EnumTypes.STATE.DETECT);
    }    
}
