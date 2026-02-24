using UnityEngine;

public class InputPickUp : MonoBehaviour
{
    [SerializeField] private GameObject itemBoxUI;
    private InventroySystem inventroySystem;

    private void Start()
    {
        inventroySystem = FindAnyObjectByType<InventroySystem>();
    }
    public void OpenItemBox(ItemBox box)
    {
        if(box != null && inventroySystem != null)
        {
            itemBoxUI.SetActive(true);
            itemBoxUI.GetComponentInChildren<ItemBoxUI>().SetupBoxUI(box);
            /*ItemInfo info = box.ItemInfo;

            itemBoxUI.SetActive(true);*/
        }
    }

    public void CloseUI()
    {
        itemBoxUI.SetActive(false);
    }

}
