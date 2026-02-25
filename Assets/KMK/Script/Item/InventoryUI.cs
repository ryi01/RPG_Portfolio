using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform[] itemUISlots;
    [SerializeField] private ItemUI[] itemUIs;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private InventroySystem inventroySystem;
    private Item item;
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
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < itemUIs.Length; i++)
        {
            itemUIs[i].SlotIndex = i;

            if (i < inventroySystem.HasItemList.Count && inventroySystem.HasItemList[i] != null)
            {
                Item item = inventroySystem.HasItemList[i];
                itemUIs[i].InitItemUI(item, () => UseItem(item));
            }
            else itemUIs[i].ClearItemUI();
        }
    }

    public void RemoveItem(Item item)
    {
        inventroySystem.RemoveItem(item);
    }

    public void UseItem(Item item)
    {
        if(item.ItemType == EnumTypes.ITEM_TYPE.WP)
        {

        }
        else
        {
            if(item.ItemCount > 1)
            {
                item.ItemCount--;
                ((ConsumableItem)item).Consume();
                UpdateInventoryUI();
                return;
            }
        }

        ((ConsumableItem)item).Consume();
        inventroySystem.RemoveItem(item);
    }
    private void OnDestroy()
    {
        if(inventroySystem != null) inventroySystem.OnChangedInventory -= UpdateInventoryUI;
    }
}
