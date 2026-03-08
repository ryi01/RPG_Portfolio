using UnityEngine;
[CreateAssetMenu(fileName = "ExpConsumable", menuName = "Item/ExpConsumable")]
public class ExpPortionItem : ConsumableItem
{
    public override void Consume(GameObject target = null)
    {
        base.Consume(target);

        GameObject actor = target != null ? target : GameObject.FindGameObjectWithTag("Player");
        if (actor != null)
        {
            if (actor.TryGetComponent<PlayerStatComponent>(out var playerStat))
            {
                playerStat.TakeExp(UpValue);
            }
            else Debug.LogError("PlayerStatComponent를 찾을 수 없습니다!");
        }
    }
}
