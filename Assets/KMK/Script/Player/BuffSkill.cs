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
        effect = Instantiate(hitEffectPrefab, transform.position, hitEffectPrefab.transform.rotation);
        effect.transform.SetParent(this.transform);
        effect.transform.localPosition = Vector3.zero;
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
        Destroy(effect);
    }

}
