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
            Vector3 evadeDir = pc.LockedAimDir;

            if (evadeDir.sqrMagnitude < 0.001f) return;
            float distance = Vector3.Distance(transform.position, pc.AimPoint);

            if(distance > skillInfo.attackMaxRange)
            {
                distance = skillInfo.attackMaxRange;
            }
            
            StartCoroutine(ExcuteEvade(evadeDir, distance));
        }
    }

    IEnumerator ExcuteEvade(Vector3 dir, float dist)
    {
        pc.IsBlink = true;
        pc.MovementComp.StopMove();
        pc.MovementComp.LookAtInstant(dir);

        StartSkill();

        pc.CameraShakeController.PlayMotionBlur(0.75f, skillInfo.attackTime);
        pc.MovementComp.Push(dir, dist, skillInfo.attackTime, false, true);
        pc.CameraShakeController.GenerateImpulseDirection(dir, 0.4f);

        yield return new WaitForSeconds(skillInfo.attackTime);

        pc.IsBlink = false;
    }

}
