using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MoveToPosSkill : PlayerSkillAttack
{

    public override void Attack()
    {
        float evadeCost = 25f;
        
        if (pc.StatComp.CurrentST >= evadeCost)
        {
            Vector3 mouseWorldPos = pc.MovementComp.GetMouseWorldPos();
            Vector3 evadeDir = mouseWorldPos - transform.position;
            evadeDir.y = 0;

            float distance = evadeDir.magnitude;
            Vector3 dir = evadeDir.normalized;
            if(distance > skillInfo.attackMaxRange)
            {
                distance = skillInfo.attackMaxRange;
            }
            
            StartCoroutine(ExcuteEvade(dir, distance));
        }
    }

    IEnumerator ExcuteEvade(Vector3 dir, float dist)
    {
        pc.IsBlink = true;
        StartSkill();
        pc.MovementComp.LookAtInstant(dir);
        pc.MovementComp.Push(dir, dist, skillInfo.attackTime);
        yield return new WaitForSeconds(skillInfo.attackTime);
        pc.IsBlink = false;
    }

}
