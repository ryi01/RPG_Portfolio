using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Text storeNameText;

    [SerializeField] private float showPopupTime = 1.5f;

    [SerializeField] private RectTransform[] storeSlotParents;
    [SerializeField] private ItemUI[] itemUIs;
    [SerializeField] private GameObject itemUIPrefab;

    private Item selectedStoreItem;
    private Item selectedInventoryItem;

    private ItemUI selectedItemUI;
    private Coroutine popupCoroutine;

    private void OnEnable()
    {
        if (storeSystem != null)
        {
            storeSystem.OnOpenStore += HandleOpenStore;
            storeSystem.OnCloseStore += HandleCloseStore;
            storeSystem.OnChangedStoreData += RefreshStoreUI;
            storeSystem.OnFailedTransaction += HandleFailedTransaction;
        }
    }
    private void Start()
    {
        InitStoreUI();

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
        storeSystem.CloseStore();
    }

    private void HandleOpenStore(StoreData storeData)
    {
        skillPanel.SetActive(false);
        storePanel.SetActive(true);
        ClearSelection();

        if (storeNameText != null) storeNameText.text = storeSystem.GetCurrentStoreName();

        inventoryUI.SetShopMode(SelectInventoryItem);
        inventoryUI.UpdateInventoryUI();

        RefreshStoreUI();
    }
    private void HandleCloseStore()
    {
        skillPanel.SetActive(true);
        storePanel.SetActive(false);
        ClearSelection();
        if (storeNameText != null) storeNameText.text = "";
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
        informations[0].text = item.ItemName + " / (" + GetTypeText(item.ItemType) + ")";
        informations[1].text = item.ItemDescription;
        if(selectedInventoryItem != null) informations[2].text = item.SellPrice.ToString();
        else if(selectedStoreItem != null) informations[2].text = item.BuyPrice.ToString();
    }

    public void RefreshStoreUI()
    {
        if (itemUIs == null || itemUIs.Length == 0) return;
        List<Item> currentStoreItems = storeSystem.CureentStoreItems;
        for (int i = 0; i < itemUIs.Length; i++)
        {
            if (currentStoreItems != null && i < currentStoreItems.Count && currentStoreItems[i] != null)
            {
                Item storeItem = currentStoreItems[i];
                ItemUI currentUI = itemUIs[i];

                currentUI.SlotIndex = i;
                currentUI.SetMode(EnumTypes.ItemUIMode.StoreBuy);
                currentUI.InitItemUI(storeItem, () => SelectStoreItem(storeItem, currentUI));
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
    private string GetTypeText(EnumTypes.ITEM_TYPE type)
    {
        switch(type)
        {
            case EnumTypes.ITEM_TYPE.WP:
                return "무기";
            case EnumTypes.ITEM_TYPE.CB:
                return "소비 아이템";
            default:
                return "기타";
        }
    }
    #region 팝업 전용
    public void ShowPopUp(bool enabled, string text = "")
    {
        popupText.text = text;
        popupPanel.SetActive(enabled);
        if (popupCoroutine != null)
            StopCoroutine(popupCoroutine);

        popupCoroutine = StartCoroutine(OffPopUpTimer());
    }
    IEnumerator OffPopUpTimer()
    {
        yield return new WaitForSeconds(showPopupTime);
        popupPanel.SetActive(false);
    }
    #endregion
}
