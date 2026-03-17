using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 문제점 : 공격 선택과 공격을 동시에 진행해 문제가 발생
// => Attack을 선택하는 부분과 공격부분을 분리했어야함 
// => Dash기능이 합쳐져 있어서 하드 코딩됨
// AttackPhase를 만들어 페이즈 별로 관리
public class BossAttackState : EnemyAttackState
{
    private enum AttackPhase { Select, Prepare, Execute, Recover}
    private AttackPhase phase;
    public Vector3 DashDir { get; set; }
    private EnemySkillAttack currentSkill;
    private bool isDashStart = false;
    private bool isChaniedSkill = false;
    private bool isSetAnim = false;

    private Coroutine dashCoroutine;

    [SerializeField]private BossMaterialHandle dashEffectHandler;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        // 페이즈 선택 단계로 변경
        phase = AttackPhase.Select;
        isDashStart = false;
        isSetAnim = false;
        isChaniedSkill = false;
        Anim.SetBool("Run", false);
        dashCoroutine = null;
        // 만약 외부(Idle)에서 주입된 스킬 데이터가 있다면 바로 할당
        if (data is EnemySkillAttack skillData)
        {
            currentSkill = skillData;
            phase = AttackPhase.Prepare; // 선택 건너뛰고 바로 준비로
        }
        else currentSkill = null;
    }
    // originMaterial 기억

    public override void UpdateState()
    {
        if (CheckDeath()) return;
        if (phase == AttackPhase.Execute)
        {
            ExecuteAttack();
            return;
        }
        switch (phase)
        {
            case AttackPhase.Select:
                SelectAttack();
                break;

            case AttackPhase.Prepare:
                PrepareAttack();
                break;

            case AttackPhase.Recover:
                Recover();
                break;
        }
    }
    // 공격 스킬 선택 단계
    private void SelectAttack()
    {
        // 컨트롤러를 
        BossController boss = controller as BossController;
        float dis = controller.GetPlayerDis();
        List<EnemySkillAttack> candiateSkills = new List<EnemySkillAttack>();
        foreach(var skill in boss.SkillList)
        {
            if(dis >= skill.AttackMinRange && dis <= skill.AttackMaxRange)
            {
                candiateSkills.Add(skill);
            }
        }
        if(candiateSkills.Count == 0)
        {
            currentSkill = null;
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            return;
        }
        else
        {
            // 후보 스킬 중 랜덤 선택
            int rand = UnityEngine.Random.Range(0, candiateSkills.Count);
            currentSkill = candiateSkills[rand];
        }

        phase = AttackPhase.Prepare;
    }
    private void PrepareAttack()
    {
        if (currentSkill == null)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            return;
        }
        if (!isSetAnim)
        {
            isSetAnim = true;
            isDashStart = false;
            if(currentSkill.NeedDash)
            {
                dashEffectHandler?.SetOriginMats();
            }
            if (currentSkill.NeedLookAtTarget && !isChaniedSkill)
            {
                LookAtTarget();
            }
            Anim.SetInteger("State", 3);
            Anim.SetInteger("Skill", currentSkill.SkillIndex);

            phase = AttackPhase.Execute;
        }
    }
    private void ExecuteAttack()
    {
        if (currentSkill == null)
        {
            controller.TransactionToState(EnumTypes.STATE.IDLE);
            return;
        }

        if (currentSkill.NeedDash)
        {
            if(!isDashStart)
            {
                isDashStart = true;
                dashCoroutine = StartCoroutine(WaitDash());
            }
            return;
        }
        if (!IsPlayingAttack())
        {
            phase = AttackPhase.Recover;
            currentSkill?.StopPlayEffect();
        }
    }
    private void Recover()
    {
        BossController boss = controller as BossController;
     
        if (currentSkill != null && currentSkill.ChainNextSkill)
        {
            currentSkill = boss.SkillList[currentSkill.NextSkillIndex];
            isDashStart = false;
            isChaniedSkill = true;
            phase = AttackPhase.Prepare;
        }
        else
        {
            isChaniedSkill = false;
            currentSkill = null;
            controller.TransactionToState(EnumTypes.STATE.IDLE);
        }
        isSetAnim = false;
    }


    public override void ExitState()
    {
        if(dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }
        StopAllCoroutines();
        ResetAttackState();
        currentSkill = null;
        base.ExitState();

    }

    IEnumerator WaitDash()
    {
        NavigationStop();
        dashEffectHandler?.CreateCharginOutline();
        Vector3 dir = controller.Player.transform.position - transform.position;
        dir.y = 0;
        DashDir = dir.sqrMagnitude < 0.01f ? transform.forward : dir.normalized;
        transform.forward = DashDir;

        float chargeDuration = 2;
        float elapsed = 0;
        

        while(elapsed < chargeDuration)
        {
            if (!(controller.CurrentState is BossAttackState))
            {
                dashEffectHandler?.ResetAll();
                dashCoroutine = null;
                yield break;
            }
            elapsed += Time.deltaTime;
            float ratio = elapsed / chargeDuration;
            dashEffectHandler?.UpdateCharginColor(ratio);
            yield return null;
        }

        float trailTimer = 0f;
        float trailInterval = 0.05f;
        // 대쉬기능
        // 애니메이션을 3으로 세팅해주고
        Anim.SetBool("Run", true);
        // 멈춘걸 푼 다음 속도 조절
        navMeshAgent.isStopped = false;
        
        navMeshAgent.speed = controller.StatComp.SetSpeedMultifle(6);
        navMeshAgent.acceleration = 1000f;
        // 최종 위치 결정
        Vector3 targetPos = transform.position + (DashDir * 10f);

        navMeshAgent.SetDestination(targetPos);
        dashEffectHandler.ClearChargingOutline();
        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.5f)
        {
            if (!(controller.CurrentState is BossAttackState))
            {
                dashCoroutine = null;
                yield break;
            }
            trailTimer += Time.deltaTime;
            if(trailTimer >= trailInterval)
            {
                dashEffectHandler.CreateGhostTrail();
                trailTimer = 0;
            }
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.5f) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        isAttack = false;
        Anim.SetBool("Run", false);

        phase = AttackPhase.Recover;

        dashCoroutine = null;
    }

    private bool IsPlayingAttack()
    {
        // 0번 레이어의 현재 상태 정보를 가져옴
        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(0);
        
        // 태그가 "Attack"인가?
        if (!stateInfo.IsTag("Attack")) return false;

        return stateInfo.normalizedTime < 1f;
    }

    private void ResetAttackState()
    {
        isDashStart = false;
        isSetAnim = false;
        isChaniedSkill = false;
        Anim.SetBool("Run", false);
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = controller.StatComp.SetSpeedMultifle(1);
        navMeshAgent.acceleration = 8f;

        dashEffectHandler?.ResetAll();
    }
}
