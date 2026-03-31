using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using Unity.VisualScripting;


public class PlayerStatComponent : CharacterStatComponent
{
    [SerializeField] private ParticleSystem levelUpEffectPrefab;
    [SerializeField] private ParticleSystem hpEffectPrefab;
    [SerializeField] private ParticleSystem coinEffectPrefab;
    [SerializeField] private GoldSystem goldSystem;
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

    private Coroutine currentEffectCoroutine;
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
        OnChangeExp?.Invoke(CurrentExp, playerInfo.requiredExpByLevel[0]);
        GameManager.Instance.OnDieEnemy += TakeExp;
    }
    private void OnEnable()
    {
        goldSystem.OnGoldChanged += GetGold;
    }
    private void Start()
    {
        OncChangeLevel?.Invoke(CurrentLevel);
    }
    public bool InvokeCri()
    {
        return Random.value < playerInfo.criticalChance;
    }
    public void RecoveryHP(float recovery)
    {
        currentHP = Mathf.Clamp(currentHP + recovery, 0, statinfo.maxHP);
        if (currentEffectCoroutine != null) StopCoroutine(currentEffectCoroutine);
        currentEffectCoroutine = StartCoroutine(EffectCoroutine(hpEffectPrefab, 0.3f));
        OnHpChanged?.Invoke(currentHP, statinfo.maxHP);
    }
    public void TakeExp(float amount)
    {
        CurrentExp += amount;
        float maxExp = playerInfo.requiredExpByLevel[CurrentLevel - 1];
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
        OnChangeExp?.Invoke(CurrentExp, playerInfo.requiredExpByLevel[CurrentLevel - 1]);
        if (currentEffectCoroutine != null) StopCoroutine(currentEffectCoroutine);
        currentEffectCoroutine = StartCoroutine(EffectCoroutine(levelUpEffectPrefab, 0.5f));
        OncChangeLevel?.Invoke(CurrentLevel);
    }
    private void GetGold(int gold)
    {
        if (currentEffectCoroutine != null) StopCoroutine(currentEffectCoroutine);
        currentEffectCoroutine = StartCoroutine(EffectCoroutine(coinEffectPrefab, 0.5f));
    }
    private void OnDisable()
    {
        GameManager.Instance.OnUnBindPlayer(this);
        GameManager.Instance.OnDieEnemy -= TakeExp;
        goldSystem.OnGoldChanged -= GetGold;
    }
    private IEnumerator EffectCoroutine(ParticleSystem effect, float duration)
    {
        effect.Play();
        yield return new WaitForSeconds(duration);
        effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
