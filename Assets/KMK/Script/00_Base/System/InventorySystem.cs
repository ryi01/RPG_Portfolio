using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// MVC 패턴 : 아이템 DB(M), 인벤토리 시스템(C), 인벤토리UI(V)
public class InventorySystem : MonoBehaviour
{
    // 아이템 정보 DB 역할 : 0 - 무기, 1 - 소모성
    [SerializeField] private ItemList[] itemLists;

    [SerializeField] private List<Item> hasItemList = new List<Item>();
    public List<Item> HasItemList { get => hasItemList; set=>hasItemList = value; }

    [SerializeField] private int inventorySize;
    
    public Action OnChangedInventory;
    public ItemBox CurrentBox { get; set; }

    private void Start()
    {
        // 미리 inventorySize만큼 hasItemList 생성
        for (int i = 0; i < inventorySize; i++)
        {
            hasItemList.Add(null);
        }
    }
    // 실제 아이템 데이터를 들고 오는 함수 => DB 조회
    public Item FindItemData(EnumTypes.ITEM_TYPE type, int id)
    {
        int listIndex = (int)type;
        if (listIndex >= itemLists.Length || itemLists[listIndex] == null) return null;
        return itemLists[listIndex].List.FirstOrDefault(item => item.ItemID == id);
    }
    // 아이템 추가
    public bool AddItem(ItemInfo itemInfo)
    {
        bool isSucess = false;
        if(itemInfo.ItemType == EnumTypes.ITEM_TYPE.CB)
        {
            ConsumableItem hasItem = (ConsumableItem)HasItemList.FirstOrDefault(Item => Item != null && Item.ItemID == itemInfo.itemId); 
            if(hasItem != null)
            {
                isSucess = true;
                hasItem.ItemCount += itemInfo.itemCount;
            }
        }
        if(!isSucess) isSucess = TryAddItemToEmptySlot(itemInfo);
        if (isSucess) OnChangedInventory?.Invoke();

        return isSucess; 
    }
    // 스토어 전용
    public bool AddItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        ItemInfo itemInfo = new ItemInfo
        {
            ItemType = item.ItemType,
            itemId = item.ItemID,
            itemCount = amount
        };
        return AddItem(itemInfo);
    }
    private bool TryAddItemToEmptySlot(ItemInfo itemInfo)
    {
        // null을 확인하고 -1인 경우, 인벤토리가 가득 찬 상태 => 실패
        int emptyIndex = hasItemList.FindIndex(x => x == null);
        if (emptyIndex == -1) return false;
        // 아이템 찾기
        Item findItem = FindItemData(itemInfo.ItemType, itemInfo.itemId);
        if (findItem == null) return false;
        Item pickUpItem = findItem.Clone();
        if(pickUpItem is ConsumableItem cb)
        {
            cb.ItemCount = itemInfo.itemCount;
        }
        HasItemList[emptyIndex] = pickUpItem;
        return true;
    }
    // 아이템 사용
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= HasItemList.Count || HasItemList[slotIndex] == null) return;
        Item item = HasItemList[slotIndex];
        if(item is ConsumableItem consume)
        {
            consume.Consume();
            if (item.ItemCount > 1)
            {
                item.ItemCount--;
                OnChangedInventory?.Invoke();
            }
            else RemoveItem(item);

        }
    }
    // 아이템 삭제
    public bool RemoveItem(Item item, int amount = 1)
    {
        // 가진 아이템 리스트에서 index를 확인
        int index = HasItemList.IndexOf(item);
        if (index == -1) return false;
        if(item is ConsumableItem consumable)
        {
            if (consumable.ItemCount < amount) return false;
            consumable.ItemCount -= amount;
            if(consumable.ItemCount <= 0)
            {
                // 가진 아이템 리스트를 비움
                HasItemList[index] = null;
            }
            OnChangedInventory?.Invoke();
            return true;
        }

        HasItemList[index] = null;
        OnChangedInventory?.Invoke();
        return true;
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= HasItemList.Count || indexB < 0 || indexB >= HasItemList.Count) return;

        Item temp = hasItemList[indexA];
        hasItemList[indexA] = hasItemList[indexB];
        hasItemList[indexB] = temp;

        OnChangedInventory?.Invoke();
    }

    public bool HasItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        Item foundItem = HasItemList.FirstOrDefault(x => x != null && x.ItemID == item.ItemID);
        if (foundItem == null) return false;
        if(foundItem is ConsumableItem consumable)
        {
            return consumable.ItemCount >= amount;
        }
        return true;
    }

    public int GetItemCount(Item item)
    {
        if (item == null) return 0;

        Item foundItem = HasItemList.FirstOrDefault(x => x != null && x.ItemID == item.ItemID);
        if (foundItem == null) return 0;

        if(foundItem is ConsumableItem consumable)
        {
            return consumable.ItemCount;
        }
        return 1;
    }
    public Item GetItemBySlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hasItemList.Count) return null;

        return HasItemList[slotIndex];
    }
}
