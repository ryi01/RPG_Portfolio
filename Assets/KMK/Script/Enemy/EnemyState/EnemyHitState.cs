using System.Collections;
using UnityEngine;

public class EnemyHitState : EnemyState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        float force = fsmInfo.NockbackForce;
        if (data != null)
        {
            force = (float)data;
        }
        if (controller.NavMeshAgent.isActiveAndEnabled && controller.NavMeshAgent.isOnNavMesh)
        {
            controller.NavMeshAgent.isStopped = true;
        }
        // 파티클 
        // 애니메이션
        Anim.SetInteger("State", (int)state);

        StartCoroutine(ApllyHitKnockback(-transform.forward, force));
    }
    public override void UpdateState()
    {
        if (fsmInfo.IsHit) return;
        if(controller.GetPlayerDis() <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);  
            return;
        }
        controller.TransactionToState(EnumTypes.STATE.DETECT);
    }
    public IEnumerator ApllyHitKnockback(Vector3 hitDir, float force)
    {
        fsmInfo.IsHit = true;
        controller.NavigationStop();
        float time = 0;
        while (time < fsmInfo.KnckBackTime)
        {
            if (controller.NavMeshAgent.isActiveAndEnabled)
            {
                controller.NavMeshAgent.Move(hitDir * force * Time.deltaTime);
            }

            time += Time.deltaTime;
            yield return null;
        }
        if (controller.NavMeshAgent.isActiveAndEnabled && controller.NavMeshAgent.isOnNavMesh)
        {
            controller.NavMeshAgent.isStopped = false;
        }
        fsmInfo.IsHit = false;
    }
}
