using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private int slotIndex;
    public int SlotIndex { get => slotIndex; set => slotIndex = value; }
    // 아이템 이미지 및 텍스트 표시
    [SerializeField] private Image itemImage;
    [SerializeField] private Image selectImage;
    [SerializeField] private Text itemCountText;

    private InventorySystem inventorySystem;

    // 드래그 시 최상단 출력을 위한 캔버스 참조
    private Canvas canvas;
    // UI 위치제어
    private RectTransform rectTransform;
    // 투명도 및 마우스 ray 제어
    private CanvasGroup canvasGroup;
    // 원래 슬롯
    private Transform originParent;

    // 잡고있는 itemInfo의 실제 데이터
    public Item item { get; private set; }
    private Action onClick;
    private Action onDoubleClick;

    private EnumTypes.ItemUIMode currentMode = EnumTypes.ItemUIMode.Use;

    private bool isFromInven;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        // 드래그 중 마우스 검사를 확인하기 위함
        canvasGroup = GetComponent<CanvasGroup>() == null ? gameObject.AddComponent<CanvasGroup>(): GetComponent<CanvasGroup>();
        ClearItemUI();
    }

    public void InitInventory(InventorySystem system)
    {
        inventorySystem = system;
    }
    public void SetSelect(bool isSelected)
    {
        selectImage.gameObject.SetActive(isSelected);
    }
    public void SetMode(EnumTypes.ItemUIMode mode)
    {
        currentMode = mode;
    }
    private bool IsDrag()
    {
        return currentMode == EnumTypes.ItemUIMode.Use ||
            currentMode == EnumTypes.ItemUIMode.BoxLoot ||
            currentMode == EnumTypes.ItemUIMode.BoxStorage;
    }

    private bool IsDoubleClick()
    {
        return currentMode == EnumTypes.ItemUIMode.Use || currentMode == EnumTypes.ItemUIMode.StoreBuy;
    }
    // 아이템 셀 초기화
    public void ClearItemUI()
    {
        item = null;
        itemImage.sprite = null;
        itemCountText.text = "";
        itemImage.color = new Color(1, 1, 1, 0); // 투명
        selectImage.gameObject.SetActive(false);
        onClick = null;
        onDoubleClick = null;
    }
    // 아이템 획득시
    public void InitItemUI(Item item, Action clickAction = null, Action doubleClickAction = null)
    {
        this.item = item;
        this.onClick = clickAction;
        this.onDoubleClick = doubleClickAction;
        if (this.item != null && this.item.ItemIconImage != null)
        {
            itemImage.sprite = this.item.ItemIconImage;
            itemImage.color = Color.white;
            itemCountText.text = this.item.ItemCount > 0 ? this.item.ItemCount.ToString() : "";
        }
        else ClearItemUI();
    }

    // 더블 클릭 확인
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (eventData.clickCount == 2)
        {
            if(IsDoubleClick()) onDoubleClick?.Invoke();
        }
        else if (eventData.clickCount == 1) onClick?.Invoke();
    }
    #region Drag&Drop
    // 마우스 왼클릭 끌기 시작 시 호출됨
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDrag()) return;
        if (item == null) return;

        // 원래 위치와 슬롯 기억
        originParent = transform.parent;
        isFromInven = GetComponentInParent<InventoryUI>() != null;

        // 좌표 유지를 위해 월드 포지션 기억 후, 부모 변경
        Vector3 worldPos = transform.position;
        transform.SetParent(canvas.transform);
        transform.position = worldPos;

        // blcokRaycast를 끄고 들고 있는 ui가 아닌 바닥의 슬롯을 인식
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }
    // 드래그 중에 호출
    public void OnDrag(PointerEventData eventdata)
    {
        if (!IsDrag()) return;
        if (item == null) return;
        // 마우스의 이동량만큼 ui위치 실시간으로 변경
        rectTransform.anchoredPosition += eventdata.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDrag()) return;
        if (item == null) return;
        // 드래그 종료 후 상태 복구
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        // 마우스를 땐 위치 확인
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        bool isSucess = HandleDrop(target);

        if (!isSucess)
        {
            ReturnToOrigin();
        }
        else
        {
            // 미리 부모를 돌려 놓기
            transform.SetParent(originParent);
            rectTransform.anchoredPosition = Vector2.zero;
            GameManager.Instance.UIManager.UpdateItemBoxUI();
            GameManager.Instance.UIManager.UpdateInventoryUI();
        }
    }
    private bool HandleDrop(GameObject target)
    {
        if (target == null) return false;
        Debug.Log($"{target.tag} : {target.name}");
        // 인벤토리쪽
        if (target.CompareTag("ItemSlot"))
        {
            // 인벤토리 내부
            if (isFromInven)
            {
                // target의 태그에 따라 위치 변경
                ItemUI targetUI = target.GetComponentInChildren<ItemUI>();
                // 위치 변경
                if (targetUI != null && targetUI != this)
                {
                    inventorySystem.SwapItems(this.slotIndex, targetUI.slotIndex);
                    return true;
                }
            }
            // 박스 -> 인벤토리
            else return LootToInven();
        }
        // 인벤토리 -> 박스
        else if (target.CompareTag("BoxSlot"))
        {
            if (isFromInven) return MoveToBox();
        }
        else if (isFromInven && target.CompareTag("Trash"))
        {
            inventorySystem.RemoveItem(item);
            return true;
        }

        return false;
    }
    private void ReturnToOrigin()
    {
        transform.SetParent(originParent);
        rectTransform.anchoredPosition = Vector2.zero;
    }
    private bool MoveToBox()
    {
        // 현재 열린 박스를 가져와서
        ItemBox box = inventorySystem.CurrentBox;
        if (box == null) return false;
        // 박스 list에 추가
        box.AddItemFromInventroy(item);
        // 인벤토리에서 삭제
        inventorySystem.RemoveItem(item);

        return true;
    }

    private bool LootToInven()
    {
        // 현재 박스에서
        ItemBox box = inventorySystem.CurrentBox;
        if (box == null) return false;
        // 들고있는 아이템의 info값을 가져와서
        ItemInfo info = new ItemInfo
        {
            itemId = item.ItemID,
            itemCount = item.ItemCount,
            ItemType = item.ItemType
        };
        // 인벤토리에 추가해주고
        if(inventorySystem.AddItem(info))
        {
            // 박스에서 제거하기
            box.RemoveItemFromBox(info);
            return true;
        }
        return false;  
    }
    #endregion
}
