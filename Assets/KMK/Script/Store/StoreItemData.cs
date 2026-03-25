using UnityEngine;
[CreateAssetMenu(fileName = "Shop Item", menuName = "Shop/Shop Item Data")]
public class StoreItemData : ScriptableObject
{
    [SerializeField] private Item itemData;
    [SerializeField] private int buyPrice;
    [SerializeField] private int sellPrice;
    [SerializeField] private bool isBuy = true;
    [SerializeField] private bool isSell = false;

    public Item ItemData => itemData;
    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
    public bool IsBuy => isBuy;
    public bool IsSell => isSell;
}
