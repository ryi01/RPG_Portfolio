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
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 1f)
            {
                NavigationStop();
                targetPos = Vector3.positiveInfinity;
                targetDis = Mathf.Infinity;
                controller.TransactionToState(EnumTypes.STATE.IDLE);
            }
        }

    }

    protected virtual void NewRandDestination(bool retry = true)
    {
        Vector2 rand = Random.insideUnitCircle * fsmInfo.NextPoint;
        Vector3 randDir = new Vector3(rand.x, 0, rand.y);
        Vector3 candidate = controller.StatComp.RoamCenter + randDir;


        if (NavMesh.SamplePosition(candidate, out NavMeshHit navCheck, 2.0f, NavMesh.AllAreas))
        {
            targetPos = navCheck.position;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(navCheck.position);
        }
        else if(retry)
        {
            NewRandDestination(false);
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
