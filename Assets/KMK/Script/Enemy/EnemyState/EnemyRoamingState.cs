using UnityEngine;
using UnityEngine.AI;

public class EnemyRoamingState : EnemyState
{
    protected Vector3 targetPos = Vector3.positiveInfinity;
    protected float targetDis = Mathf.Infinity;

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
        if(dis <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if(dis <= fsmInfo.DetectRange)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            return;
        }
        if (targetPos != Vector3.positiveInfinity)
        {
            targetDis = Vector3.Distance(transform.position, targetPos);
            if (targetDis < 1f || !navMeshAgent.hasPath)
            {
                NavigationStop();
                controller.TransactionToState(EnumTypes.STATE.IDLE);
                return;
            }
        }
        else NewRandDestination();
    }

    protected virtual void NewRandDestination(bool retry = true)
    {
        Vector3 randDir = Random.insideUnitSphere * fsmInfo.NextPoint;
        randDir.y = 0;
        targetPos = fsmInfo.WayPoint.position + randDir;
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit navCheck, 2.0f, NavMesh.AllAreas))
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(navCheck.position);
        }
    }

    public override void ExitState()
    {
        navMeshAgent.isStopped = true;
        targetPos = Vector3.positiveInfinity;
        targetDis = Mathf.Infinity;
        navMeshAgent.speed = fsmInfo.SetSpeedMultifle(1); 
    }
}
