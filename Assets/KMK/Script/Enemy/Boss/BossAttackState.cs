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

    private EnemySkillAttack currentSkill;
    private bool isChaniedSkill = false;
    private bool isSetAnim = false;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        // 페이즈 선택 단계로 변경
        phase = AttackPhase.Select;

        isSetAnim = false;
        isChaniedSkill = false;
        Anim.SetBool("Run", false);

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
            if(currentSkill is BossDashSkillAttack dashSkill)
            {
                dashSkill.PrepareSkill();
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
        BossController boss = controller as BossController;
     
        if (currentSkill != null && currentSkill.ChainNextSkill)
        {
            currentSkill = boss.SkillList[currentSkill.NextSkillIndex];
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
        if(currentSkill is BossDashSkillAttack dashSkill)
        {
            dashSkill.ResetSkillState();
        }

        ResetAttackState();
        currentSkill = null;

        base.ExitState();
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
        isSetAnim = false;
        isChaniedSkill = false;

        Anim.SetBool("Run", false);
        controller.NavMeshAgent.isStopped = true;
        controller.NavMeshAgent.speed = controller.StatComp.SetSpeedMultifle(1f);
        controller.NavMeshAgent.acceleration = 8f;

    }
}
