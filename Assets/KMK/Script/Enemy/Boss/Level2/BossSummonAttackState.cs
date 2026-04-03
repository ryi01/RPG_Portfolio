using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BossSummonAttackState : EnemyAttackState
{
    private EnemySkillAttack summonSkill;
    private EnemySkillAttack homingSkill;
    private EnemySkillAttack knockbackSkill;
    private EnemySkillAttack linearSkill;

    private EnemySkillAttack currentSkill;
    private EnemySkillAttack lastUsedSkill = null;

    [SerializeField] private float summonChance = 0.3f;
    [SerializeField] private float summonBlockTime = 5f;
    [SerializeField] private bool isReturnToDetectIfNoSkill = true;
    [SerializeField] private bool allowSameSkillTwice = false;

    private float lastSummonTime = -999f;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        InitSkills();

        currentSkill = null;

        if (data is EnemySkillAttack skillData)
        {
            currentSkill = skillData;
        }
        if (Anim != null)
        {
            Anim.SetInteger("State", (int)EnumTypes.STATE.ATTACK);
        }
    }

    public override void UpdateState()
    {
        if (CheckDeath()) return;

        if (controller.Player == null) return;
        if (controller.BossSkill == null || controller.BossSkill.SkillList == null)
        {
            controller.TransitionToState(EnumTypes.STATE.IDLE);
            return;
        }

        if (IsAttack) return;
        LookAtTarget();

        if (currentSkill == null) currentSkill = SelectSkillDistance();
        if (currentSkill == null)
        {
            if (isReturnToDetectIfNoSkill) controller.TransitionToState(EnumTypes.STATE.DETECT);
            else controller.TransitionToState(EnumTypes.STATE.IDLE);

            return;
        }

        StartSkill();
    }

    public override void ExitState()
    {
        base.ExitState();
        currentSkill = null;
    }
    private void InitSkills()
    {
        summonSkill = null;
        homingSkill = null;
        knockbackSkill = null;
        linearSkill = null;

        var skills = controller.BossSkill.SkillList;

        if (skills == null) return;
        foreach(var skill in skills)
        {
            if (skill == null) continue;
            if (skill is BossHomingMissileSkillAttack) homingSkill = skill;
            else if(skill is BossSummonSkillAttack) summonSkill = skill;
            else if(skill is BossKnockbackBurstSkillAttack) knockbackSkill = skill;
            else if(skill is BossLinearShotSkillAttack) linearSkill = skill;
        }
    }
    private EnemySkillAttack SelectSkillDistance()
    {
        float dis = controller.GetPlayerDis();

        if (dis <= 5 && CanSelectSkill(knockbackSkill, dis, true))
        {
            return knockbackSkill;
        }

        List<EnemySkillAttack> candidates = new List<EnemySkillAttack>();
        AddCandidate(candidates, linearSkill, dis);
        AddCandidate(candidates, homingSkill, dis);

        if (CanSelectSummonSkill(summonSkill, dis, true) && Random.value < summonChance)
        {
            candidates.Add(summonSkill);
        }
        
        EnemySkillAttack selected = SelectRandomCandidate(candidates);
        if (selected != null) return selected;

        if (CanUseSkill(linearSkill, dis)) return linearSkill;
        if (CanUseSkill(homingSkill, dis)) return homingSkill;
        if (CanUseSkill(knockbackSkill, dis)) return knockbackSkill;

        return null;
    }
    private void AddCandidate(List<EnemySkillAttack> list, EnemySkillAttack skill, float dis)
    {
        if (CanSelectSkill(skill, dis, true))
        {
            list.Add(skill);
        }
    }

    private EnemySkillAttack SelectRandomCandidate(List<EnemySkillAttack> list)
    {
        if (list == null || list.Count == 0) return null;

        int rndIndex = Random.Range(0, list.Count);
        return list[rndIndex];
    }

    private bool CanSelectSkill(EnemySkillAttack skill, float dis, bool blockSameSkill)
    {
        if (!CanUseSkill(skill, dis)) return false;
        if (!allowSameSkillTwice && blockSameSkill && blockSameSkill && skill == lastUsedSkill) return false;
        return true;
    }
    private void StartSkill()
    {
        if (currentSkill == null) return;

        controller.NavigationStop();
        LookAtTarget();

        if (Anim != null)
        {
            Anim.SetInteger("State", (int)EnumTypes.STATE.ATTACK);
            Anim.SetInteger("Skill", currentSkill.SkillIndex);
        }
    }

    public void OnSkillCast()
    {
        if (currentSkill == null) return;
        lastUsedSkill = currentSkill;
        currentSkill.SetLastTime();
        if (currentSkill == summonSkill)
        {
            lastSummonTime = Time.time;
        }
    }

    private bool CanSelectSummonSkill(EnemySkillAttack skill, float dis, bool blockSameSkill)
    {
        if(!CanUseSummonSkill(skill, dis)) return false;
        if (!allowSameSkillTwice && blockSameSkill && skill == lastUsedSkill) return false;
        return true;
    }

    private bool CanUseSummonSkill(EnemySkillAttack skill, float distance)
    {
        if (!CanUseSkill(skill, distance)) return false;
        if (controller.BossSummon == null || !controller.BossSummon.CanSummon()) return false;
        if (Time.time < lastSummonTime + summonBlockTime) return false;
        return true;
    }
    private bool CanUseSkill(EnemySkillAttack skill, float distance)
    {
        if(skill == null) return false;
        if(!skill.IsReady) return false;
        if (distance < skill.AttackMinRange || distance > skill.AttackMaxRange) return false;

        return true;
    }

    public void OnAttackFinish()
    {
        currentSkill = null;
        if (isReturnToDetectIfNoSkill) controller.TransitionToState(EnumTypes.STATE.DETECT);
        else controller.TransitionToState(EnumTypes.STATE.IDLE);
    }
}
