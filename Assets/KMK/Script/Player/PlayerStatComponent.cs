using UnityEngine;

public class PlayerStatComponent : CharacterStatComponent
{
    private PlayerStatInfo playerInfo;
    private float currentST;

    public float CurrentST { get => currentST; }
    public float MaxST { get => playerInfo.maxST; }
    public float CriticalMultifle { get => playerInfo.criticalMultifle; }
    protected override void Awake()
    {
        base.Awake();
        playerInfo = statinfo as PlayerStatInfo;
        if (playerInfo == null) Debug.Log($"playerinfo ¾øÀ½");
    }

    public bool InvokeCri()
    {
        return Random.value < playerInfo.criticalChance;
    }

    public bool UseST(float amount)
    {
        if (currentST < amount) return false;
        currentST -= amount;
        return true;
    }
    public void ReganST(float deltaTime)
    {
        currentST = Mathf.Clamp(currentST + playerInfo.regenST * deltaTime, 0, playerInfo.maxST);
    }
}
