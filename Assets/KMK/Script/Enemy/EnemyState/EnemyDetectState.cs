using UnityEngine;

public class EnemyDetectState : EnemyState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;

        controller.NavigationResume();
        controller.NavMeshAgent.speed = statComp.SetSpeedMultifle(1.5f);
        if (Anim != null)
        {
            Anim.SetInteger("State", (int)state);
            if(controller.BossQuest != null ) Anim.SetBool("Run", false);
        }
    }
    public override void UpdateState()
    {
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;
        float dis = controller.GetPlayerDis();
        if(dis <= statComp.AttackRange)
        {
            controller.TransitionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if(dis > statComp.DetectRange)
        {
            controller.TransitionToState(EnumTypes.STATE.RETURN);
            return;

        }
        controller.NavigationResume();
        controller.NavMeshAgent.SetDestination(controller.Player.transform.position);
    }

}
