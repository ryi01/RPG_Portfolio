using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 상점 전체 관리
/// </summary>
public class StoreUI : MonoBehaviour
{
    [SerializeField] private StoreSystem storeSystem;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private GameObject skillPanel;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Text[] informations;
    [SerializeField] private Text buttText;
    [SerializeField] private Text popupText;

    [SerializeField] private float showPopupTime = 1.5f;

    [SerializeField] private RectTransform[] storeSlotParents;
    [SerializeField] private ItemUI[] itemUIs;
    [SerializeField] private GameObject itemUIPrefab;

    private Item selectedStoreItem;
    private Item selectedInventoryItem;

    private ItemUI selectedItemUI;
    private void Start()
    {
        InitStoreUI();
        if (storeSystem != null)
        {
            storeSystem.OnOpenStore += HandleOpenStore;
            storeSystem.OnCloseStore += HandleCloseStore;
            storeSystem.OnChangedStoreData += RefreshStoreUI;
            storeSystem.OnFailedTransaction += HandleFailedTransaction;
        }
        storePanel.SetActive(false);
        ShowPopUp(false);
    }

    public void InitStoreUI()
    {
        if (storeSlotParents == null || storeSlotParents.Length == 0) return;
        itemUIs = new ItemUI[storeSlotParents.Length];

        for(int i = 0; i < storeSlotParents.Length;i++)
        {
            itemUIs[i] = Instantiate(itemUIPrefab, storeSlotParents[i]).GetComponent<ItemUI>();
        }

        for(int i = 0; i< informations.Length;i++)
        {
            informations[i].text = "";
        }
        inventoryUI.InitInventoryUI();
    }

    public void OnButtCloseStore()
    {
        storeSystem.CloseShop();
    }

    private void HandleOpenStore(StoreData storeData)
    {
        skillPanel.SetActive(false);
        storePanel.SetActive(true);
        ClearSelection();

        inventoryUI.SetShopMode(SelectInventoryItem);
        inventoryUI.UpdateInventoryUI();

        RefreshStoreUI();
    }
    private void HandleCloseStore()
    {
        skillPanel.SetActive(true);
        storePanel.SetActive(false);
        ClearSelection();
        inventoryUI.SetUseMode();
        inventoryUI.UpdateInventoryUI();
        ClearStoreUI();
    }
    private void ClearSelection()
    {
        if (selectedItemUI != null) selectedItemUI.SetSelect(false);
        selectedItemUI = null;
        selectedStoreItem = null;
        selectedInventoryItem = null;
        for (int i = 0; i < informations.Length; i++)
        {
            informations[i].text = "";
        }
    }
    private void SelectInform(Item item)
    {
        informations[0].text = item.ItemName;
        informations[1].text = item.ItemDescription;
        if(selectedInventoryItem != null) informations[2].text = item.SellPrice.ToString();
        else if(selectedStoreItem != null) informations[2].text = item.BuyPrice.ToString();
    }

    public void RefreshStoreUI()
    {
        if (itemUIs == null || itemUIs.Length == 0) return;
        StoreData currentStore = storeSystem.CurrentStore;
        for (int i = 0; i < itemUIs.Length; i++)
        {
            if (currentStore != null && currentStore.ShopItems != null && i < currentStore.ShopItems.Count && currentStore.ShopItems[i] != null)
            {
                Item storeItem = currentStore.ShopItems[i];
                ItemUI currentUI = itemUIs[i];

                itemUIs[i].SlotIndex = i;
                itemUIs[i].SetMode(EnumTypes.ItemUIMode.StoreBuy);
                itemUIs[i].InitItemUI(storeItem, () => SelectStoreItem(storeItem, currentUI));
            }
            else
            {
                itemUIs[i].SetMode(EnumTypes.ItemUIMode.StoreBuy);
                itemUIs[i].ClearItemUI();
            }
        }
    }

    private void ClearStoreUI()
    {
        if (itemUIs == null) return;
        for(int i = 0; i < itemUIs.Length; i++)
        {
            if (itemUIs[i] != null) itemUIs[i].ClearItemUI();
        }
    }

    private void SelectStoreItem(Item storeItem, ItemUI clickedUI)
    {
        buttText.text = "구매";
        selectedInventoryItem = null;
        if (selectedItemUI != null) selectedItemUI.SetSelect(false);
        selectedItemUI = clickedUI;
        selectedStoreItem = storeItem;
        if (selectedItemUI != null) selectedItemUI.SetSelect(true);
        SelectInform(storeItem);
        Debug.Log($"구매 선택: {storeItem.name} / 가격: {storeItem.BuyPrice}");
    }

    private void SelectInventoryItem(Item item, ItemUI clickedUI)
    {
        buttText.text = "판매";
        selectedStoreItem = null;
        if (selectedItemUI != null) selectedItemUI.SetSelect(false);
        selectedItemUI = clickedUI;
        selectedInventoryItem = item;
        if (selectedItemUI != null) selectedItemUI.SetSelect(true);
        SelectInform(item);
        Debug.Log($"판매 선택: {item.name}");
    }

    public void OnBuyOrSellButt()
    {
        if (selectedInventoryItem != null) SellSelectedItem();
        else if (selectedStoreItem != null) BuySelectStoreItem();
    }
    public void BuySelectStoreItem()
    {
        if (selectedStoreItem == null) return;
        bool success = storeSystem.BuyItem(selectedStoreItem, 1);
        if(success)
        {
            ClearSelection();
            inventoryUI.UpdateInventoryUI();
            RefreshStoreUI();
        }
    }
    public void SellSelectedItem()
    {
        if (selectedInventoryItem == null) return;
        bool sucess = storeSystem.SellItem(selectedInventoryItem, 1);
        if(sucess)
        {
            ClearSelection();
            inventoryUI.UpdateInventoryUI();
            RefreshStoreUI();
        }
    }

    private void HandleFailedTransaction(string message)
    {
        ShowPopUp(true, message);
        Debug.LogWarning(message);
    }

    private void OnDestroy()
    {
        if (storeSystem != null)
        {
            storeSystem.OnOpenStore -= HandleOpenStore;
            storeSystem.OnCloseStore -= HandleCloseStore;
            storeSystem.OnChangedStoreData -= RefreshStoreUI;
            storeSystem.OnFailedTransaction -= HandleFailedTransaction;
        }
    }
    #region 팝업 전용
    public void ShowPopUp(bool enabled, string text = "")
    {
        popupText.text = text;
        popupPanel.SetActive(enabled);
        StartCoroutine(OffPopUpTimer());
    }
    IEnumerator OffPopUpTimer()
    {
        yield return new WaitForSeconds(showPopupTime);
        popupPanel.SetActive(false);
    }
    #endregion
}
