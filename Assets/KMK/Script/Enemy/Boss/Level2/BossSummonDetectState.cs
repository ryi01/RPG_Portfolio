using UnityEngine;

public class BossSummonDetectState : EnemyState
{
    [SerializeField] private float retreatRange = 3f;
    [SerializeField] private float retreatDist = 4f;
    [SerializeField] private Vector2 summonRange = new Vector2(4, 8);
    [SerializeField] private float changeSpeedMultiplier = 1.5f;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        controller.NavigationResume(changeSpeedMultiplier);
        if (Anim != null)
        {
            Anim.SetInteger("State", (int)state);
        }
    }

    public override void UpdateState()
    {
        if (CheckDeath()) return;
        if (controller.Player == null) return;
        float dis = controller.GetPlayerDis();

        if(dis <= retreatRange)
        {
            Retreat();
            return;
        }    

        if(controller.BossSummon != null && controller.BossSummon.CanSummon() && dis >= summonRange.x && dis <= summonRange.y)
        {
            controller.TransitionToState(EnumTypes.STATE.ATTACK);
            return;
        }

        if(dis <= statComp.DetectRange)
        {
            Chase();
            return;
        }

        controller.TransitionToState(EnumTypes.STATE.IDLE);
    }

    private void Chase()
    {
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;
        controller.NavigationResume(changeSpeedMultiplier);
        controller.NavMeshAgent.SetDestination(controller.Player.transform.position);
    }

    private void Retreat()
    {
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;
        Vector3 dir = controller.transform.position - controller.Player.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) dir = -controller.transform.forward;

        Vector3 retreatTarget = controller.transform.position + dir.normalized * retreatDist;
        controller.NavigationResume(changeSpeedMultiplier);
        controller.NavMeshAgent.SetDestination(retreatTarget);
    }
}
