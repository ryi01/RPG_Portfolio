using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BossSummonAttackState : EnemyAttackState
{
    private EnemySkillAttack currentSkill;
    private enum SummonSkill { SUMMON, HOMING, KNOCKBACK };

    private SummonSkill? lastUsedSkill = null;
    private float lastSummonTime = -100f;
    [SerializeField] private bool IsReturnToDetectIsNoSkill = true;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
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

    private EnemySkillAttack SelectSkillDistance()
    {
        float dis = controller.GetPlayerDis();

        EnemySkillAttack[] skills = controller.BossSkill.SkillList;
        List<EnemySkillAttack> candiates = new List<EnemySkillAttack>();

        if (dis <= 5 && CanUseSkill(skills, SummonSkill.KNOCKBACK, dis, out EnemySkillAttack knockback)) return knockback;
        if (CanUseSummonSkill(skills, SummonSkill.SUMMON, dis, out EnemySkillAttack summon) && lastUsedSkill != SummonSkill.SUMMON && Random.value < 0.3f) candiates.Add(summon);
        if (CanUseSkill(skills, SummonSkill.HOMING, dis, out EnemySkillAttack homing)) candiates.Add(homing);
        
        if (candiates.Count > 0)
        {
            int rnd = Random.Range(0, candiates.Count);
            return candiates[rnd];
        }
        
        if (CanUseSummonSkill(skills, SummonSkill.SUMMON, dis, out summon) && Random.value < 0.2f) return summon;
        if (CanUseSkill(skills, SummonSkill.KNOCKBACK, dis, out knockback)) return knockback;
        if (CanUseSkill(skills, SummonSkill.HOMING, dis, out homing)) return homing;
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
        lastUsedSkill = GetSkillTypeFromIndex(currentSkill.SkillIndex);

        currentSkill.SetLastTime();
    }
    private SummonSkill GetSkillTypeFromIndex(int skillIndex)
    {
        return (SummonSkill)skillIndex;
    }
    private bool CanUseSummonSkill(EnemySkillAttack[] skills, SummonSkill skillType, float distance, out EnemySkillAttack skill)
    {
        skill = null;
        if (!CanUseSkill(skills, SummonSkill.SUMMON, distance, out skill)) return false;
        if (!controller.BossSummon.CanSummon()) return false;

        if (Time.time - lastSummonTime < 10) return false;
        return true;
    }
    private bool CanUseSkill(EnemySkillAttack[] skills, SummonSkill skillType, float distance, out EnemySkillAttack skill)
    {
        skill = null;
        if (!TryGetSkill(skills, skillType, out skill))
        {
            return false;
        }
        if (!skill.IsReady)
        {
            return false;
        }
        if (distance < skill.AttackMinRange || distance > skill.AttackMaxRange)
        {
            return false;
        }
        return true;
    }
    private bool TryGetSkill(EnemySkillAttack[] skills, SummonSkill skillType, out EnemySkillAttack skill)
    {
        skill = null;
        if (skills == null) return false;

        int index = (int)skillType;
        if (index < 0 || index >= skills.Length) return false;
        if (skills[index] == null) return false;

        skill = skills[index];
        return true;

    }

    public void OnAttackFinish()
    {
        Debug.Log("OnAttackFinish »£√‚µ ");
        currentSkill = null;
        if (IsReturnToDetectIsNoSkill) controller.TransitionToState(EnumTypes.STATE.DETECT);
        else controller.TransitionToState(EnumTypes.STATE.IDLE);
    }
}
