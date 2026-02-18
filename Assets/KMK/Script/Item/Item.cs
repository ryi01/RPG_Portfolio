using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField] private EnumTypes.ITEM_TYPE itemType;
    [SerializeField] private int itemId;
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private Sprite itemIconImage;
    [SerializeField] private int itemPrice;
    [SerializeField] private int itemCount;
    [SerializeField] private bool isEquip;

    public EnumTypes.ITEM_TYPE ItemType { get => itemType; set => itemType = value; }
    public int ItemID { get => itemId; set => itemId = value; }
    public string ItemName { get => itemName; set => itemName = value; }
    public string ItemDescription { get => itemDescription; set => itemDescription = value; }
    public Sprite ItemIconImage { get => itemIconImage; set => itemIconImage = value; }
    public int ItemPrice { get => itemPrice; set => itemPrice = value; }
    public int ItemCount { get => itemCount; set => itemCount = value; }
    public bool IsEquip { get => isEquip; set => isEquip = value; }

    public Item Clone()
    {
        Item newItem = Instantiate(this);
        return newItem;
    }
}
