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
    [SerializeField] private bool IsReturnToDetectIsNoSkill = true;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        InitSkills();

        currentSkill = null;

        if (data is EnemySkillAttack skillData)
        {
            currentSkill = skillData;
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
            controller.TransitionToState(EnumTypes.STATE.DETECT);
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

        EnemySkillAttack[] skills = controller.BossSkill.SkillList;
        List<EnemySkillAttack> candidates = new List<EnemySkillAttack>();

        if (dis <= 5 && CanUseSkill(knockbackSkill, dis)) return knockbackSkill;
        if(CanUseSkill(linearSkill, dis)) candidates.Add(linearSkill);
        if (CanUseSkill(homingSkill, dis)) candidates.Add(homingSkill);
        if (CanUseSummonSkill(summonSkill, dis) && lastUsedSkill != summonSkill && Random.value < summonChance) candidates.Add(summonSkill);
        
        if (candidates.Count > 0)
        {
            int rnd = Random.Range(0, candidates.Count);
            return candidates[rnd];
        }

        if (CanUseSkill(linearSkill, dis)) return linearSkill;
        if (CanUseSkill(homingSkill, dis)) return homingSkill;
        if (CanUseSkill(knockbackSkill, dis)) return knockbackSkill;
        if (CanUseSummonSkill(summonSkill, dis) && Random.value < summonChance) return summonSkill;
        return null;
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
    }

    private bool CanUseSummonSkill(EnemySkillAttack skill, float distance)
    {
        if(!CanUseSkill(skill, distance)) return false;
        if(!controller.BossSummon.CanSummon()) return false;
        if(!skill.IsReady) return false;
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
        if (IsReturnToDetectIsNoSkill) controller.TransitionToState(EnumTypes.STATE.DETECT);
        else controller.TransitionToState(EnumTypes.STATE.IDLE);
    }
}
