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

    protected virtual bool CheckDeath()
    {
        if(controller.StatComp.CurrentHP <= 0)
        {
            controller.TransitionToState(EnumTypes.STATE.DEATH);
            return true;
        }
        return false;   
    }

}
