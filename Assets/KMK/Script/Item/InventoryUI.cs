using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnumTypes;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform[] itemUISlots;
    [SerializeField] private ItemUI[] itemUIs;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private InventorySystem inventroySystem;

    public Action<Item, ItemUI> OnClickItem;
    public Action<Item, ItemUI> OnDoubleClickItem;

    private ItemUIMode currentMode = ItemUIMode.Use;
    private void OnEnable()
    {
        Refresh();
    }
    private void Start()
    {
        inventroySystem.OnChangedInventory += UpdateInventoryUI;
    }
    public void InitInventoryUI()
    {
        itemUIs = new ItemUI[itemUISlots.Length];
        for(int i = 0; i < itemUISlots.Length; i++)
        {
            itemUIs[i] = Instantiate(itemUIPrefab, itemUISlots[i]).GetComponent<ItemUI>();
        }
        SetUseMode();
    }
    public void Refresh()
    {
        UpdateInventoryUI();
    }

    public void SetMode(ItemUIMode mode, Action<Item, ItemUI> clickAction = null, Action<Item, ItemUI> doubleClickAction = null)
    {
        currentMode = mode;
        OnClickItem = clickAction;
        OnDoubleClickItem = doubleClickAction;
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < itemUIs.Length; i++)
        {
            itemUIs[i].SlotIndex = i;
            itemUIs[i].SetMode(currentMode);

            if (i < inventroySystem.HasItemList.Count && inventroySystem.HasItemList[i] != null)
            {
                Item item = inventroySystem.HasItemList[i];
                ItemUI currentUI = itemUIs[i];
                itemUIs[i].InitItemUI(item, () => OnClickItem?.Invoke(item, currentUI), () => OnDoubleClickItem?.Invoke(item, currentUI));
            }
            else itemUIs[i].ClearItemUI();
        }
    }

    public void RemoveItem(Item item)
    {
        inventroySystem.RemoveItem(item);
    }
    public void UseItem(Item item, ItemUI ui)
    {
        int index = inventroySystem.HasItemList.IndexOf(item);
        if (index != -1) inventroySystem.UseItem(index);
    }
    public void SetUseMode()
    {
        SetMode(ItemUIMode.Use, null, UseItem);
    }
    public void SetShopMode(Action<Item, ItemUI> customAction)
    {
        SetMode(ItemUIMode.ShopSell, customAction, null);
    }    
    private void OnDestroy()
    {
        if(inventroySystem != null) inventroySystem.OnChangedInventory -= UpdateInventoryUI;
    }
}
