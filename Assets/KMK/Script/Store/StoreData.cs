using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop", menuName = "Shop/Shop Item Data")]
public class StoreData : ScriptableObject
{
    [SerializeField] private string storeName;
    [SerializeField] private List<StoreItemData> storeItems = new List<StoreItemData>();

    public string StoreName => storeName;
    public List<StoreItemData> ShopItems => storeItems;
}
