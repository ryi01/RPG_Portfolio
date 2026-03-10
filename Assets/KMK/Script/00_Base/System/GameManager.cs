using System;
using UnityEngine;

public enum GameState { Pause, Playing, Town, Dungeon, BossPhase, Dialogue }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InventroySystem inventroySystem;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private GoldSystem goldSystem;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private SceneLoadManager sceneLoadManager;
    [SerializeField] private QuestManager questManager;
    [SerializeField] private EnemyUIManager enemyUIManager;
    [SerializeField] private GameObject protalPrefab;
    [SerializeField] private CameraEnviroment cameraEnviroment;

    public UIManager UIManager => uiManager;
    public InventroySystem InventroySystem => inventroySystem;
    public DataManager DataManager => dataManager;
    public GoldSystem GoldSystem => goldSystem;

    public DungeonGenerator DungeonGenerator => dungeonGenerator;

    public SceneLoadManager SceneLoadManager => sceneLoadManager;
    public QuestManager QuestManager => questManager;

    public DialogueSystem DialogueSystem => dialogueSystem;
    public EnemyUIManager EnemyUIManager => enemyUIManager;

    public CameraEnviroment CameraEnviroment => cameraEnviroment;

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
        CurrentState = GameState.Town;
        cameraEnviroment.ChnageToTown();
        BindGoldSystem();
    }
    private void OnDestroy()
    {
        UnBindGoldSystem();
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
    public void OnBindEnemy(CharacterStatComponent enemy, float y = 2.5f)
    {
        EnemyUIManager.CreateEnemyHPBar(enemy, y);
    }
    public void OnUnBindEnemy(CharacterStatComponent enemy)
    {
        EnemyUIManager.UnBindEnemyUI(enemy);
    }
    #endregion
    #region 인벤토리
    public void SwapItem(int a, int b)
    {
        inventroySystem.SwapItems(a, b);
    }
    public void OpenBoxInfo(ItemBox box)
    {
        inventroySystem.CurrentBox = box;
        UIManager.OpenItemBox(box);
    }
    public void CloseBoxInfo()
    {
        inventroySystem.CurrentBox = null;
    }
    #endregion
    #region 데이터 관련
    public void BindGoldSystem()
    {
        goldSystem.OnChangedGold += DataManager.ChangeGold;
        goldSystem.OnChangedGold += UIManager.ChangeGold;
    }

    public void UnBindGoldSystem()
    {
        goldSystem.OnChangedGold -= DataManager.ChangeGold;
        goldSystem.OnChangedGold -= UIManager.ChangeGold;
    }

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
        GameObject existingPortal = GameObject.FindGameObjectWithTag("Portal");
        if (existingPortal != null)
        {
            Destroy(existingPortal);
        }
        if (protalPrefab != null)
        {
            GameObject go = Instantiate(protalPrefab, pos, Quaternion.identity);
            go.tag = "Portal";
            if (go.TryGetComponent<Portal>(out Portal portal))
            {
                portal.ChangeTargetSceneName(sceneName);
            }
        }
    }
    public void ChangeScene(string unloadSceneName, string loadSceneName)
    {
        bool isDungeon = loadSceneName.Contains("Game");
        CurrentState = isDungeon ? GameState.Dungeon : GameState.Town;
        if (isDungeon) cameraEnviroment.ChangeToDungeon();
        else cameraEnviroment.ChnageToTown();
        IsPathFindingEnable = isDungeon;
        StartCoroutine(SceneLoadManager.ChangeSceneCor(unloadSceneName, loadSceneName));
    }

    public bool IsPathFindingEnable { get; private set; } = false;
   
    #endregion
}
