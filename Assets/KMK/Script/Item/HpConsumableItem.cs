using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpConsumableItem : ConsumableItem
{
    public override void Consume(GameObject target = null)
    {
        base.Consume(target);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerStatComponent playerStat = player.GetComponent<PlayerStatComponent>();
        playerStat.RecoveryHP(upValue);
    }
}
