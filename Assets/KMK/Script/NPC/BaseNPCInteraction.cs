using UnityEngine;

public abstract class BaseNPCInteraction : InteractionObject
{
    [SerializeField] protected string uiCanvasRootName = "NPCUI";
    [SerializeField] protected float yOffset = 4;
    [SerializeField] protected GameObject uiPrefab; // 머리 위 아이콘 오브젝트

    protected Transform uiCanvasRoot;
    protected NPCUI npcUI;

    protected virtual void Start()
    {
        InitNPCUI();
    }

    protected void InitNPCUI()
    {
        if (string.IsNullOrEmpty(uiCanvasRootName) || uiPrefab == null) return;
        GameObject rootObj = GameObject.Find(uiCanvasRootName);
        if (rootObj == null) return;
        uiCanvasRoot = rootObj.transform;
        npcUI = Instantiate(uiPrefab, uiCanvasRoot).GetComponent<NPCUI>();
        npcUI.SetUpUi(transform, yOffset);

        RefreshNPCUI();
    }

    protected void SetIcon(Sprite icon)
    {
        if (npcUI == null) return;
        npcUI.SetActiveIcon(icon != null);
        if (icon != null) npcUI.SetIconSprites(icon);
    }

    protected void HideIcon()
    {
        if (npcUI == null) return;
        npcUI.SetActiveIcon(false);
    }

    protected abstract void RefreshNPCUI();
}
