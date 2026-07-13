using UnityEngine;
using UnityEngine.AI;

public class EnemyRoamingState : EnemyState
{
    protected Vector3 targetPos = Vector3.positiveInfinity;
    protected float targetDis = Mathf.Infinity;
    protected float roamDelay;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        // 애니메이션 파라미터 변경
        Anim.SetInteger("State", (int)state);
        NewRandDestination();
    }

    public override void UpdateState()
    {
        float dis = controller.GetPlayerDis();
        if(dis <= statComp.AttackRange)
        {
            controller.TransitionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if(dis <= statComp.DetectRange)
        {
            controller.TransitionToState(EnumTypes.STATE.DETECT);
            return;
        }
        if (targetPos != Vector3.positiveInfinity)
        {
            if (!controller.NavMeshAgent.pathPending && controller.NavMeshAgent.remainingDistance < 1f)
            {
                controller.NavigationStop();
                targetPos = Vector3.positiveInfinity;
                targetDis = Mathf.Infinity;
                controller.TransitionToState(EnumTypes.STATE.IDLE);
            }
        }

    }

    protected virtual void NewRandDestination(bool retry = true)
    {
        Vector2 rand = Random.insideUnitCircle * statComp.NextPoint;
        Vector3 randDir = new Vector3(rand.x, 0, rand.y);
        Vector3 candidate = controller.StatComp.RoamCenter + randDir;


        if (NavMesh.SamplePosition(candidate, out NavMeshHit navCheck, 2.0f, NavMesh.AllAreas))
        {
            targetPos = navCheck.position;
            controller.NavMeshAgent.isStopped = false;
            controller.NavMeshAgent.SetDestination(navCheck.position);
        }
        else if(retry)
        {
            NewRandDestination(false);
        }
    }

    public override void ExitState()
    {
        controller.NavMeshAgent.isStopped = true;
        targetPos = Vector3.positiveInfinity;
        targetDis = Mathf.Infinity;
        controller.NavMeshAgent.speed = statComp.SetSpeedMultifle(1); 
    }
}
