using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemCountText;

    private Item item = null;
    private Action onDoubleClick;

    // ¾ÆÀÌÅÛ ¼¿ ÃÊ±âÈ­
    public void ClearItemUI()
    {
        itemImage.sprite = null;
        itemCountText.text = "0";
        onDoubleClick = null;
    }
    // ¾ÆÀÌÅÛ È¹µæ½Ã
    public void InitItemUI(Item item, Action doubleClickAction)
    {
        this.item = item;
        this.onDoubleClick = doubleClickAction;

        if (this.item.ItemIconImage != null) itemImage.sprite = this.item.ItemIconImage;
        itemCountText.text = this.item.ItemCount > 0 ? this.item.ItemCount.ToString() : "";
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(eventData.clickCount == 2)
            {
                onDoubleClick?.Invoke();
            }
        }
    }

    private void UseItem()
    {

    }
}
