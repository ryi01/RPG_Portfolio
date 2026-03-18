using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossDashSkillAttack : EnemySkillAttack
{
    [SerializeField] private BossMaterialHandle dashEffectHandler;

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
        if (IsRunning) return;
        IsRunning = true;
        IsFinished = false;
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(WaitDash());
    }

    IEnumerator WaitDash()
    {
        if(TryGetComponent<BossController>(out boss)) boss.NavigationStop();

        dashEffectHandler?.SetOriginMats();
        dashEffectHandler?.CreateCharginOutline();
        Vector3 dir = boss.Player.transform.position - transform.position;
        dir.y = 0;
        Vector3 dashDir = dir.sqrMagnitude < 0.01f ? transform.forward : dir.normalized;
        transform.forward = dashDir;

        float chargeDuration = 2;
        float elapsed = 0;

        while (elapsed < chargeDuration)
        {
            if (!(boss.CurrentState is BossAttackState))
            {
                ForceStop();
                yield break;
            }
            elapsed += Time.deltaTime;
            float ratio = elapsed / chargeDuration;
            dashEffectHandler?.UpdateCharginColor(ratio);
            yield return null;
        }

        float trailTimer = 0f;
        float trailInterval = 0.05f;
        // 대쉬기능
        // 애니메이션을 3으로 세팅해주고
        boss.Animator.SetBool("Run", true);
        // 멈춘걸 푼 다음 속도 조절
        boss.NavMeshAgent.isStopped = false;

        boss.NavMeshAgent.speed = boss.StatComp.SetSpeedMultifle(6);
        boss.NavMeshAgent.acceleration = 1000f;
        // 최종 위치 결정
        Vector3 targetPos = transform.position + (dashDir * 10f);

        boss.NavMeshAgent.SetDestination(targetPos);
        dashEffectHandler.ClearChargingOutline();
        while (boss.NavMeshAgent.pathPending || boss.NavMeshAgent.remainingDistance > 0.5f)
        {
            if (!(boss.CurrentState is BossAttackState))
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
            if (!boss.NavMeshAgent.pathPending && boss.NavMeshAgent.remainingDistance <= 0.5f) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

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
        boss.Animator.SetBool("Run", false);
        boss.NavMeshAgent.isStopped = true;
        boss.NavMeshAgent.speed = boss.StatComp.SetSpeedMultifle(1);
        boss.NavMeshAgent.acceleration = 8f;

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
