using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HpConsumable", menuName = "Item/HpConsumable")]
public class HpConsumableItem : ConsumableItem
{
    public override void Consume(GameObject target = null)
    {
        base.Consume(target);

        GameObject actor = target != null ? target : GameObject.FindGameObjectWithTag("Player");
        if(actor != null)
        {
            if(actor.TryGetComponent<PlayerStatComponent>(out var playerStat))
            {
                playerStat.RecoveryHP(UpValue);
            }
            else Debug.LogError("PlayerStatComponent를 찾을 수 없습니다!");
        }
    }
}
