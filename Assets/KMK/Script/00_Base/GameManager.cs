using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InventroySystem inventroySystem;
    [SerializeField] private DataManager dataManager;
    public UIManager UIManager => uiManager;
    public InventroySystem InventroySystem => inventroySystem;
    public DataManager DataManager => dataManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnBindPlayer(PlayerStatComponent player)
    {
        uiManager.BindPlayerUI(player);
    }
    public void OnUnBindPlayer(PlayerStatComponent player)
    {
        uiManager.UnBindPlayerUI(player);
    }
    public void OnBindEnemy(CharacterStatComponent enemy, float y = 2.5f)
    {
        uiManager.CreateEnemyHPBar(enemy, y);
    }
    public void OnUnBindEnemy(CharacterStatComponent enemy)
    {
        uiManager.UnBindEnemyUI(enemy);
    }

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
}
