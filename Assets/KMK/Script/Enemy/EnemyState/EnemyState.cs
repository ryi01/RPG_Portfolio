using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState : MonoBehaviour
{
    protected EnemyController controller;
    protected EnemyStatComponent statComp;
    protected Animator Anim { get => controller.Animator; }

    [SerializeField]private EnumTypes.STATE stateType;
    public EnumTypes.STATE StateType { get => stateType; }

    // 애니메이션 재생속도
    [Range(1f, 2f)]
    [SerializeField] protected float animSpeed;
    [Range(0f, 1f)]
    [SerializeField] protected float clipVolume = 1;
    [SerializeField] protected AudioClip clip;


    protected ParticleSystem effect;
    public virtual void Intialize(EnemyController owner)
    {
        controller = owner;
        statComp = owner.StatComp;
    }
    public virtual void EnterState(EnumTypes.STATE state, object data = null)
    {
        Anim.speed = animSpeed;  
    }

    public abstract void UpdateState();
    public virtual void ExitState() { }
    protected void SetEffect(ParticleSystem particle)
    {
        effect = particle;
    }
    protected virtual void PlayEffect()
    {
        if (effect != null)
        {
            effect.Play();
        }
    }
    protected virtual void StopPlayEffect(bool isClear = false)
    {
        if (effect != null)
        {
            if(isClear) effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            else effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public virtual void PlaySFX()
    {
        if (clip != null)
        {
            GameManager.Instance.SoundManager.PlayImpactSFX(clip, clipVolume);
        }
    }
    protected virtual bool CheckDeath()
    {
        if(controller.StatComp.CurrentHP <= 0)
        {
            controller.TransitionToState(EnumTypes.STATE.DEATH);
            return true;
        }
        return false;   
    }
    protected void LookAtTarget(float rotateSpeed = -1)
    {
        if (controller == null || controller.Player == null) return;
        LookAtPosition(controller.Player.transform.position, rotateSpeed);
    }
    protected void LookAtPosition(Vector3 targetPos, float rotateSpeed = -1f)
    {
        Vector3 dir = GetFlatDir(targetPos);
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        float speed = (rotateSpeed > 0) ? rotateSpeed : statComp.RotSpeed;

        controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, lookRot, Time.deltaTime * speed);
    }
    protected Vector3 GetFlatDir(Vector3 targetPos)
    {
        Vector3 dir = targetPos - controller.transform.position;
        dir.y = 0;
        return dir.normalized;
    }

    protected void SnapLookAtPosition(Vector3 target)
    {
        Vector3 dir = GetFlatDir(target);
        if (dir.sqrMagnitude < 0.001f) return;
        controller.transform.rotation = Quaternion.LookRotation(dir);
    }

    protected bool HasAnyAvailableSkillInRange(float dis)
    {
        if (controller.BossSkill == null || controller.BossSkill.SkillList == null) return false;

        foreach (var skill in controller.BossSkill.SkillList)
        {
            if (skill == null || !skill.IsReady) continue;

            if (skill is BossSummonSkillAttack)
            {
                if (controller.BossSummon == null || !controller.BossSummon.CanSummon())
                    continue;
            }

            if (dis >= skill.AttackMinRange && dis <= skill.AttackMaxRange)
                return true;
        }

        return false;
    }
}
