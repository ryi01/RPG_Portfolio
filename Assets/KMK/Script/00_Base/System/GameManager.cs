using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum GameState { Pause, Start, Town, Dungeon, BossPhase, Dialogue }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private StartUI startUI;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private EnemyUIManager enemyUIManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private PortalSpawner portalSpawner;
    [SerializeField] private SQLiteManager sQLiteManager;

    public PlayerStatComponent Player { get; private set; }

    public SQLiteManager SQLiteManager => sQLiteManager;
    public DataManager DataManager => dataManager;
    public StartUI StartUI => startUI;
    public UIManager UIManager => uiManager;
    public EnemyUIManager EnemyUIManager => enemyUIManager;
    public QuestManager QuestManager => questManager;
    public DialogueSystem DialogueSystem => dialogueSystem;
    public SoundManager SoundManager => soundManager;
    public PortalSpawner PortalSpawner => portalSpawner;

    public GameState CurrentState {  get; private set; }

    public Action<int> OnDieEnemy;

    private GameState returnStateFromTitle = GameState.Town;
    private GameState preState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sQLiteManager.InitializeDatabase();
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        ChangeState(GameState.Start);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            TogglePause();
        }
    }
    #region UI
    public void OnBindPlayer(PlayerStatComponent player)
    {
        Player = player;
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

    public void SendEnemyKilled(int exp)
    {
        OnDieEnemy?.Invoke(exp);
    }

    public void ChangeState(GameState state)
    {
        CurrentState = state;
        UpdateBGMbyState(state);
    }
    public void UpdateBGMbyState(GameState state)
    {
        switch (state)
        {
            case GameState.Start:
                SoundManager.PlayBGM(EBGMType.MAIN_MENU);
                break;
            case GameState.Town:
                SoundManager.PlayBGM(EBGMType.TOWN);
                break;
            case GameState.Dungeon:
                SoundManager.PlayBGM(EBGMType.DUNGEON);
                break;
            case GameState.BossPhase:
                SoundManager.PlayBGM(EBGMType.BOSS_BATTLE);
                break;
            case GameState.Pause:
            case GameState.Dialogue:
                break;
        }
    }
    #endregion
    #region 포탈 관련
    public void SpawnPortal(string sceneName, Vector3 pos)
    {
        if (portalSpawner == null) return;
        portalSpawner.SpawnPortal(sceneName, pos);
    }
    #endregion
    #region 시작
    public void EnterTitleMenu()
    {
        if(CurrentState != GameState.Start && CurrentState != GameState.Pause && CurrentState != GameState.Dialogue)
        {
            returnStateFromTitle = CurrentState;
        }
        else if(preState == GameState.Town || preState == GameState.Dungeon || preState == GameState.BossPhase)
        {
            returnStateFromTitle = preState;
        }
        Time.timeScale = 1f;
        ChangeState(GameState.Start);
    }
    public void StartGameFromTitle()
    {
        ChangeState(returnStateFromTitle);
    }
    #endregion
    #region 일시정지
    public void TogglePause()
    {
        if (CurrentState == GameState.Pause)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }    
    public void SaveAndQuitGame()
    {
        SaveCurrentPlayerData();
        Time.timeScale = 0;
        Application.Quit();
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Pause) return;
        preState = CurrentState;
        ChangeState(GameState.Pause);
        startUI.SetPauseCanvas(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        ChangeState(preState);
        Time.timeScale = 1;
        startUI.SetPauseCanvas(false);
    }
    public void SaveCurrentPlayerData()
    {
        if (Player == null) return;

        Player.SyncToSaveData();
        DataManager.SaveData();
    }
    #endregion
}
