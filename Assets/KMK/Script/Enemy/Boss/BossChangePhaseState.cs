using UnityEngine;

public class BossChangePhaseState : EnemyState
{
    private int jumpCount = 0;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        NavigationStop();
        Anim.SetInteger("State", 10);
        Anim.SetTrigger("Phase");
    }
    public override void UpdateState()
    {
        Debug.Log($"페이지 체인지");
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
}
