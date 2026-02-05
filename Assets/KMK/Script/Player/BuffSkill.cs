using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BuffSkill : PlayerSkillAttack
{
    private bool isBuff = false;    
    public override void Attack()
    {
        if (isBuff) return;
        StartCoroutine(BuffRoutine());
    }
    IEnumerator BuffRoutine()
    {
        isBuff = true;
        StartSkill();
        CS.attackBuffMultifle = skillInfo.attackMultifle;
        EndSkill();
        yield return new WaitForSeconds(skillInfo.attackTime);
        CS.attackBuffMultifle = 1.0f;
        isBuff = false;

    }

}
