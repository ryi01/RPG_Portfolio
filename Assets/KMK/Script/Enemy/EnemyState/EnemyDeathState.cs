using UnityEngine;

public class EnemyDeathState : EnemyHitState
{
    protected float time = 0;
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
    }

    public override void UpdateState()
    {
        time += Time.deltaTime;
        if(time >= fsmInfo.DeathDelayTime)
        {
            ExitState();
        }
    }

    public override void ExitState()
    {

        Destroy(gameObject);
    }
}
