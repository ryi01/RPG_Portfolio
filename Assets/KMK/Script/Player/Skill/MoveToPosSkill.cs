using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MoveToPosSkill : PlayerSkillAttack
{

    public override void Attack()
    {
        Vector3 evadeDir = pc.LockedAimDir;

        if (evadeDir.sqrMagnitude < 0.001f) return;
        float distance = Vector3.Distance(transform.position, pc.AimPoint);

        if (distance > skillInfo.attackMaxRange)
        {
            distance = skillInfo.attackMaxRange;
        }

        StartCoroutine(ExcuteEvade(evadeDir, distance));
    }

    IEnumerator ExcuteEvade(Vector3 dir, float dist)
    {
        pc.IsBlink = true;
        pc.MovementComp.StopMove();
        pc.MovementComp.LookAtInstant(dir);
        int playerLayer = pc.gameObject.layer;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if(enemyLayer != -1)
        {
            Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }

        StartSkill();

        pc.CameraShakeController.PlayMotionBlur(0.75f, skillInfo.attackTime);
        pc.MovementComp.Push(dir, dist, skillInfo.attackTime, false, true);
        pc.CameraShakeController.GenerateImpulseDirection(dir, 0.4f);

        yield return new WaitForSeconds(skillInfo.attackTime);
        if (enemyLayer != -1)
        {
            Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }
        pc.IsBlink = false;
    }

}
