using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct ItemInfo
{
    public EnumTypes.ITEM_TYPE ItemType;
    public int itemId;
    public int itemCount;
}
public class ItemBox : InteractionObject
{
    [SerializeField] private List<ItemInfo> itemInfoList = new List<ItemInfo>();
    public List<ItemInfo> ItemInfoList => itemInfoList;
    [SerializeField] private Vector2 idWpRange;
    [SerializeField] private Vector2 idCbRange;

    private void Awake()
    {
        MakeRandomContents();
    }

    private void MakeRandomContents()
    {
        // 리스트 초기화
        itemInfoList.Clear();
        // 아이템 갯수 랜덤
        int count = UnityEngine.Random.Range(2, 5);
        
        for (int i = 0; i < count; i++)
        {
            // wp/cb 중 아무거나 넣기
            EnumTypes.ITEM_TYPE randomType = EnumTypes.ITEM_TYPE.CB;// (EnumTypes.ITEM_TYPE)UnityEngine.Random.Range(0, 2);
            int randomID = 0;
            bool isStacked = false;
            // 아이템의 종류에 따라 id가 달라짐
            if (randomType == EnumTypes.ITEM_TYPE.CB)
            {
                randomID = UnityEngine.Random.Range((int)idCbRange.x, (int)idCbRange.y + 1);
                for(int j = 0; j < itemInfoList.Count; j++)
                {
                    if (itemInfoList[j].itemId == randomID &&
                        itemInfoList[j].ItemType == randomType)
                    {
                        ItemInfo temp = itemInfoList[j];
                        temp.itemCount++;
                        itemInfoList[j] = temp;

                        isStacked = true;
                        break;
                    }
                }
                if (!isStacked)
                {
                    ItemInfo newInfo = new ItemInfo();
                    newInfo.itemId = randomID;
                    newInfo.ItemType = randomType;
                    newInfo.itemCount = 1;

                    ItemInfoList.Add(newInfo);
                } 
            }

        }
    }
    // 아이템 삭제
    public void RemoveItemFromBox(ItemInfo info)
    {
        itemInfoList.Remove(info);
        GameManager.Instance.UIManager.UpdateItemBoxUI();
    }

    public void AddItemFromInventroy(Item item)
    {
        ItemInfo info = new ItemInfo
        {
            itemId = item.ItemID,
            itemCount = item.ItemCount,
            ItemType = item.ItemType
        };

        itemInfoList.Add(info);
        GameManager.Instance.UIManager.UpdateItemBoxUI();
    }

    public override void Interact(PlayerController player)
    {
        player.OpenBox(this);
    }
}
