using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

// 보스 스킬 만들기
public class BossController : EnemyController
{
    [SerializeField] private EnemySkillAttack[] skillList;
    public int LastSkillIndex { get; set; } = -1;
    private bool isPhaseTwo = false;
    public bool IsPhaseTwo { get => isPhaseTwo; }
    public EnemySkillAttack[] SkillList { get => skillList; }
    protected override void Update()
    {
        base.Update();

        float hpRatio = StatComp.CurrentHP / StatComp.MaxHP;
        if(hpRatio < 0.4f && !isPhaseTwo)
        {
            isPhaseTwo = true;
            StatComp.SetSpeedMultifle(2);
            TransactionToState(EnumTypes.STATE.PATTERN_PHASE);
        }
    }
}
