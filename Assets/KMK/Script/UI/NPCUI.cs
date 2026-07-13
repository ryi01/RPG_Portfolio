using UnityEngine;
using UnityEngine.UI;

public class NPCUI : EnemyStatUI
{
    [SerializeField] private Image iconImage;

    public void SetIconSprites(Sprite sprite)
    {
        if (sprite == null) return;
        iconImage.sprite = sprite;
    }

    public void SetActiveIcon(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
