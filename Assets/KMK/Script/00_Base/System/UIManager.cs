using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private StatUI playerHUD;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ItemBoxUI itemBoxUI;
    [SerializeField] private Text goldText;
    [SerializeField] private Text storeGoldText;

    private void Start()
    {
        inventoryUI.InitInventoryUI();
        goldText.text = "0";
        storeGoldText.text = "0";
    }
    #region 체력 관련
    public void BindPlayerUI(PlayerStatComponent player)
    {
        player.OnHpChanged += playerHUD.UpdateHP;
        player.OnChangeExp += playerHUD.UpdateExp;
        player.OncChangeLevel += playerHUD.UpdateLevel;
    }
    public void UnBindPlayerUI(PlayerStatComponent player)
    {
        player.OnHpChanged -= playerHUD.UpdateHP;
        player.OnChangeExp -= playerHUD.UpdateExp;
        player.OncChangeLevel -= playerHUD.UpdateLevel;
    }
    #endregion
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
    #region 골드 관련
    public void ChangeGold(int amount)
    {
        goldText.text = amount.ToString();
        storeGoldText.text = amount.ToString();
    }
    #endregion

}
