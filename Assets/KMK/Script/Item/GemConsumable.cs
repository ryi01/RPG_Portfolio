using UnityEngine;

public class GemConsumable : ConsumableItem
{
    public override void Consume(GameObject target = null)
    {
        base.Consume(target);

        GameObject actor = target != null ? target : GameObject.FindGameObjectWithTag("Player");
        if (actor != null)
        {
            GameManager.Instance.DataManager.Gold += UpValue;
        }
    }
}
