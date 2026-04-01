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
    [SerializeField] private int inventorySize;
    [SerializeField] private UIManager uiManager;
    public List<Item> HasItemList { get => hasItemList; set=>hasItemList = value; }
    public Action OnChangedInventory;

    public ItemBox CurrentBox { get; set; }

    private void Start()
    {
        LoadInventoryFromDB();
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
        if (isSucess)
        {
            OnChangedInventory?.Invoke();
            SaveInventoryToDB();
        }

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
    public void UseItem(int slotIndex, GameObject target)
    {
        if (slotIndex < 0 || slotIndex >= HasItemList.Count || HasItemList[slotIndex] == null) return;
        Item item = HasItemList[slotIndex];
        if(item is ConsumableItem consume)
        {
            consume.Consume(target);
            if (item.ItemCount > 1)
            {
                item.ItemCount--;
                OnChangedInventory?.Invoke();
                SaveInventoryToDB();
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
            SaveInventoryToDB();
            return true;
        }

        HasItemList[index] = null;
        OnChangedInventory?.Invoke();
        SaveInventoryToDB();
        return true;
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= HasItemList.Count || indexB < 0 || indexB >= HasItemList.Count) return;

        Item temp = hasItemList[indexA];
        hasItemList[indexA] = hasItemList[indexB];
        hasItemList[indexB] = temp;

        OnChangedInventory?.Invoke();
        SaveInventoryToDB();
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

    public void OpenItemBox(ItemBox box)
    {
        if (box == null || uiManager == null) return;
        CurrentBox = box;
        uiManager.OpenItemBox(box);
    }

    public void CloseItemBox()
    {
        CurrentBox= null;
        uiManager.CloseItemBox();
    }
    #region SQLite
    public void SaveInventoryToDB()
    {
        if (GameManager.Instance == null || GameManager.Instance.DataManager == null || GameManager.Instance.SQLiteManager == null) return;
        int playerId = GameManager.Instance.DataManager.Id;
        var sqlLiteManager = GameManager.Instance.SQLiteManager;

        // 인벤토리 데이터 전부 삭제 => 전체 덮어쓰기 저장이기에 한번 정리
        sqlLiteManager.ClearInventory(playerId);

        // 현재 인벤토리 슬롯 검사
        for(int i = 0; i < hasItemList.Count; i++)
        {
            // 슬롯 기준으로 저장
            Item item = hasItemList[i];
            if (item == null) continue;
            int count = 1;
            // 소모품이면 갯수 저장
            if(item is ConsumableItem consumable)
            {
                count = consumable.ItemCount;
            }
            sqlLiteManager.InsertInventoryItem(playerId, i,item.ItemID, count);
        }

    }
    public void LoadInventoryFromDB()
    {
        if (GameManager.Instance == null || GameManager.Instance.DataManager == null || GameManager.Instance.SQLiteManager == null) return;
        int playerId = GameManager.Instance.DataManager.Id;
        var sqlLiteManager = GameManager.Instance.SQLiteManager;

        // 기존 인벤토리 비우기
        hasItemList.Clear();
        for (int i = 0; i < inventorySize; i++)
        {
            hasItemList.Add(null);
        }

        List<InventorySaveData> savedItems = sqlLiteManager.LoadInventory(playerId);
        // DB 인벤토리 데이터 읽기
        foreach(var saved in savedItems)
        {
            if (saved.SlotIndex < 0 || saved.SlotIndex >= inventorySize) continue;
            
            // DB의 itemid를 실제 item 데이터로 변경
            Item itemData = FindItemById(saved.ItemId);
            if (itemData == null) continue;

            // 복사본 생성 => SO/원본데이터 공유 방지
            Item cloneItem = itemData.Clone();

            // 소모품이면 한 슬롯에 카운트 넣음
            if(cloneItem is ConsumableItem consumable)
            {
                consumable.ItemCount = saved.Count;
                hasItemList[saved.SlotIndex] = consumable;
            }
            else
            {
                hasItemList[saved.SlotIndex] = cloneItem;
            }
        }

        OnChangedInventory?.Invoke();
    }
    private Item FindItemById(int itemId)
    {
        // 아이템 리스트 안의 모든 아이템 DB를 순회하면서 ID가 일치하는것 찾기
        foreach(var itemList in itemLists)
        {
            if (itemList == null || itemList.List == null) continue;

            Item foundItem = itemList.List.FirstOrDefault(item => item.ItemID == itemId);
            if (foundItem != null) return foundItem;
        }
        return null;
    }
    #endregion
}
