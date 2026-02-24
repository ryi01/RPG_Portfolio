using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct ItemInfo
{
    public EnumTypes.ITEM_TYPE ItemType;
    public int itemId;
}
public class ItemBox : MonoBehaviour
{
    [SerializeField] private ItemInfo[] itemInfoList;
    public ItemInfo[] ItemInfoList => itemInfoList;
    [SerializeField] private Vector2 idWpRange;
    [SerializeField] private Vector2 idCbRange;

    private void Awake()
    {
        MakeRandomContents();
    }

    private void MakeRandomContents()
    {
        int count = UnityEngine.Random.Range(3, 5);
        itemInfoList = new ItemInfo[count];
        for (int i = 0; i < count; i++)
        {
            // wp/cb 중 아무거나 넣기
            EnumTypes.ITEM_TYPE randomType = EnumTypes.ITEM_TYPE.CB;// (EnumTypes.ITEM_TYPE)UnityEngine.Random.Range(0, 2);
            int randomID = 0;
            // 아이템의 종류에 따라 id가 달라짐
            if(randomType == EnumTypes.ITEM_TYPE.CB)
                randomID = UnityEngine.Random.Range((int)idCbRange.x, (int)idCbRange.y + 1);
            else
                randomID = UnityEngine.Random.Range((int)idWpRange.x, (int)idWpRange.y + 1);
            // 중복되지만 아이템 타입과 아이디를 아이템박스의 list에 넣음
            itemInfoList[i] = new ItemInfo { ItemType = randomType, itemId = randomID };
        }
    }
    // 아이템 삭제
    public void RemoveItemFromBox(ItemInfo info)
    {
        for(int i = 0; i< itemInfoList.Length; i++)
        {
            // 아이템이 있는지 확인
            if (itemInfoList[i].ItemType == info.ItemType &&
                itemInfoList[i].itemId == info.itemId)
            {
                itemInfoList[i] = default;
                break;
            }
        }
        
    }

}
