using System.Collections;
using UnityEngine;

public class EnemyDamageState : EnemyState
{
    [SerializeField] protected ParticleSystem hitParticle;
    private Coroutine hitCoroutine;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        SetEffect(hitParticle);
        float force = statComp.NockbackForce;
        Transform attacker = null;
        if (data is HitData hitData)
        {
            force = hitData.Force;
            attacker = hitData.Attacker;
        }
        else if(data is float hitforce)
        {
            force = hitforce;
        }    
        controller.NavigationStop();
        // 파티클 
        // 애니메이션
        PlayEffect();
        if (Anim != null) Anim.SetInteger("State", (int)state);
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        Vector3 dir;
        if (attacker != null)
        {
            dir = transform.position - attacker.position;
        }
        else if (controller.Player != null)
        {
            dir = transform.position - controller.Player.transform.position;
        }
        else dir = -transform.forward;
        hitCoroutine = StartCoroutine(ApllyHitKnockback(dir.normalized, force));
    }

    public override void UpdateState()
    {
        if (CheckDeath()) return;
        if (statComp.IsHit || IsPlayingHit()) return;
        if(controller.GetPlayerDis() <= statComp.AttackRange)
        {
            controller.TransitionToState(EnumTypes.STATE.ATTACK);
        }
        else controller.TransitionToState(EnumTypes.STATE.DETECT);
    }
    public override void ExitState()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }
        statComp.IsHit = false;
    }
    public IEnumerator ApllyHitKnockback(Vector3 hitDir, float force)
    {
        statComp.IsHit = true;
        float time = 0;
        while (time < statComp.KnckBackTime)
        {
            if (controller.NavMeshAgent != null && controller.NavMeshAgent.isActiveAndEnabled && controller.NavMeshAgent.isOnNavMesh)
            {
                controller.NavMeshAgent.Move(hitDir * force * Time.deltaTime);
            }

            time += Time.deltaTime;
            yield return null;
        }
        if (controller.NavMeshAgent != null && controller.NavMeshAgent.isActiveAndEnabled && controller.NavMeshAgent.isOnNavMesh)
        {
            controller.NavMeshAgent.isStopped = false;
        }
        statComp.IsHit = false;
        hitCoroutine = null;
    }
    private bool IsPlayingHit()
    {
        if (Anim == null) return false;
        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(0);
        // 상태 이름이나 태그로 피격 애니메이션 확인
        return stateInfo.IsTag("Hit");
    }
}
