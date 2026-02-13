using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : EnemyAttackState
{
    public Vector3 DashDir { get; set; }
    private EnemySkillAttack currentSkill;
    private bool isBeforeDashAttack = false;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (data != null) currentSkill = (EnemySkillAttack)data;

        if (currentSkill != null)
        {
            if (!isBeforeDashAttack)
            {
                LookAtTarget();
            }
            if (currentSkill.SkillIndex == 2)
            {
                isBeforeDashAttack = true;
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
        if (isBeforeDashAttack && currentSkill.SkillIndex == 1) isBeforeDashAttack = false;
        StopAllCoroutines();
        base.ExitState();
        navMeshAgent.speed = controller.StatComp.SetSpeedMultifle(1);
        navMeshAgent.acceleration = 8f;
        if (!isBeforeDashAttack)
        {
            LookAtTarget();
        }
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
        
        navMeshAgent.speed = controller.StatComp.SetSpeedMultifle(4);
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
        BossController boss = controller as BossController;
        controller.TransactionToState(EnumTypes.STATE.ATTACK, boss.SkillList[1]);
    }    
}
