using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 문제점 : 공격 선택과 공격을 동시에 진행해 문제가 발생
// => Attack을 선택하는 부분과 공격부분을 분리했어야함 
// => Dash기능이 합쳐져 있어서 하드 코딩됨
public class BossAttackState : EnemyAttackState
{
    public Vector3 DashDir { get; set; }
    private EnemySkillAttack currentSkill;
    private bool isBeforeDashAttack = false;
    private Coroutine dashCoroutine;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (data != null) currentSkill = (EnemySkillAttack)data;
        if (currentSkill == null) return;
        if (!isBeforeDashAttack)
        {
            LookAtTarget();
        }

        if (!isBeforeDashAttack)
        {
            LookAtTarget();
        }
        if (currentSkill.SkillIndex == 2)
        {
            isBeforeDashAttack = true;
            if (dashCoroutine != null) StopCoroutine(dashCoroutine);
            dashCoroutine = StartCoroutine(WaitDash());
        }
        else
        {
            isBeforeDashAttack = false;

            Anim.SetInteger("State", 3);
            Anim.SetInteger("Skill", currentSkill.SkillIndex);
        }
    }
    public override void UpdateState()
    {

        if (currentSkill == null) return;

        if (!IsAttack && currentSkill.SkillIndex != 2)
        {
            controller.TransactionToState(EnumTypes.STATE.IDLE);
            return;
        }

    }
    public override void ExitState()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }

        base.ExitState();

        navMeshAgent.speed = controller.StatComp.SetSpeedMultifle(1);
        navMeshAgent.acceleration = 8f;

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
