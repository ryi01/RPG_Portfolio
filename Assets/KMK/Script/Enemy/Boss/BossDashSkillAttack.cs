using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class BossDashSkillAttack : EnemySkillAttack
{
    public void OnDashAttack()
    {
        IsSkill = true;
    }
}
