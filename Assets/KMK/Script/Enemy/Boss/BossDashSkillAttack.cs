using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossDashSkillAttack : EnemySkillAttack
{
    public void OnDashAttack()
    {
        IsSkill = true;
    }

}
