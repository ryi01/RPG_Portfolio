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
        int index = inventroySystem.HasItemList.IndexOf(item);
        if (index != -1) inventroySystem.UseItem(index);
        inventroySystem.RemoveItem(item);
    }
    private void OnDestroy()
    {
        if(inventroySystem != null) inventroySystem.OnChangedInventory -= UpdateInventoryUI;
    }
}
