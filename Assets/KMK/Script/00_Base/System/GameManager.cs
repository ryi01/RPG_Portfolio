using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum GameState { Pause, Playing, Town, Dungeon, BossPhase, Dialogue }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private EnemyUIManager enemyUIManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private PortalSpawner portalSpawner;

    public UIManager UIManager => uiManager;
    public EnemyUIManager EnemyUIManager => enemyUIManager;
    public QuestManager QuestManager => questManager;
    public DialogueSystem DialogueSystem => dialogueSystem;
    public SoundManager SoundManager => soundManager;
    public PortalSpawner PortalSpawner => portalSpawner;

    public GameState CurrentState {  get; private set; }

    public Action<float> OnDieEnemy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        ChangeState(GameState.Town);
    }

    #region UI
    public void OnBindPlayer(PlayerStatComponent player)
    {
        UIManager.BindPlayerUI(player);
    }
    public void OnUnBindPlayer(PlayerStatComponent player)
    {
        UIManager.UnBindPlayerUI(player);
    }

    public void BindBoss(CharacterStatComponent enemy)
    {
        EnemyUIManager.ShowBossHP(enemy);
    }
    public void OnBindEnemy(CharacterStatComponent enemy, float y = 2.5f)
    {
        EnemyUIManager.CreateEnemyHPBar(enemy, y);
    }
    public void OnUnBindEnemy(CharacterStatComponent enemy)
    {
        EnemyUIManager.UnBindEnemyUI(enemy);
    }
    public void OnUnBindBoss(CharacterStatComponent enemy)
    {
        EnemyUIManager.UnBindBoss(enemy);
    }
    #endregion
    #region 데이터 관련

    public void SendEnemyKilled(float exp)
    {
        OnDieEnemy?.Invoke(exp);
    }

    public void ChangeState(GameState state)
    {
        CurrentState = state;
    }
    #endregion
    #region 포탈 관련
    public void SpawnPortal(string sceneName, Vector3 pos)
    {
        if (portalSpawner == null) return;
        portalSpawner.SpawnPortal(sceneName, pos);
    }  
    #endregion
}
