using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBoxUI : MonoBehaviour
{
    // 아이템 ui 프리팹
    [SerializeField] private ItemUI itemBoxItemUIPrefab;
    // 아이템 ui 위치
    [SerializeField] private RectTransform[] itemUISlots;
    [SerializeField] private InventorySystem inventorySystem;
    private ItemBox currentBox;
    private List<ItemUI> spawnUIs = new List<ItemUI>();

    // 박스를 클릭해서 ui가 열리면 넘겨진 box에 따라 itemPrefab 설정하기
    public void SetupBoxUI(ItemBox box)
    {
        ClearExistingUI();
        currentBox = box;
        List<ItemInfo> contents = box.ItemInfoList;
        // 상자에 미리 생성된 item 수만큼 생성
        for(int i = 0; i < contents.Count; i++)
        {
            if (i >= itemUISlots.Length) break;
            // 아이템 종류 및 아이디 담기
            ItemInfo item = contents[i];
            // 실제 아이템 값 들고오기 
            Item actualItem = inventorySystem.FindItemData(item.ItemType, item.itemId);
            if(actualItem != null)
            {
                // 아이템의 정보에 따른 prefab설정
                ItemUI newUI = Instantiate(itemBoxItemUIPrefab, itemUISlots[i]).GetComponent<ItemUI>();
                newUI.InitInventory(inventorySystem);
                newUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                spawnUIs.Add(newUI);

                Item displayItem = actualItem.Clone();
                if(displayItem is ConsumableItem cb)
                {
                    cb.ItemCount = item.itemCount;
                }
                // 초기화 및 클릭 이벤트 => 익명함수에서 인자를 바로 넘기면 인덱스가 엉킴
                ItemInfo currentInfo = item;
                ItemUI currentUI = newUI;
                newUI.InitItemUI(displayItem, () => LootItem(currentInfo, currentUI));
            }
            
        }

    }
    public void UpdateBoxUI()
    {
        if (currentBox == null) return;
        SetupBoxUI(currentBox);
    }

    private void LootItem(ItemInfo info, ItemUI ui)
    {
        if(inventorySystem.AddItem(info))
        {
            currentBox.RemoveItemFromBox(info);
            spawnUIs.Remove(ui);
            Destroy(ui.gameObject);
        }
    }
    private void ClearExistingUI()
    {
        foreach(var ui in spawnUIs)
        {
            if (ui != null) Destroy(ui.gameObject);
        }
        spawnUIs.Clear();
    }
    public void CloseUI()
    {
        ClearExistingUI();
        gameObject.SetActive(false);
    }
}
