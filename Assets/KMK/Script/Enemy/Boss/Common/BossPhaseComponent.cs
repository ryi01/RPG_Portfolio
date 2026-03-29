using System;
using UnityEngine;

public class BossPhaseComponent : MonoBehaviour
{
    public event Action OnPhaseTwoStarted;
    [SerializeField] private float phaseTwoHpRatio = 0.4f;
    [SerializeField] private float phaseTwoSpeedMultiplier = 2f;

    private EnemyController controller;
    public bool IsPhaseTwo { get; private set; }

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private void Update()
    {
        if (controller == null || controller.StatComp == null) return;
        if (controller.CurrentState != null && controller.CurrentState.StateType == EnumTypes.STATE.DEATH) return;
        float hpRatio = controller.StatComp.CurrentHP / controller.StatComp.MaxHP;
        if (!IsPhaseTwo && hpRatio < phaseTwoHpRatio) EnterPhaseTwo();
    }


    private void EnterPhaseTwo()
    {
        IsPhaseTwo = true;
        controller.NavigationStop();
        controller.StatComp.SetSpeedMultifle(phaseTwoSpeedMultiplier);
        controller.TransitionToState(EnumTypes.STATE.PATTERN_PHASE);
        OnPhaseTwoStarted?.Invoke();
        if (controller.BossLightning != null) controller.BossLightning.StartPattern();
    }

}
