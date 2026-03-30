using UnityEngine;
/* 전반적인 아이템 로직
 * 1. 데이터 생성 
 *     itemBox에서 itemList를 통해 무기나 소모품 추출 및 아이템 정보 목록 가짐
 * 2. ui 시각화 단계
 *     상자에 있는 리스트를 스캔해 데이터를 맵핑함 => itemUI 생성
 * 3. 상호작용 및 이동(Looting)
 *     인벤토리 시스템을 통해 공간을 확인하고 상자의 itemInfo를 넘김
 * 4. 아이템 이동 후 갱신
 *     상자 리스트 갱신, UI 삭제, 인벤토리 증가
 */
public class InputPickUp : MonoBehaviour
{
    [SerializeField] private GameObject itemBoxUI;
    private InventorySystem inventroySystem;
    private PlayerController pc;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
        inventroySystem = GameManager.Instance.InventroySystem;
    }
    public void OpenItemBox(ItemBox box)
    {
        if(box != null && inventroySystem != null)
        {
            GameManager.Instance.OpenBoxInfo(box);
        }
    }
    public void OpenStore(StoreData storeData)
    {
        GameManager.Instance.OpenStoreUI(storeData);
    }
    public void CloseStore()
    {
        GameManager.Instance.CloseStoreUI();
        if (pc != null) pc.CloseStore();
    }

    public void CloseUI()
    {
        itemBoxUI.SetActive(false);
        GameManager.Instance.CloseBoxInfo();
    }

}
