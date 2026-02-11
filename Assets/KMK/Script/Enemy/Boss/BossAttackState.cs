using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : EnemyAttackState
{
    [SerializeField] private List<EnemySkillAttack> skillList;
    private EnemySkillAttack currentSkill;
    private Vector3 dashDir;
    public Vector3 DashDir { get => dashDir; set => dashDir = value; }

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        SelectSkill();
    }
    public override void UpdateState()
    {
        if (IsAttack)
        {
            if(currentSkill is BossDashSkillAttack dashAttack)
            {
                controller.StatComp.SetSpeedMultifle(2);
                NavigationStop();
                Vector3 targetPos = transform.position + DashDir * 10f;
                navMeshAgent.SetDestination(targetPos);
            }
            return;
        }
        controller.StatComp.SetSpeedMultifle(1);
        LookAtTarget();
        controller.TransactionToState(EnumTypes.STATE.IDLE);
    }

    private void SelectSkill()
    {
        List<EnemySkillAttack> readySkills = new List<EnemySkillAttack>();
        // 스킬 사용중이 아니라면
        foreach(var skill in skillList)
        {
            // 준비된 스킬에 추가
            if(skill.IsReady)
            {
                readySkills.Add(skill);
            }
        }

        if (readySkills.Count > 0)
        {
            int rnd = Random.Range(0, readySkills.Count);
            //currentSkill = readySkills[rnd];
            currentSkill = skillList[2];
            int skillIndex = skillList.IndexOf(currentSkill);
            Anim.SetInteger("Skill", 2);
            currentSkill.SetLastTime();
        }
        else
        {
            controller.TransactionToState(EnumTypes.STATE.IDLE);
        }
    }
}
