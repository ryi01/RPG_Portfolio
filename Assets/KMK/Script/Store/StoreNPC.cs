using UnityEngine;

public class StoreNPC : BaseNPCInteraction
{
    [SerializeField] private int storeId;
    [SerializeField] private Sprite storeSprite;
    [SerializeField]
    [Range(0f, 1f)] protected float clipVolume = 0.5f;
    [SerializeField] protected AudioClip clip;
    public int StoreId => storeId;
    protected override void RefreshNPCUI()
    {
        if (storeSprite != null) SetIcon(storeSprite);
        else HideIcon();
    }
    public override void Interact(PlayerController player)
    {
        if(clip != null)GameManager.Instance.SoundManager.PlayImpactSFX(clip, clipVolume);
        player.MovementComp.LookAtInstant((transform.position - player.transform.position).normalized);
        player.OpenStore(this);
    }
}
