using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private StatUI playerHUD;
    [SerializeField] private GameObject enemyHPBarPrefab;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ItemBoxUI itemBoxUI;
    private StatUI enemyStatUI;

    private void Start()
    {
        inventoryUI.InitInventoryUI();
    }

    public void BindPlayerUI(PlayerStatComponent player)
    {
        player.OnHpChanged += playerHUD.UpdateHP;
        playerHUD.UpdateHP(player.CurrentHP, player.MaxHP);
/*        player.OnChangeST += playerHUD.UpdateST;
        playerHUD.UpdateST(player.CurrentST, player.MaxST);*/
    }

    public void CreateEnemyHPBar(CharacterStatComponent enemyStat, float y = 2.5f)
    {
        GameObject go = Instantiate(enemyHPBarPrefab, enemyStat.transform);
        go.transform.localPosition = new Vector3(0, y, 0);
        enemyStatUI = go.GetComponent<StatUI>();
        enemyStat.OnHpChanged += enemyStatUI.UpdateHP;
        enemyStatUI.UpdateHP(enemyStat.CurrentHP, enemyStat.MaxHP);
    }
    
    public void UnBindPlayerUI(PlayerStatComponent player)
    {
        player.OnHpChanged -= playerHUD.UpdateHP;
        //player.OnChangeST -= playerHUD.UpdateST;
    }

    public void UnBindEnemyUI(CharacterStatComponent enemyStat)
    {
        enemyStat.OnHpChanged -= enemyStatUI.UpdateHP;
    }
    #region 인벤토리 관련
    public void UpdateItemBoxUI()
    {
        if(itemBoxUI != null && itemBoxUI.gameObject.activeSelf)
        {
            itemBoxUI.UpdateBoxUI();
        }
    }

    public void UpdateInventoryUI()
    {
        inventoryUI.UpdateInventoryUI();
    }

    public void OpenItemBox(ItemBox box)
    {
        itemBoxUI.transform.parent.gameObject.SetActive(true);
        itemBoxUI.SetupBoxUI(box);
    }
    #endregion
}
