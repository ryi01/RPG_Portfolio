using UnityEngine;

public class StoreNPC : BaseNPCInteraction
{
    [SerializeField] private StoreData storeData;
    [SerializeField] private Sprite storeSprite;

    public StoreData StoreData => storeData;
    protected override void RefreshNPCUI()
    {
        if (storeSprite != null) SetIcon(storeSprite);
        else HideIcon();
    }
    public override void Interact(PlayerController player)
    {
        player.MovementComp.LookAtInstant((transform.position - player.transform.position).normalized);
        player.OpenStore(this);
    }
}
