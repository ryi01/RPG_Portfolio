using UnityEngine;

public class BossChangePhaseState : EnemyState
{
    private int jumpCount = 0;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        controller.NavigationStop();
        Anim.SetInteger("State", 10);
        Anim.SetTrigger("Phase");
    }
    public override void UpdateState()
    {
        controller.NavMeshAgent.speed = controller.StatComp.SetSpeedMultifle(3);
    }

    public void OnJumpEnd()
    {
        jumpCount++;
        if(jumpCount < 2)
        {
            Anim.SetTrigger("Phase");
        }
        else
        {
            UpdateState();
        }
    }
    public void OnChangeEndPhase()
    {
        controller.TransactionToState(EnumTypes.STATE.IDLE);
    }
}
