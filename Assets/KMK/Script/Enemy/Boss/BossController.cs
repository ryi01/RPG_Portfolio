using System.Collections.Generic;
using UnityEngine;

// 보스 스킬 만들기
public class BossController : EnemyController
{
    [SerializeField] private List<EnemySkillAttack> skillList;
    public EnemySkillAttack CurrentSkill { get; private set; }
    private bool isPhaseTwo = false;
    protected override void Update()
    {
        base.Update();

        float hpRatio = StatComp.CurrentHP / StatComp.MaxHP;
        if(hpRatio < 0.4f && !isPhaseTwo)
        {
            isPhaseTwo = true;
            TransactionToState(EnumTypes.STATE.PATTERN_PHASE);
        }
    }
    public override void TransactionToState(EnumTypes.STATE state, object data = null)
    {
        
        if (state == EnumTypes.STATE.ATTACK)
        {
            BossAttackState attack;
            if(TryGetComponent<BossAttackState>(out attack))
            {
                Vector3 pPos = player.transform.position;
                Vector3 bPos = transform.position;
                pPos.y = 0;
                bPos.y = 0;
                Vector3 targetPos = (pPos - bPos).normalized;
                if(targetPos == Vector3.zero)
                {
                    targetPos = transform.forward;
                }

                attack.DashDir = targetPos;
            }
        }
        base.TransactionToState(state, data);
    }
    // 스킬 결정 후, 변경
    public bool CheckSkillReady()
    {
        List<EnemySkillAttack> readySkills = new List<EnemySkillAttack>();
        // 스킬 사용중이 아니라면
        foreach (var skill in skillList)
        {
            // 준비된 스킬에 추가
            if (skill.IsReady && GetPlayerDis() <= skill.AttackRange )
            {
                readySkills.Add(skill);
            }
        }

        if (readySkills.Count > 0)
        {
            int rnd = Random.Range(0, readySkills.Count);
            //currentSkill = readySkills[rnd];
            CurrentSkill = skillList[2];
            int skillIndex = skillList.IndexOf(CurrentSkill);
            Animator.SetInteger("Skill", 2);
            CurrentSkill.SetLastTime();
            return true;
        }
        return false;
    }
}
