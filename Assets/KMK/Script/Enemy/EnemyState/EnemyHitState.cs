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
        navMeshAgent.isStopped = true;
        // 파티클 
        // 애니메이션
        StartCoroutine(ApllyHitKnockback(-transform.forward, force));
    }
    public override void UpdateState()
    {
        
    }
    public IEnumerator ApllyHitKnockback(Vector3 hitDir, float force)
    {
        fsmInfo.IsHit = true;
        NavigationStop();
        float time = 0;
        while (time < fsmInfo.KnckBackTime)
        {
            navMeshAgent.Move(hitDir * force * Time.deltaTime);

            time += Time.deltaTime;
            yield return null;
        }
        navMeshAgent.isStopped = false;
        fsmInfo.IsHit = false;
    }
}
