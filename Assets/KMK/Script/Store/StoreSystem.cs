
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 해야하는 일 
/// 1. 어떤 상점을 열었는가?
/// 2. 현재 상점의 아이템 목록 제공
/// 3. 구매 가능 여부 확인
/// 4. 구매 처리
/// 5. 판매 처리
/// </summary>
public class StoreSystem : MonoBehaviour
{
    [SerializeField] private StoreData currentStore;
    [SerializeField] private GoldSystem goldSystem;
    [SerializeField] private InventorySystem inventorySystem;

    public StoreData CurrentStore => currentStore;

    public Action<StoreData> OnOpenStore;
    public Action OnCloseStore;
    public Action OnChangedStoreData;
    public Action<string> OnFailedTransaction;

    private List<Item> currentStoreItems = new List<Item>();
    public List<Item> CureentStoreItems => currentStoreItems;

    public void OpenStore(StoreData storeData)
    {
        if (storeData == null) return;
        currentStore = storeData;
        currentStoreItems = GetCurrentStoreItemsFromDB();
        OnOpenStore?.Invoke(currentStore);
        OnChangedStoreData?.Invoke();
        DebugStoreJoinAndType();
    }

    public void CloseStore()
    {
        currentStore = null;
        OnCloseStore?.Invoke();
    }

    public bool BuyItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            FailTransaction("상점에 아이템이 없습니다.");
            return false;
        }
        if (amount <= 0)
        {
            FailTransaction("수량이 올바르지 않습니다");
            return false;
        }

        if (goldSystem == null || inventorySystem == null) return false;
        int totalPrice = item.BuyPrice * amount;
        if(!goldSystem.IsEnoughGold(totalPrice))
        {
            FailTransaction("골드가 부족합니다.");
            return false;
        }
        bool spent = goldSystem.SpendGold(totalPrice);
        if (!spent)
        {
            FailTransaction("골드 차감에 실패했습니다.");
            return false;
        }
        bool added = inventorySystem.AddItem(item, amount);
        if(!added)
        {
            goldSystem.AddGold(totalPrice);
            FailTransaction("인벤토리가 가득 찼거나, 아이템 추가에 실패했습니다.");
            return false;
        }
        OnChangedStoreData?.Invoke();
        return true;
    }

    public bool SellItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            FailTransaction("판매할 아이템이 없습니다.");
            return false;
        }

        if (amount <= 0)
        {
            FailTransaction("수량이 올바르지 않습니다.");
            return false;
        }

        if (!inventorySystem.HasItem(item, amount))
        {
            FailTransaction("아이템 수량이 부족합니다.");
            return false;
        }
        Item foundItem = inventorySystem.HasItemList.Find(x => x != null && x.ItemID == item.ItemID);
        if (foundItem == null)
        {
            FailTransaction("인벤토리에서 판매 아이템을 찾지 못했습니다.");
            return false;
        }
        bool removed = inventorySystem.RemoveItem(foundItem, amount);
        if (!removed)
        {
            FailTransaction("아이템 판매에 실패했습니다.");
            return false;
        }

        goldSystem.AddGold(item.SellPrice * amount);
        OnChangedStoreData?.Invoke();
        return true;
    }

    public int GetCurrentGold()
    {
        return goldSystem != null ? goldSystem.CurrentGold : 0;
    }

    public int GetOwnedItemCount(Item storeItem)
    {
        if (storeItem == null) return 0;

        return inventorySystem.GetItemCount(storeItem);
    }

    private void FailTransaction(string message)
    {
        OnFailedTransaction?.Invoke(message);
    }

    public string GetCurrentStoreName()
    {
        var sqliteManager = GameManager.Instance.SQLiteManager;
        if (sqliteManager == null) return "";
        var rows = sqliteManager.LoadStoreItemsByJoin(currentStore.StoreId);
        if (rows == null || rows.Count == 0) return "";
        return rows[0].StoreName;
    }
    private void DebugStoreJoinAndType()
    {
        var sqliteManager = GameManager.Instance.SQLiteManager;
        if (sqliteManager == null) return;
        var storeRows = sqliteManager.LoadStoreItemsByJoin(currentStore.StoreId);
        Debug.Log($"===== [상점 연결 확인] / StoreId:{currentStore.StoreId} =====");
        foreach (var row in storeRows)
        {
            Item matchedItem = currentStore.ShopItems.Find(x => x != null && x.ItemID == row.ItemId);
            if (matchedItem == null)
            {
                Debug.Log($"DB ItemId:{row.ItemId} -> SO에서 못 찾음");
                continue;
            }
            int dbTypeValue = sqliteManager.LoadItemType(row.ItemId);
            string dbTypeText = dbTypeValue >= 0 ? ((EnumTypes.ITEM_TYPE)dbTypeValue).ToString() : "UnKnown";
            Debug.Log(
            $"상점:{row.StoreName} / " +
            $"ItemId:{row.ItemId} / " +
            $"이름:{matchedItem.ItemName} / " +
            $"SO타입:{matchedItem.ItemType} / " +
            $"DB타입:{dbTypeText} / " +
            $"가격:{matchedItem.BuyPrice} / " );
        }
        Debug.Log("===== [상점 연결 확인 끝] =====");
    }

    public List<Item> GetCurrentStoreItemsFromDB()
    {
        List<Item> result = new List<Item>();
        var sqliteManager = GameManager.Instance.SQLiteManager;
        if (sqliteManager == null || currentStore == null || currentStore.ShopItems == null) return result;

        var rows = sqliteManager.LoadStoreItemsByJoin(currentStore.StoreId);
        foreach(var row in rows)
        {
            Item matchedItem = currentStore.ShopItems.Find(x => x != null && x.ItemID == row.ItemId);
            if (matchedItem != null)
            {
                result.Add(matchedItem);
            }
        }
        return result;
    }
}
