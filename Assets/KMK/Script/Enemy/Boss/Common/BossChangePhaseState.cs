using UnityEngine;

public class BossChangePhaseState : EnemyState
{
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        Debug.Log("페이지 변경");
        controller.NavigationStop();
        Anim.SetInteger("State", 10);
        Anim.SetTrigger("Phase");
    }
    public override void UpdateState()
    {
        controller.NavMeshAgent.speed = controller.StatComp.SetSpeedMultifle(3);
    }

    public void OnChangeEndPhase()
    {
        controller.TransitionToState(EnumTypes.STATE.IDLE);
    }
}
