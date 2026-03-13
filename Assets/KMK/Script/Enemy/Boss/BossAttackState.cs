using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 문제점 : 공격 선택과 공격을 동시에 진행해 문제가 발생
// => Attack을 선택하는 부분과 공격부분을 분리했어야함 
// => Dash기능이 합쳐져 있어서 하드 코딩됨
// AttackPhase를 만들어 페이즈 별로 관리
public class BossAttackState : EnemyAttackState
{
    private enum AttackPhase { Select, Prepare, Execute, Recover}
    private AttackPhase phase;
    public Vector3 DashDir { get; set; }
    private EnemySkillAttack currentSkill;
    private bool isDashStart = false;
    private bool isChaniedSkill = false;
    private bool isSetAnim = false;
    [SerializeField] private SkinnedMeshRenderer[] bossRenderer;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        if (phase == AttackPhase.Execute) return;
        // 페이즈 선택 단계로 변경
        phase = AttackPhase.Select;
        isDashStart = false;
        // 만약 외부(Idle)에서 주입된 스킬 데이터가 있다면 바로 할당
        if (data is EnemySkillAttack skillData)
        {
            currentSkill = skillData;
            phase = AttackPhase.Prepare; // 선택 건너뛰고 바로 준비로
        }
    }
    public override void UpdateState()
    {
        if (CheckDeath()) return;
        if (phase == AttackPhase.Execute)
        {
            ExecuteAttack();
            return;
        }
        switch (phase)
        {
            case AttackPhase.Select:
                SelectAttack();
                break;

            case AttackPhase.Prepare:
                PrepareAttack();
                break;

            case AttackPhase.Recover:
                Recover();
                break;
        }
    }

    private void SelectAttack()
    {
        BossController boss = controller as BossController;
        float dis = controller.GetPlayerDis();
        List<EnemySkillAttack> candiateSkills = new List<EnemySkillAttack>();
        foreach(var skill in boss.SkillList)
        {
            if(dis >= skill.AttackMinRange && dis <= skill.AttackMaxRange)
            {
                candiateSkills.Add(skill);
            }
        }
        if(candiateSkills.Count == 0)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
        }
        else
        {
            // 후보 스킬 중 랜덤 선택
            int rand = UnityEngine.Random.Range(0, candiateSkills.Count);
            currentSkill = candiateSkills[rand];
        }
        Debug.Log("Selected Skill : " + currentSkill.SkillIndex);
        phase = AttackPhase.Prepare;
    }
    private void PrepareAttack()
    {
        if (!isSetAnim)
        {
            isSetAnim = true;
            isDashStart = false;
            if (currentSkill.NeedLookAtTarget && !isChaniedSkill)
            {
                LookAtTarget();
            }
            Anim.SetInteger("State", 3);
            Anim.SetInteger("Skill", currentSkill.SkillIndex);

            phase = AttackPhase.Execute;
        }
    }
    private void ExecuteAttack()
    {
        if (currentSkill.NeedDash)
        {
            if(!isDashStart)
            {
                isDashStart = true;
                StartCoroutine(WaitDash());
            }
            return;
        }
        if (!IsPlayingAttack())
        {
            phase = AttackPhase.Recover;
            currentSkill?.StopPlayEffect();
        }
    }
    private void Recover()
    {
        BossController boss = controller as BossController;

        if (currentSkill.ChainNextSkill)
        {
            currentSkill = boss.SkillList[currentSkill.NextSkillIndex];
            isDashStart = false;
            isChaniedSkill = true;
            phase = AttackPhase.Prepare;
        }
        else
        {
            isChaniedSkill = false;
            controller.TransactionToState(EnumTypes.STATE.IDLE);
        }
        isSetAnim = false;
    }


    public override void ExitState()
    {
        StopAllCoroutines();
        base.ExitState();
        navMeshAgent.speed = controller.StatComp.SetSpeedMultifle(1);
        navMeshAgent.acceleration = 8f;
    }

    IEnumerator WaitDash()
    {
        NavigationStop();

        yield return new WaitForSeconds(2);

        float trailTimer = 0f;
        float trailInterval = 0.05f;
        // 대쉬기능
        // 애니메이션을 3으로 세팅해주고
        Anim.SetBool("Run", true);
        // 멈춘걸 푼 다음 속도 조절
        navMeshAgent.isStopped = false;
        
        navMeshAgent.speed = controller.StatComp.SetSpeedMultifle(4);
        navMeshAgent.acceleration = 1000f;
        // 최종 위치 결정
        Vector3 targetPos = transform.position + (DashDir * 10f);

        navMeshAgent.SetDestination(targetPos);
        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.5f)
        {
            trailTimer += Time.deltaTime;
            if(trailTimer >= trailInterval)
            {
                CreateGhostTrail();
                trailTimer = 0;
            }
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.5f) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        isAttack = false;
        Anim.SetBool("Run", false);

        phase = AttackPhase.Recover;
    }

    private bool IsPlayingAttack()
    {
        // 0번 레이어의 현재 상태 정보를 가져옴
        AnimatorStateInfo stateInfo = Anim.GetCurrentAnimatorStateInfo(0);
        // 태그가 "Attack"인가?
        return stateInfo.IsTag("Attack");
    }

    private void CreateGhostTrail()
    {
        foreach(var skin in bossRenderer)
        {
            GameObject ghost = new GameObject("GhostTrail");
            MeshFilter mf = ghost.AddComponent<MeshFilter>();
            MeshRenderer mr = ghost.AddComponent<MeshRenderer>();

            Mesh bakedMesh = new Mesh();
            skin.BakeMesh(bakedMesh);
            mf.mesh = bakedMesh;

            mr.material = new Material(skin.material);
            // 2. 렌더링 순서 조정 (본체 뒤에 그려지도록)
            mr.sortingOrder = -1;

            ghost.transform.position = skin.transform.position;
            ghost.transform.rotation = skin.transform.rotation;

            StartCoroutine(FadeOutGhost(mr, 0.5f));
        }

    }

    private IEnumerator FadeOutGhost(MeshRenderer mr, float duration)
    {
        float elapsed = 0;
        Material mat = mr.material;
        Color startColor = mr.material.color;
        Color emitColor = Color.white * 5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            mat.SetColor("_EmissionColor", emitColor * (1f - elapsed / duration));
            yield return null;
        }
        Destroy(mat);
        Destroy(mr.gameObject);
    }
}
