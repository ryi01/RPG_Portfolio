using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop", menuName = "Shop/Shop  Data")]
public class StoreData : ScriptableObject
{
    [SerializeField] private int storeId;
    [SerializeField] private List<Item> storeItems = new List<Item>();

    public int StoreId => storeId;
    public List<Item> ShopItems => storeItems;
}
