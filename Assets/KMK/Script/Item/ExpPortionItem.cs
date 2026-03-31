using UnityEngine;
[CreateAssetMenu(fileName = "ExpConsumable", menuName = "Item/ExpConsumable")]
public class ExpPortionItem : ConsumableItem
{
    public override void Consume(GameObject target = null)
    {
        base.Consume(target);
        if (target.TryGetComponent<PlayerStatComponent>(out var playerStat))
        {
            playerStat.TakeExp(UpValue);
        }
    }
}
