using UnityEngine;

public class EnemyIdleState : EnemyState
{
    protected float time;
    [SerializeField] protected float checkTime;
    [SerializeField] protected Vector2 checkTimeRange;

    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        StandWait();
        controller.NavigationStop();
        // 애니메이션 호출 => 컨트롤러에 존재
        Anim.SetInteger("State", (int)state);
    }
    public override void UpdateState()
    {
        time += Time.deltaTime;
        float dis = controller.GetPlayerDis();
        if(dis <= fsmInfo.AttackRange)
        {
            controller.TransactionToState(EnumTypes.STATE.ATTACK);
            return;
        }
        if(dis <= fsmInfo.DetectRange)
        {
            controller.TransactionToState(EnumTypes.STATE.DETECT);
            return;
        }
        if(time > checkTime)
        {
            int selet = Random.Range(0, 2);
            switch(selet)
            {
                case 0:
                    time = 0;
                    StandWait();
                    break;
                case 1:
                    controller.TransactionToState(EnumTypes.STATE.WANDER);
                    return;
            }
        }
    }

    private void StandWait()
    {
        time = 0;
        checkTime = Random.Range(checkTimeRange.x, checkTimeRange.y);
    }
}
