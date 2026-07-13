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
    [SerializeField]
    [Range(0f, 1f)] protected float levelUpClipVolume = 0.75f;
    [SerializeField] private AudioClip levelupClip;
    private PlayerStatInfo playerInfo;

    public int CurrentExp { get; private set; }
    public int CurrentLevel { get; private set; } = 1;
    public float MaxST { get => playerInfo.maxST; }
    public float CriticalMultifle { get => playerInfo.criticalMultifle; }

    public Action<float, float> OnChangeST;
    public Action<float, float> OnChangeExp;
    public Action<int> OnChangeLevel;

    private Coroutine currentEffectCoroutine;
    protected override void Awake()
    {
        playerInfo = statinfo as PlayerStatInfo;
        if (playerInfo == null) Debug.Log($"playerinfo 없음");
        base.Awake();

        GameManager.Instance.OnBindPlayer(this);
        ApplySaveData();
    }
    private void Start()
    {
        RefreshUI();
    }
    private void RefreshUI()
    {
        int expIndex = Mathf.Clamp(CurrentLevel - 1, 0, playerInfo.requiredExpByLevel.Count - 1);

        OnHpChanged?.Invoke(CurrentHP, MaxHP);
        OnChangeExp?.Invoke(CurrentExp, playerInfo.requiredExpByLevel[expIndex]);
        OnChangeLevel?.Invoke(CurrentLevel);
    }
    private void OnEnable()
    {
        goldSystem.OnGoldChanged += GetGold;
        GameManager.Instance.OnDieEnemy += TakeExp;
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
    public void TakeExp(int amount)
    {
        CurrentExp += amount;
        int maxExp = playerInfo.requiredExpByLevel[CurrentLevel - 1];
        OnChangeExp?.Invoke(CurrentExp, maxExp);
       
        if(CurrentExp >= maxExp)
        {
            LevelUp(maxExp);
        }
    }

    private void LevelUp(int usedExp)
    {
        CurrentExp -= usedExp;
        CurrentLevel++;
        GameManager.Instance.SoundManager.PlayImpactSFX(levelupClip, levelUpClipVolume);
        OnChangeExp?.Invoke(CurrentExp, playerInfo.requiredExpByLevel[CurrentLevel - 1]);
        if (currentEffectCoroutine != null) StopCoroutine(currentEffectCoroutine);
        currentEffectCoroutine = StartCoroutine(EffectCoroutine(levelUpEffectPrefab, 0.5f));
        OnChangeLevel?.Invoke(CurrentLevel);
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
    // 저장된 데이터 불러오기
    private void ApplySaveData()
    {
        if (GameManager.Instance.DataManager == null) return;
        var data = GameManager.Instance.DataManager.PlayerData;
        if (data == null) return;
        float hp = data.CurrentHP;
        if(hp <=0f)
        {
            hp = MaxHP;
        }
        SetCurrentHP(hp);
        CurrentLevel = data.Level;
        CurrentExp = data.CurrentExp;
    }
    // 데이터 저장하기
    public void SyncToSaveData()
    {
        if (GameManager.Instance.DataManager == null) return;
        DataManager datamanager = GameManager.Instance.DataManager;

        datamanager.SetLevel(CurrentLevel);
        datamanager.SetCurrentHP(CurrentHP);
        datamanager.SetCurrentExp(CurrentExp);
    }
    public void ReviveFull()
    {
        isDead = false;
        CurrentHP = MaxHP;
        IsHit = false;

        OnHpChanged?.Invoke(CurrentHP, MaxHP);
    }
}
