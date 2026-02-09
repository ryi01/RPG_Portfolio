using UnityEngine;


public class PlayerStatComponent : CharacterStatComponent
{
    private PlayerStatInfo playerInfo;
    private float currentST;
    private float lastSTUsedTime;
    [SerializeField] private float regenDely = 0.5f;

    public float CurrentST { get => currentST; }
    public float MaxST { get => playerInfo.maxST; }
    public float CriticalMultifle { get => playerInfo.criticalMultifle; }
    protected override void Awake()
    {
        base.Awake();
        playerInfo = statinfo as PlayerStatInfo;
        if (playerInfo == null) Debug.Log($"playerinfo ¾øÀ½");
        currentST = MaxST;
    }

    public bool InvokeCri()
    {
        return Random.value < playerInfo.criticalChance;
    }

    public bool UseST(float amount)
    {
        if (currentST < amount) return false;
        currentST -= amount;
        lastSTUsedTime = Time.time;
        return true;
    }
    public void ReganST(float deltaTime)
    {
        if (Time.time > lastSTUsedTime + regenDely && currentST < MaxST)
        {
            currentST = Mathf.Clamp(currentST + playerInfo.regenST * deltaTime, 0, MaxST);
        }
    }
}
