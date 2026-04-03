using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossDashSkillAttack : EnemySkillAttack
{
    [SerializeField] private BossMaterialHandle dashEffectHandler;
    [SerializeField] private float chargeDuration = 0.6f;
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private float endDelay = 0.05f;

    private Coroutine dashCoroutine;

    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        IsRunning = false;
        IsFinished = false;
    }
    public void OnDashAttack()
    {
        if (IsRunning || owner == null) return;
        IsRunning = true;
        IsFinished = false;
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(WaitDash());
    }

    IEnumerator WaitDash()
    {
        dashEffectHandler?.SetOriginMats();
        dashEffectHandler?.CreateCharginOutline();
        if(owner.Player == null)
        {
            ForceStop();
            yield break;
        }

        Vector3 dir = owner.Player.transform.position - transform.position;
        dir.y = 0;
        Vector3 dashDir = dir.sqrMagnitude < 0.01f ? transform.forward : dir.normalized;
        transform.forward = dashDir;

        owner.NavigationStop();
        float elapsed = 0;

        while (elapsed < chargeDuration)
        {
            if(owner.CurrentState == null || owner.CurrentState.StateType != EnumTypes.STATE.ATTACK)
            {
                ForceStop();
                yield break;
            }
            owner.NavigationStop();

            elapsed += Time.deltaTime;
            float ratio = elapsed / chargeDuration;
            dashEffectHandler?.UpdateCharginColor(ratio);
            yield return null;
        }
        if(owner.NavMeshAgent == null || !owner.NavMeshAgent.enabled || !owner.NavMeshAgent.isOnNavMesh)
        {
            ForceStop();
            yield break;
        }    
        float trailTimer = 0f;
        float trailInterval = 0.05f;
        // 대쉬기능
        // 애니메이션을 3으로 세팅해주고
        owner.NavigationResume();
        owner.Animator.SetBool("Run", true);
        // 멈춘걸 푼 다음 속도 조절
        owner.NavMeshAgent.isStopped = false;

        owner.NavMeshAgent.speed = owner.StatComp.SetSpeedMultifle(6);
        owner.NavMeshAgent.acceleration = 1000f;
        // 최종 위치 결정
        Vector3 targetPos = transform.position + (dashDir * dashDistance);

        owner.NavMeshAgent.SetDestination(targetPos);
        dashEffectHandler.ClearChargingOutline();
        while (owner.NavMeshAgent.pathPending || owner.NavMeshAgent.remainingDistance > 0.5f)
        {
            if(owner.CurrentState == null || owner.CurrentState.StateType != EnumTypes.STATE.ATTACK)
            {
                ForceStop();
                yield break;
            }
            trailTimer += Time.deltaTime;
            if (trailTimer >= trailInterval)
            {
                dashEffectHandler.CreateGhostTrail();
                trailTimer = 0;
            }
            if (!owner.NavMeshAgent.pathPending && owner.NavMeshAgent.remainingDistance <= 0.5f) break;
            yield return null;
        }
        yield return new WaitForSeconds(endDelay);

        ForceStop();
    }
    public void ResetSkillState()
    {
        if(dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }
        ForceStop();
    }
    private void ForceStop()
    {
        if(owner != null)
        {
            if (owner.Animator != null) owner.Animator.SetBool("Run", false);
            if(owner.NavMeshAgent != null && owner.NavMeshAgent.enabled)
            {
                owner.NavMeshAgent.isStopped = true;
                owner.NavMeshAgent.speed = owner.StatComp.SetSpeedMultifle(1);
                owner.NavMeshAgent.acceleration = 8f;
            }
        }

        dashEffectHandler?.ResetAll();

        IsRunning = false;
        IsFinished = true;
        dashCoroutine = null;
    }

    public void PrepareSkill()
    {
        IsRunning = false;
        IsFinished = false;
    }
}
