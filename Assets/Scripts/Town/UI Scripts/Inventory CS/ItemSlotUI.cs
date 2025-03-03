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
    [SerializeField] [ReadOnly] private int itemIndex;  // 현재 슬롯의 인벤토리 내 인덱스
    private MaterialItem item; //획득 아이템 객체

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

    [Header("Test Method : AddItem")] 
    [SerializeField] private MaterialItemData TestItemData;
    [SerializeField] private int testItemCount;

    // Start is called before the first frame update
    void Start()
    {
        tooltipUI = FindObjectOfType<TooltipUI>(true);
        rectTransform = GetComponent<RectTransform>();
    }

    public void ReturnToOriginSlot()
    {
        if (originSlot != null)
        {
			originSlot.AddItem(item);
			originSlot = null;
			ClearSlot();
		}
	}

    /// <summary>
    /// Add Item to Slot Method
    /// </summary>
    public void AddItem(MaterialItem insertItem)
    {
        if (insertItem == null) return;

        /* 이미 존재하는 아이템일 경우 병합 */
        if(item != null && item.Data.ItemId == insertItem.Data.ItemId)
        {
            item.CurItemStack += insertItem.CurItemStack;
			text_ItemAmount.text = item.CurItemStack.ToString();
            return;
		}

		/* 새 아이템 정보 업데이트 */
		item = insertItem;
        itemImage.sprite = insertItem.ItemData.ItemIcon;
        text_ItemAmount.text = insertItem.CurItemStack.ToString();

        SetItemImageAlpha(1);
    }

    /// <summary>
    /// Update Item Count Method, When Count Under/Equal 0, auto call ClearSlot()
    /// </summary>
    public int UpdateItemCount(int itemAmount)
    {
        if (item == null) return -1;

        item.CurItemStack += itemAmount;
        if (item.CurItemStack <= 0)
        {
            ClearSlot();
            return -1;
        }
        text_ItemAmount.text = item.CurItemStack.ToString();
        return item.CurItemStack;
    }

    /// <summary>
    /// Clear Item Slot Method
    /// </summary>
    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        text_ItemAmount.text = string.Empty;

        SetItemImageAlpha(0);
    }

    #region Mouse Event
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) return;

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

		if (eventData.button == PointerEventData.InputButton.Right) return;

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
        if (DragSlot.instance.dragSlot == null) return;

        MaterialItem draggedItem = DragSlot.instance.dragSlot.GetItem();
        if (draggedItem == null) return;

        if (HasItem() && item.Data.ItemId == draggedItem.Data.ItemId)
        {
			item.CurItemStack += draggedItem.CurItemStack;
			text_ItemAmount.text = item.CurItemStack.ToString();
			DragSlot.instance.dragSlot.ClearSlot();
		}  
        else ChangeSlot();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null) return;
		
        #region Right Mouse Click
		if (eventData.button == PointerEventData.InputButton.Right)
        {
			DecomUI = GameObject.Find("UIDecomItem");

            if (DecomUI != null && DecomUI.activeSelf) /* 분해창이 활성화 되어 있을 시 */
            {
                if (isInven) /* 인벤토리에서 우클릭 시  */
                {
                    Debug.Log("Right Click On Inventory Slot");
                    ItemSlotUI emptySlot = FindEmptySlot(false);
                    if (emptySlot != null)
                    {
                        emptySlot.AddItem(item);
                        emptySlot.originSlot = this;
                        ClearSlot();
                    }
                    else return;
                }
                else if (!isInven) /* 분해창에서 우클릭 시 */
                {
                    Debug.Log("Right Click On Decom Slot");
                    ReturnToOriginSlot();
                }
            }
            else if(DecomUI == null || !DecomUI.activeSelf) /* 분해창 비활성화 때 우클릭 시 아이템 분리 */
			{
				SeperatingItem(); 
			} 
		}
		#endregion

		#region Left Mouse Click
		if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
		#endregion
	}
	#endregion

	#region Getter n Setter
	public MaterialItem GetItem()
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

	/// <summary>
	/// Seperate Cur Slot Item Count Method 
	/// </summary>
	/// <returns>seperated item count, If the number of items is odd, a smaller value is returned</returns>
	private int SeperatingItem()
    {
        if (item.CurItemStack <= 1) return -1;

        Debug.Log("Seperate Item Method");
        int seperateLeftCount = item.CurItemStack / 2;
        item.CurItemStack -= seperateLeftCount;
		text_ItemAmount.text = item.CurItemStack.ToString();

		MaterialItem tempItem = new MaterialItem(item.ItemData, seperateLeftCount);
        DragSlot.instance.dragSlot = this;

        DragSlot.instance.DragSetImage(itemImage);
        DragSlot.instance.SetItemImageAlpha(1);

        DragSlot.instance.dragSlot.AddItem(tempItem);

		return seperateLeftCount;
    }

	/* 아이템 등록 시 이미지 객체의 투명도 조절을 위한 메서드 */
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
			ClearSlot();
		}
		EventManager.Unsubscribe("OnDestroyItem", OnDestroyItem);
		selectedSlotToDestroy = null;
	}

	private void ChangeSlot()
	{
        if (DragSlot.instance.dragSlot == null) return;

        MaterialItem draggedItem = DragSlot.instance.dragSlot.GetItem();
        MaterialItem targetItem = item;

        if (draggedItem == null) return;

        /* 같은 종류의 아이템일 경우 합침 */
        if(targetItem != null && targetItem.Data.ItemId == draggedItem.Data.ItemId)
        {
            targetItem.CurItemStack += draggedItem.CurItemStack;
            DragSlot.instance.dragSlot.ClearSlot();
        }
        else /* 다른 종류의 아이템일 경우 교체 */
        {
			MaterialItem tempItem = item;
			int tempItenIndex = itemIndex;

			AddItem(DragSlot.instance.dragSlot.item);
			SetItemIndex(DragSlot.instance.dragSlot.GetItemIndex());

			if (tempItem != null)
			{
				DragSlot.instance.dragSlot.AddItem(tempItem);
				SetItemIndex(DragSlot.instance.dragSlot.SetItemIndex(tempItenIndex));
			}
			else
			{
				DragSlot.instance.dragSlot.ClearSlot();
			}
		}

		Debug.Log($"Item Slot Change: {this.name} ({itemIndex}) ↔ {DragSlot.instance.dragSlot.name} ({DragSlot.instance.dragSlot.GetItemIndex()})");
	}

	private void UpdateTooltipUI()
	{
		tooltipUI.SetItemDesc(item.ItemData);
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


    #if UNITY_EDITOR
	public void AddTestItem()
    {
        if (TestItemData != null)
        {
            MaterialItem newTestItem = new MaterialItem(TestItemData, testItemCount);
            AddItem(newTestItem);
        }
    }
    #endif
}
