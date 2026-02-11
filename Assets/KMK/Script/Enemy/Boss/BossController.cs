using UnityEngine;

// 보스 스킬 만들기
public class BossController : EnemyController
{
    private bool isPhaseTwo = false;
    protected override void Update()
    {
        base.Update();

        float hpRatio = StatComp.CurrentHP / StatComp.MaxHP;
        if(hpRatio < 0.4f && !isPhaseTwo)
        {
            isPhaseTwo = true;
            TransactionToState(EnumTypes.STATE.PATTERN_PHASE);
        }
    }
    public override void TransactionToState(EnumTypes.STATE state, object data = null)
    {
        base.TransactionToState(state, data);
        if (state == EnumTypes.STATE.ATTACK)
        {
            BossAttackState attack;
            if(TryGetComponent<BossAttackState>(out attack))
            {
                Vector3 targetPos = player.transform.position - transform.position;
                targetPos.y = 0;
                attack.DashDir = targetPos.normalized;
            }
        }
    }

}
