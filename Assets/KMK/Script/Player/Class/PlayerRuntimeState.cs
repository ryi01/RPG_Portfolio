using System;
[Serializable]
public class PlayerRuntimeState
{
    public float CurrentHP;
    public float SpeedMultiplier = 1f;
    public float AttackBuffMultiplier = 1f;
    public bool IsHit;
}
