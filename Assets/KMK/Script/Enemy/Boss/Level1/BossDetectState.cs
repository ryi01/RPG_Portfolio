using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossDetectState : EnemyDetectState
{
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
        if(HasAnyAvailableSkillInRange(dis))
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
    public void OnWalkSFX()
    {
        PlaySFX();
    }
    public override void ExitState()
    {
        if (Anim != null) Anim.SetBool("Run", false);
        base.ExitState();
    }

}
