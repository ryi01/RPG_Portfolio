using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventroySystem : MonoBehaviour
{
    // 아이템 정보 DB 역할 : 0 - 무기, 1 - 소모성
    [SerializeField] private ItemList[] itemLists;

    [SerializeField] private List<Item> hasItemList = new List<Item>();
    public List<Item> HasItemList { get => hasItemList; set=>hasItemList = value; }

    [SerializeField] private int inventorySize;

    private void Start()
    {
        
    }
    protected void FirstAddItem()
    {

    }
    public Item GetItemData(EnumTypes.ITEM_TYPE type, int id)
    {
        int listIndex = (int)type;
        if (listIndex >= itemLists.Length || itemLists[listIndex] == null) return null;
        return itemLists[listIndex].List.FirstOrDefault(item => item.ItemID == id);
    }
    public bool AddItem(ItemInfo itemInfo)
    {
        if (itemInfo.ItemType == EnumTypes.ITEM_TYPE.WP)
        {
            if (hasItemList.Count >= inventorySize)
            {
                Debug.Log("인벤토리 Full");
                return false;
            }
            Item findItem = itemLists[(int)EnumTypes.ITEM_TYPE.WP].List.FirstOrDefault(item => item.ItemID == itemInfo.itemId);
            if (findItem == null) return false;

            Item pickUpItem = findItem.Clone();
            HasItemList.Add(pickUpItem);
        }
        else if(itemInfo.ItemType == EnumTypes.ITEM_TYPE.CB)
        {
            ConsumableItem hasItem = (ConsumableItem)HasItemList.FirstOrDefault(Item => Item.ItemID == itemInfo.itemId);
            if(hasItem != null)
            {
                Debug.Log($"인벤토리에 [{hasItem.ItemName}] " +
                    $"소모성 아이템이 추가됨 [현재아이템갯수 : {++hasItem.ItemCount}");
            }
            else
            {
                if(hasItemList.Count >= inventorySize)
                {
                    Debug.Log("꽉참");
                    return false;
                }
                Item findItem = itemLists[(int)EnumTypes.ITEM_TYPE.CB].List.FirstOrDefault(item => item.ItemID == itemInfo.itemId);
                if(findItem == null) return false;
                Item pickUpItem = findItem.Clone();
                HasItemList.Add(pickUpItem);
            }
        }
        
        return true; 
    }

    public void RemoveItem(Item item)
    {
        if(HasItemList.Contains(item))
        {
            HasItemList.Remove(item);
        }

    }

}
