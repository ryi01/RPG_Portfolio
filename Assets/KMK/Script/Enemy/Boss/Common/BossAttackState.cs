using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 문제점 : 공격 선택과 공격을 동시에 진행해 문제가 발생
// => Attack을 선택하는 부분과 공격부분을 분리했어야함 
// => Dash기능이 합쳐져 있어서 하드 코딩됨
// AttackPhase를 만들어 페이즈 별로 관리
public class BossAttackState : EnemyAttackState
{
    private enum AttackPhase { Select, Prepare, Execute, Recover}
    private AttackPhase phase;

    private EnemySkillAttack currentSkill;

    private bool isChaniedSkill = false;
    private bool isSetAnim = false;

    private int lastSkillIndex = -1;

    private float prepareMoveTime = 0f;
    [SerializeField] private float maxPrepareMoveTime = 1.2f;


    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        // 페이즈 선택 단계로 변경
        phase = AttackPhase.Select;
        controller.NavigationStop();
        isSetAnim = false;
        isChaniedSkill = false;
        currentSkill = null;
        if (controller.BossLightning != null) Anim.SetBool("Run", false);

        // 만약 외부(Idle)에서 주입된 스킬 데이터가 있다면 바로 할당
        if (data is EnemySkillAttack skillData)
        {
            currentSkill = skillData;
            phase = AttackPhase.Prepare; // 선택 건너뛰고 바로 준비로
        }

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
        float dis = controller.GetPlayerDis();
        List<EnemySkillAttack> candidateSkills = new List<EnemySkillAttack>();

        foreach (var skill in controller.BossSkill.SkillList)
        {
            if (skill == null || !skill.IsReady) continue;

            if (skill is BossSummonSkillAttack)
            {
                if (controller.BossSummon == null || !controller.BossSummon.CanSummon()) continue;
            }

            if (dis >= skill.AttackMinRange && dis <= skill.AttackMaxRange)
            {
                candidateSkills.Add(skill);
            }
        }

        if (candidateSkills.Count == 0)
        {
            currentSkill = null;
            controller.TransitionToState(EnumTypes.STATE.DETECT);
            return;
        }

        currentSkill = ChooseBossSkill(candidateSkills, dis);
        phase = AttackPhase.Prepare;
    }

    private void PrepareAttack()
    {
        if (currentSkill == null)
        {
            controller.TransitionToState(EnumTypes.STATE.DETECT);
            return;
        }
        float dis = controller.GetPlayerDis();
        if(currentSkill is BossThrowSkill throwSkill)
        {
            controller.NavigationStop();
            if(!isSetAnim)
            {
                isSetAnim = true;
                if (currentSkill.NeedLookAtTarget && !isChaniedSkill)
                {
                    LookAtTarget();
                }
                controller.IsSkillLocked = currentSkill.LockStateDuringSkill;
                Anim.SetInteger("State", 3);
                Anim.SetInteger("Skill", currentSkill.SkillIndex);
            }

            phase = AttackPhase.Execute;
            return;
        }
        if (!isChaniedSkill && !IsGoodAttackDistance(currentSkill, dis))
        {
            prepareMoveTime += Time.deltaTime;
            controller.NavigationResume(1.2f);
            controller.NavMeshAgent.SetDestination(controller.Player.transform.position);
            if(prepareMoveTime >= maxPrepareMoveTime)
            {
                prepareMoveTime = 0f;
                currentSkill = null;
                isChaniedSkill = false;
                phase = AttackPhase.Select;
            }
            return;
        }
        prepareMoveTime = 0f;
        controller.NavigationStop();
        if (!isSetAnim)
        {
            isSetAnim = true;
            if(currentSkill is BossDashSkillAttack dashSkill)
            {
                dashSkill.PrepareSkill();
            }
            if (currentSkill.NeedLookAtTarget && !isChaniedSkill)
            {
                LookAtTarget();
            }

            controller.IsSkillLocked = currentSkill.LockStateDuringSkill;

            Anim.SetInteger("State", 3);
            Anim.SetInteger("Skill", currentSkill.SkillIndex);

            phase = AttackPhase.Execute;
        }
    }
    private void ExecuteAttack()
    {
        if (currentSkill == null)
        {
            controller.TransitionToState(EnumTypes.STATE.IDLE);
            return;
        }

        if (currentSkill is BossDashSkillAttack dashSkill)
        {
            if (!dashSkill.IsRunning && !dashSkill.IsFinished) return;
            if(dashSkill.IsFinished)
            {
                phase = AttackPhase.Recover;
            }
            return;
        }
        if (!IsPlayingAttack())
        {
            currentSkill?.StopPlayEffect();
            phase = AttackPhase.Recover;
        }
    }
    private void Recover()
    {
        controller.IsSkillLocked = false;
        lastSkillIndex = currentSkill != null ? currentSkill.SkillIndex : -1;
        if (currentSkill != null && currentSkill.ChainNextSkill)
        {
            currentSkill = controller.BossSkill.SkillList[currentSkill.NextSkillIndex];
            isChaniedSkill = true;
            isSetAnim = false;
            phase = AttackPhase.Prepare;
            return;
        }
        isChaniedSkill = false;
        isSetAnim = false;
        currentSkill = null;
        phase = AttackPhase.Select;
    }


    public override void ExitState()
    {
        if(currentSkill is BossDashSkillAttack dashSkill)
        {
            dashSkill.ResetSkillState();
        }

        ResetAttackState();
        currentSkill = null;
        controller.IsSkillLocked = false;
        base.ExitState();
    }

    private bool IsPlayingAttack()
    {
        // 0번 레이어의 현재 상태 정보를 가져옴
        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(0);

        if (Anim.IsInTransition(0)) return true;
        // 태그가 "Attack"인가?
        if (!stateInfo.IsTag("Attack")) return false;

        return stateInfo.normalizedTime < 1f;
    }

    private void ResetAttackState()
    {
        isSetAnim = false;
        isChaniedSkill = false;

        if (controller.BossLightning != null) Anim.SetBool("Run", false);
        controller.NavMeshAgent.isStopped = true;
        controller.NavMeshAgent.speed = controller.StatComp.SetSpeedMultifle(1f);
        controller.NavMeshAgent.acceleration = 8f;

    }

    private EnemySkillAttack ChooseBossSkill(List<EnemySkillAttack> candidateSkills, float dis)
    {
        if (dis >= 11f)
        {
            foreach (var skill in candidateSkills)
            {
                if (skill is BossDashSkillAttack)
                    return skill;
            }
        }

        // 같은 스킬 반복 줄이기
        List<EnemySkillAttack> filtered = new List<EnemySkillAttack>();
        foreach (var skill in candidateSkills)
        {
            if (skill.SkillIndex != lastSkillIndex)
                filtered.Add(skill);
        }

        if (filtered.Count > 0)
            return filtered[UnityEngine.Random.Range(0, filtered.Count)];

        return candidateSkills[UnityEngine.Random.Range(0, candidateSkills.Count)];
    }

    private bool IsGoodAttackDistance(EnemySkillAttack skill, float dis)
    {
        if (skill == null) return false;

        float preferredMin = skill.AttackMinRange;
        float preferredMax = skill.AttackMaxRange;

        if (!(skill is BossDashSkillAttack)) preferredMax -= 0.7f;

        return dis >= preferredMin && dis <= preferredMax;
    }
}
