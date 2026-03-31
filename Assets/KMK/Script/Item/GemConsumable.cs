using UnityEngine;
[CreateAssetMenu(fileName = "GoldConsumable", menuName = "Item/GoldConsumable")]
public class GemConsumable : ConsumableItem
{
    public override void Consume(GameObject target = null)
    {
        base.Consume(target);
        if (target.TryGetComponent(out PlayerController player))
        {
            player.AddGold(upValue);
        }
    }
}
