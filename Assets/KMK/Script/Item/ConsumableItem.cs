using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Item/Consumable")]
public class ConsumableItem : Item
{
    [SerializeField] protected EnumTypes.CB_TYPE cbType;
    [SerializeField] protected int upValue;
    public EnumTypes.CB_TYPE CbType { get => cbType; set => cbType = value; }
    public int UpValue { get => upValue; set => upValue = value; }

    public virtual void Consume(GameObject target = null)
    {

    }
}
