using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class PlayerStatComponent : CharacterStatComponent
{
    private PlayerStatInfo playerInfo;
    private float currentST;
    public float CurrentExp { get; private set; }
    public int CurrentLevel { get; private set; } = 1;

    public float CurrentST { get => currentST; }
    public float MaxST { get => playerInfo.maxST; }
    public float CriticalMultifle { get => playerInfo.criticalMultifle; }

    public Action<float, float> OnChangeST;
    public Action<float, float> OnChangeExp;
    public Action<int> OncChangeLevel;
    protected override void Awake()
    {
        GameManager.Instance.OnBindPlayer(this);
        playerInfo = statinfo as PlayerStatInfo;
        if (playerInfo == null) Debug.Log($"playerinfo ¾øÀ½");
        base.Awake();
        
        InitUI();
    }
    private void InitUI()
    {
        currentST = MaxST;
        CurrentExp = 0;
        OnChangeExp?.Invoke(CurrentExp, playerInfo.exp[0]);
        GameManager.Instance.OnDieEnemy += TakeExp;
    }
    private void Start()
    {
        OncChangeLevel?.Invoke(CurrentLevel);
    }
    public bool InvokeCri()
    {
        return Random.value < playerInfo.criticalChance;
    }

    public void TakeExp(float amount)
    {
        CurrentExp += amount;
        float maxExp = playerInfo.exp[CurrentLevel - 1];
        OnChangeExp?.Invoke(CurrentExp, maxExp);
        if(CurrentExp >= maxExp)
        {
            LevelUp(maxExp);
        }
    }

    private void LevelUp(float usedExp)
    {
        CurrentExp -= usedExp;
        CurrentLevel++;
        OnChangeExp?.Invoke(CurrentExp, playerInfo.exp[CurrentLevel - 1]);
        OncChangeLevel?.Invoke(CurrentLevel);
    }
    private void OnDisable()
    {
        GameManager.Instance.OnUnBindPlayer(this);
        GameManager.Instance.OnDieEnemy -= TakeExp;
    }
}
