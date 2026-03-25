using System;
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

    public StoreData CurrentStore => currentStore;

    public Action<StoreData> OnOpenStore;
    public Action OnCloseStore;
    public Action OnChangedStoreData;
    public Action<string> OnFailedTransaction;

    public void OpenStore(StoreData storeData)
    {
        if (storeData == null) return;
        currentStore = storeData;
        OnOpenStore?.Invoke(currentStore);
        OnChangedStoreData?.Invoke();
    }

    public void CloseShop()
    {
        currentStore = null;
        OnCloseStore?.Invoke();
    }

    public bool BuyItem(StoreItemData storeItem, int amount = 1)
    {
        if (storeItem == null || storeItem.ItemData == null)
        {
            FailTransaction("상점 아이템이 없습니다.");
            return false;
        }

        if (!storeItem.IsBuy)
        {
            FailTransaction("구매할 수 없는 아이템 입니다.");
            return false;
        }
        if (amount <= 0)
        {
            FailTransaction("수량이 올바르지 않습니다.");
            return false;
        }
        int totalPrice = storeItem.BuyPrice * amount;
        if (!GameManager.Instance.GoldSystem.IsEnoughGold(totalPrice))
        {
            FailTransaction("골드가 부족합니다.");
            return false;
        }
        bool added = GameManager.Instance.InventroySystem.AddItem(storeItem.ItemData, amount);
        if (!added)
        {
            FailTransaction("인벤토리가 가득 찼거나, 아이템 추가에 실패했습니다.");
            return false;
        }
        bool spent = GameManager.Instance.GoldSystem.SpendGold(totalPrice);
        if(!spent)
        {
            GameManager.Instance.InventroySystem.RemoveItem(storeItem.ItemData, amount);
            FailTransaction("골드 차감에 실패 했습니다");
            return false;
        }
        OnChangedStoreData?.Invoke();
        return true;
    }

    public bool SellItem(StoreItemData storeItem, int amount = 1)
    {
        if (storeItem == null || storeItem.ItemData == null)
        {
            FailTransaction("판매할 아이템이 없습니다.");
            return false;
        }
        if (!storeItem.IsSell)
        {
            FailTransaction("판매할 수 없는 아이템입니다.");
            return false;
        }
        if (amount <= 0)
        {
            FailTransaction("수량이 올바르지 않습니다.");
            return false;
        }
        var inventorySystem = GameManager.Instance.InventroySystem;
        if (!inventorySystem.HasItem(storeItem.ItemData, amount))
        {
            FailTransaction("아이템 수량이 부족합니다.");
            return false;
        }
        Item foundItem = inventorySystem.HasItemList.Find(x => x != null && x.ItemID == storeItem.ItemData.ItemID);
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

        GameManager.Instance.GoldSystem.AddGold(storeItem.SellPrice * amount);
        OnChangedStoreData?.Invoke();
        return true;
    }

    public int GetCurrentGold()
    {
        var goldSystem = GameManager.Instance.GoldSystem;
        return goldSystem != null ? goldSystem.CurrentGold : 0;
    }

    public int GetOwnedItemCount(StoreItemData storeItem)
    {
        if (storeItem == null || storeItem.ItemData == null) return 0;

        return GameManager.Instance.InventroySystem.GetItemCount(storeItem.ItemData);
    }

    private void FailTransaction(string message)
    {
        OnFailedTransaction?.Invoke(message);
    }
}
