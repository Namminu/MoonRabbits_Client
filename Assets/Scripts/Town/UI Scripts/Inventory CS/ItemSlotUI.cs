using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IPointerClickHandler
{
    [Header("Slot Position")]
    [Tooltip("인벤토리UI 안의 슬롯일 경우 True 세팅/분해UI 안의 슬롯일 경우 False 세팅")]
    [SerializeField] private bool isInven;

    [Header("Item Info")]
    [SerializeField] private TMP_Text text_ItemAmount; //아이템 수량

    [SerializeField] private Image itemImage; //아이템 이미지

    [SerializeField] [ReadOnly] private int itemCount;
    [SerializeField][ReadOnly] private int itemIndex;
    private Item item; //획득 아이템 객체

    [Header("Slot Highlighter")]
    [SerializeField] private Image itemHighLighter;

    [Header("Popup UI")]
    [SerializeField] private GameObject PopupUI;
    private PopupUI popupUICs;

    [SerializeField] [ReadOnly] private TooltipUI tooltipUI;
    private RectTransform rectTransform;
    private GameObject DecomUI;
    private ItemSlotUI originSlot;

    private readonly string destroyText = "파괴하시겠습니까?";
    private static ItemSlotUI selectedSlotToDestroy; //특정 슬롯만 파괴 수행하도록 자신 참조
    private bool isSubscribe = false; // 현재 슬롯이 이벤트 구독 중인지 여부

    //[Header("Test Method : AddItem")]
    //[SerializeField] private GameObject itemLowLighter;

    // Start is called before the first frame update
    void Start()
    {
        tooltipUI = FindObjectOfType<TooltipUI>(true);
        rectTransform = GetComponent<RectTransform>();

        // DB에서 인벤토리 정보 받아오는 과정..?
        // AddItem(DB에서 받아온 정보)?
    }

    public void ReturnToOriginSlot()
    {
        if (originSlot != null)
        {
			originSlot.AddItem(item, itemCount);
			originSlot = null;
			ClearSlot();
		}
    }

    /// <summary>
    /// Add Item to Slot Method
    /// </summary>
    public void AddItem(Item insertItem, int insertItemCount = 1)
    {
        item = insertItem;
        itemCount = insertItemCount;
        /* //슬롯 UI에 아이템 이미지를 추가하는 과정 - Item 코드 완성 및 DB 연동과정 필요
        itemImage.sprite = insertItem.GetItemImage().sprite; */

        text_ItemAmount.text = insertItemCount.ToString();

        SetItemImageAlpha(1);
    }

    /// <summary>
    /// Update Item Count Method, When Count Under/Equal 0, auto call ClearSlot()
    /// </summary>
    public int UpdateItemCount(int newItemCount)
    {
        itemCount += newItemCount;
        if (itemCount <= 0)
        {
            ClearSlot();
            return -1;
        }
        text_ItemAmount.text = itemCount.ToString();
        return itemCount;
    }

    /// <summary>
    /// Clear Item Slot Method
    /// </summary>
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetItemImageAlpha(0);

        text_ItemAmount.text = string.Empty;
    }

    #region Mouse Event
    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (item == null) return;

        itemHighLighter.gameObject.SetActive(true);
        UpdateTooltipUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemHighLighter.gameObject.SetActive(false);
        tooltipUI.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        // Image copy process when dragging starts from an item slot
        DragSlot.instance.dragSlot = this;
        DragSlot.instance.DragSetImage(itemImage);
        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetItemImageAlpha(0);
        DragSlot.instance.dragSlot = null;

        //if (item == null) return;
        if (!IsPointerInsideInventory(eventData))
            SetPopupDestroy();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
            ChangeSlot();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DecomUI = GameObject.Find("UIDecomItem");

            if (isInven && DecomUI.activeSelf) /* 인벤토리에서 우클릭 시  */
            {
                Debug.Log("Right Click On Inventory Slot");
                ItemSlotUI emptySlot = FindEmptySlot(false);
                if (emptySlot != null)
                {
                    emptySlot.AddItem(item, itemCount);
                    emptySlot.originSlot = this;
                    ClearSlot();
                }
                else return;
            }
            else if (!isInven) /* 분해창에서 우클릭 시 */
            {
                Debug.Log("Right CLick On Decom Slot");
                if (originSlot != null)
                {
                    originSlot.AddItem(item, itemCount);
                    originSlot = null;
                    ClearSlot();
                }
            }
        }
    }
    #endregion

    #region Getter n Setter
    public Item GetItem()
    {
        return item;
    }

    public bool HasItem()
    {
        return item != null;
    }
    public int GetItemIndex()
    {
        return itemIndex;
	}
    public ItemSlotUI GetOriginSlot()
    {
        return originSlot;
    }

    public ItemSlotUI SetOriginSlot(ItemSlotUI slot)
    {
        originSlot = slot;
        return originSlot;
    }
    public int SetItemIndex(int index)
    {
        itemIndex = index;
        return itemIndex;
    }
	#endregion

	#region Local Method

	// 아이템 등록 시 이미지 객체의 투명도 조절을 위한 메서드
	private void SetItemImageAlpha(float alpha)
	{
		Color newColor = itemImage.color;
		newColor.a = alpha;
		itemImage.color = newColor;
	}

	/// <summary>
	/// Destroy Item Method When EndDrag Over Inven UI
	/// </summary>
	private void OnDestroyItem()
	{
		if (selectedSlotToDestroy == this)
		{
			Debug.Log("Destory Item! " + item);
			// ClearSlot();
		}
		EventManager.Unsubscribe("OnDestroyItem", OnDestroyItem);
		selectedSlotToDestroy = null;
	}

	private void ChangeSlot()
	{
		Item tempItem = item;
		int tempItemCount = itemCount;
        int tempItenIndex = itemIndex;

		AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
        SetItemIndex(DragSlot.instance.dragSlot.GetItemIndex());

		if (tempItem != null)
		{
			DragSlot.instance.dragSlot.AddItem(tempItem, tempItemCount);
			SetItemIndex(DragSlot.instance.dragSlot.SetItemIndex(tempItenIndex));
		}
		else
		{
			DragSlot.instance.dragSlot.ClearSlot();
		}
		Debug.Log($"Item Slot Change: {this.name} ({itemIndex}) ↔ {DragSlot.instance.dragSlot.name} ({DragSlot.instance.dragSlot.GetItemIndex()})");
	}

	private void UpdateTooltipUI()
	{
		//tooltipUI.SetItemDesc(item.GetItemData());
		tooltipUI.SetTooltipUIPos(rectTransform);
		tooltipUI.Show();
	}

	/// <summary>
	/// Return True When eventData Out of Inventory UI Range
	/// </summary>
	private bool IsPointerInsideInventory(PointerEventData eventData)
    {
        RectTransform tr_InvenUI = null;
        GameObject Inventory = GameObject.Find("Inventory");
        if (Inventory != null)
        {
            tr_InvenUI = Inventory.GetComponent<RectTransform>();
        }
        else
            Debug.Log("ItemSlotUI : Not Found Inventory GameObject");

        if (tr_InvenUI == null)
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            tr_InvenUI,
            eventData.position,
            eventData.pressEventCamera
        );
    }

    private void SetPopupDestroy()
    {
        if (popupUICs == null)
        {
            Debug.Log("popupUICs null");
            return;
        }
        selectedSlotToDestroy = this;
        if (!isSubscribe)
        {
            EventManager.Subscribe("OnDestroyItem", OnDestroyItem);
            isSubscribe = true;
        }

        popupUICs.SetPopupUI(destroyText, "OnDestroyItem");
    }

    /// <summary>
    /// Find Empty Slot to Change Item Position In Inventory or Decom
    /// </summary>
    /// <param name="_isInven">True : in Inventory, False : In Decom</param>
    /// <returns>Empty Slot</returns>
    private ItemSlotUI FindEmptySlot(bool _isInven)
    {
        GameObject targetUI = _isInven
            ? GameObject.Find("Inventory")
            : GameObject.Find("UIDecomItem");
        if (targetUI == null)
            return null;

        ItemSlotUI[] slots = targetUI.GetComponentsInChildren<ItemSlotUI>(); // UI 내부의 모든 슬롯 가져오기
        foreach (var slot in slots)
        {
            if (!slot.HasItem())
                return slot; // 빈 슬롯 찾기
        }
        return null; // 빈 슬롯 없음
    }
    #endregion

    private void OnEnable()
    {
        if (PopupUI == null)
        {
            Debug.Log("PopupUI null");
            return;
        }
        else
            popupUICs = PopupUI.GetComponent<PopupUI>();
    }

    private void OnDisable()
    {
        if (selectedSlotToDestroy == this && isSubscribe)
        {
            EventManager.Unsubscribe("OnDestroyItem", OnDestroyItem);
            isSubscribe = false;
        }
    }
}
