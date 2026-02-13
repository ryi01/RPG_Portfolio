using UnityEngine;
using UnityEngine.AI;

public class EnemyRoamingState : EnemyState
{
    protected Transform targetTrans = null;

    protected Vector3 targetPos = Vector3.positiveInfinity;
    protected float targetDis = Mathf.Infinity;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        // 애니메이션 파라미터 변경
        Anim.SetInteger("State", (int)state);
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
        if(targetTrans != null)
        {
            targetDis = Vector3.Distance(transform.position, targetPos);
            if (targetDis < 1f)
            {
                controller.TransactionToState(EnumTypes.STATE.IDLE);
                return;
            }
        }
    }

    protected virtual void NewRandDestination(bool retry = true)
    {
        int index = Random.Range(0, fsmInfo.WanderPoints.Length);
        float dis = Vector3.Distance(fsmInfo.WanderPoints[index].position, transform.position);

        if(dis < fsmInfo.NextPoint && retry)
        {
            NewRandDestination();
            return;
        }
        // 로밍 중심 위치 설정
        targetTrans = fsmInfo.WanderPoints[index];
        // 구체로 랜덤한 방향 벡터 구하기 => nextPoint가 반지름
        Vector3 randDir = Random.insideUnitSphere;
        // y 축 제거
        randDir.y = 0;
        randDir *= fsmInfo.NextPoint;
        // 상대적 거리에서 실제거리로 변경 
        randDir += fsmInfo.WanderPoints[index].position;

        targetPos = randDir;

        NavMeshHit navCheck;
        if(NavMesh.SamplePosition(targetPos, out navCheck, fsmInfo.WanderNavCheckRadius,1))
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(targetPos);
        }
    }

    public override void ExitState()
    {
        navMeshAgent.isStopped = true;
        targetTrans = null;
        targetPos = Vector3.positiveInfinity;
        targetDis = Mathf.Infinity;
        navMeshAgent.speed = fsmInfo.SetSpeedMultifle(1); 
    }
}
