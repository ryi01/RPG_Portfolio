using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop", menuName = "Shop/Shop  Data")]
public class StoreData : ScriptableObject
{
    [SerializeField] private string storeName;
    [SerializeField] private List<Item> storeItems = new List<Item>();

    public string StoreName => storeName;
    public List<Item> ShopItems => storeItems;
}
