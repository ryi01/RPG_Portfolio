using System;
using UnityEngine;

public class BossPhaseComponent : MonoBehaviour
{
    public event Action OnPhaseTwoStarted;

    [SerializeField] private float phaseTwoHpRatio = 0.4f;

    private EnemyController controller;

    public bool IsPhaseTwo { get; private set; }
    public bool RequestPhaseTwo {  get; private set; }
    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (controller == null || controller.StatComp == null) return;
        if (IsPhaseTwo || RequestPhaseTwo) return;
        if (controller.CurrentState != null && controller.CurrentState.StateType == EnumTypes.STATE.DEATH) return;
        float hpRatio = controller.StatComp.CurrentHP / controller.StatComp.MaxHP;

        if (hpRatio <= phaseTwoHpRatio)
        {
            RequestPhaseTwo = true;
            controller.TransitionToState(EnumTypes.STATE.PATTERN_PHASE);
        }
    }

    public void CompletePhaseTwo()
    {
        if (IsPhaseTwo) return;
        IsPhaseTwo = true;
        RequestPhaseTwo = false;
        OnPhaseTwoStarted?.Invoke();
    }

}
