using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HpConsumable", menuName = "Item/HpConsumable")]
public class HpConsumableItem : ConsumableItem
{
    public override void Consume(GameObject target = null)
    {
        base.Consume(target);

        if (target.TryGetComponent<PlayerStatComponent>(out var playerStat))
        {
            playerStat.RecoveryHP(UpValue);
            
        }
    }
}
