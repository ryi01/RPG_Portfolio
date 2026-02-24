using System.Collections;
using UnityEngine;

public class EnemyHitState : EnemyState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (state != EnumTypes.STATE.DEATH && controller.StatComp.CurrentHP <= 0) return;
        base.EnterState(state, data);
        float force = fsmInfo.NockbackForce;
        if (data != null)
        {
            force = (float)data;
        }
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
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
        NavigationStop();
        float time = 0;
        while (time < fsmInfo.KnckBackTime)
        {
            if (navMeshAgent.isActiveAndEnabled)
            {
                navMeshAgent.Move(hitDir * force * Time.deltaTime);
            }

            time += Time.deltaTime;
            yield return null;
        }
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false;
        }
        fsmInfo.IsHit = false;
    }
}
