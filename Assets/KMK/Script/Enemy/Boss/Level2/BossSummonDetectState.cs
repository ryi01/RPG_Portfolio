using UnityEngine;
using UnityEngine.UIElements;

public class BossSummonDetectState : EnemyState
{
    [SerializeField] private Vector2 retreatRange = new Vector2(3, 5);
    [SerializeField] private float retreatDist = 4f;
    [SerializeField] private float attackDecisionRange = 15f;
    [SerializeField] private float changeSpeedMultiplier = 1.5f;
    [SerializeField] private float keepDistance = 6f;
    private bool isRetreating = false;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        controller.NavigationResume(changeSpeedMultiplier);
        if (Anim != null)
        {
            Anim.SetInteger("State", (int)state);
        }
        isRetreating = false;
    }

    public override void UpdateState()
    {
        if (CheckDeath()) return;
        if (controller.Player == null || controller.BossSkill == null)
        {
            controller.TransitionToState(EnumTypes.STATE.IDLE);
            return;
        }
        float dis = controller.GetPlayerDis();

        if(!isRetreating && dis <= retreatRange.x)
        {
            isRetreating = true;
        }    
        // 도망중이면 충분히 멀어질때까지 도망
        if(isRetreating)
        {
            if (dis >= retreatRange.y) isRetreating = false;
            else
            {
                Retreat();
                return;
            }
        }
        if(dis <= attackDecisionRange && HasAnyAvailableSkillInRange(dis))
        {
            controller.TransitionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if(dis > retreatRange.y && dis <= keepDistance + 0.5f)
        {
            controller.NavigationStop();
            LookAtTarget();
            return;
        }
        // 감지 범위면 추적
        if(dis <= statComp.DetectRange)
        {
            Chase();
            return;
        }
        controller.TransitionToState(EnumTypes.STATE.IDLE);
    }

    public override void ExitState()
    {
        isRetreating = false;
    }

    private void Chase()
    {
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;
        Vector3 playerPos = controller.Player.transform.position;
        Vector3 myPos = controller.transform.position;

        Vector3 dir = playerPos - myPos;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;

        Vector3 targetPos = playerPos - dir.normalized * keepDistance;
        LookAtTarget();

        controller.NavigationResume(changeSpeedMultiplier);
        controller.NavMeshAgent.SetDestination(targetPos);
    }

    private void Retreat()
    {
        if (controller.NavMeshAgent == null || !controller.NavMeshAgent.isOnNavMesh) return;
        Vector3 dir = controller.transform.position - controller.Player.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) dir = -controller.transform.forward;

        Vector3 retreatTarget = controller.transform.position + dir.normalized * retreatDist;
        LookAtTarget();
        controller.NavigationResume(changeSpeedMultiplier);
        controller.NavMeshAgent.SetDestination(retreatTarget);
    }

}
