using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BuffSkill : PlayerSkillAttack
{
    private bool isBuff = false;
    private GameObject effect;
    public override void Attack()
    {
        if (isBuff) return;
        PlayEffect();
        StartCoroutine(BuffRoutine());
    }
    IEnumerator BuffRoutine()
    {
        isBuff = true;
        StartSkill();
        CS.attackBuffMultifle = skillInfo.attackMultifle;
        yield return new WaitForSeconds(skillInfo.attackTime);
        CS.attackBuffMultifle = 1.0f;
        isBuff = false;
        StopPlayEffect();
    }

}
