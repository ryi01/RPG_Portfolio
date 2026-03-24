using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDetectState : EnemyDetectState
{
    private int dashSkillIndex = 2;

    public override void UpdateState()
    {
        if (CheckDeath()) return;
        if (controller.Player == null) return;
        if (controller.BossSkill == null)
        {
            controller.TransitionToState(EnumTypes.STATE.IDLE);
            return;
        }
        // 1. 플레이어와 보스의 거리를 측정하고
        float dis = controller.GetPlayerDis();
        EnemySkillAttack[] skills = controller.BossSkill.SkillList;
        if (skills == null || skills.Length == 0) return;
        if(dashSkillIndex >=0 && dashSkillIndex < skills.Length)
        {
            EnemySkillAttack dashSkill = skills[dashSkillIndex];
            if(dashSkill != null && dashSkill.IsReady && dis >= dashSkill.AttackMinRange && dis <= dashSkill.AttackMaxRange )
            {
                controller.TransitionToState(EnumTypes.STATE.ATTACK, dashSkill);
                return;
            }
        }
        if(dis < statComp.AttackRange)
        {
            controller.TransitionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if(dis > statComp.DetectRange)
        {
            controller.TransitionToState(EnumTypes.STATE.IDLE);
            return;
        }
        controller.NavigationResume(1.5f);
        controller.NavMeshAgent.SetDestination(controller.Player.transform.position);
    }

    public override void ExitState()
    {
        if (Anim != null) Anim.SetBool("Run", false);
        base.ExitState();
    }

}
