using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

// 보스 스킬 만들기
public class BossController : EnemyController
{
    [SerializeField] private EnemySkillAttack[] skillList;
    [SerializeField] private GameObject wariningPrefab;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float lightningInterval = 2.0f;
    [SerializeField] private float strikeDelay = 1.0f;

    public int LastSkillIndex { get; set; } = -1;
    private bool isPhaseTwo = false;
    public bool IsPhaseTwo { get => isPhaseTwo; }
    public EnemySkillAttack[] SkillList { get => skillList; }
    public bool CoolTimeAttack { get; private set; }
    protected override void Update()
    {
        base.Update();

        float hpRatio = StatComp.CurrentHP / StatComp.MaxHP;
        if(hpRatio < 0.4f && !isPhaseTwo)
        {
            isPhaseTwo = true;
            StatComp.SetSpeedMultifle(2);
            TransactionToState(EnumTypes.STATE.PATTERN_PHASE);

            StartCoroutine(LightningRoutine());
        }
    }

    public void ExccuteAttack(EnemySkillAttack skill, NavMeshAgent navMeshAgent)
    {
        if (CoolTimeAttack) return;
        if (skill.SkillIndex == 2)
        {
            if (TryGetComponent<BossAttackState>(out BossAttackState bossAttack))
            {
                Vector3 pPos = Player.transform.position;
                Vector3 bPos = transform.position;
                pPos.y = 0;
                bPos.y = 0;
                Vector3 dir = (pPos - bPos).normalized;
                bossAttack.DashDir = dir == Vector3.zero ? transform.forward : dir;
            }
        }
        navMeshAgent.isStopped = true;
        TransactionToState(EnumTypes.STATE.ATTACK, skill);
        StartCoroutine(AttackCoolTimeRoutine(2));
    }
    private IEnumerator AttackCoolTimeRoutine(float delay)
    {
        CoolTimeAttack = true;
        yield return new WaitForSeconds(delay);
        CoolTimeAttack = false;
    }    

    private IEnumerator LightningRoutine()
    {
        while(isPhaseTwo)
        {
            yield return new WaitForSeconds(lightningInterval);
            if (Player == null) yield break;
            Vector3 strikePos = Player.transform.position;
            strikePos.y = 0.05f;

            StartCoroutine(ExecuteLightning(strikePos));
        }
    }
    private IEnumerator ExecuteLightning(Vector3 pos)
    {
        GameObject warning = Instantiate(wariningPrefab, pos, Quaternion.identity);

        yield return new WaitForSeconds(strikeDelay);

        Destroy(warning);
        GameObject bolt = Instantiate(lightningPrefab, pos, Quaternion.identity);
        Destroy(bolt, 2);
    }
}
