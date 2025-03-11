using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Seperate UI")]
    [SerializeField] private GameObject SepeUI;
    private SeperateUI sepeUICS;

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
    public void AddItem(MaterialItem insertItem, bool skipTransmit = false)
    {
        if (insertItem == null)
            return;

        int maxStackSize = 9999; // 최대 스택 크기

        // 이미 슬롯에 같은 아이템이 존재하면 병합 처리
        if (item != null && item.Data.ItemId == insertItem.Data.ItemId)
        {
            int totalStack = item.CurItemStack + insertItem.CurItemStack;
            if (totalStack <= maxStackSize)
            {
                // 병합하여 수량 업데이트
                item.CurItemStack = totalStack;
                text_ItemAmount.text = item.CurItemStack.ToString();
            }
            else
            {
                // 최대 스택 초과 처리
                item.CurItemStack = maxStackSize;
                text_ItemAmount.text = maxStackSize.ToString();
                int restStack = totalStack - maxStackSize;
                MaterialItem overflowItem = new MaterialItem(insertItem.ItemData, restStack);

                InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
                if (inventoryUI != null)
                {
                    inventoryUI.AddRemainingItems(overflowItem);
                }
            }

            // 병합 후 상태 전송 (초기화 시에는 생략)
            if (!skipTransmit)
            {
                InventoryUI updatedUI = FindObjectOfType<InventoryUI>();
                if (updatedUI != null)
                {
                    updatedUI.OnInventoryStateChanged();
                }
            }
            return;
        }

        // 슬롯이 비어 있을 경우 새 아이템 할당
        item = insertItem;
        itemImage.sprite = insertItem.ItemData.ItemIcon;
        text_ItemAmount.text = item.CurItemStack.ToString();
        SetItemImageAlpha(1);

        // 아이템 추가 후 상태 전송 (초기화 시에는 생략)
        if (!skipTransmit)
        {
            InventoryUI invUI = FindObjectOfType<InventoryUI>();
            if (invUI != null)
            {
                invUI.OnInventoryStateChanged();
            }
        }
    }

    public void SetItemDirectly(MaterialItem newMaterialItem)
    {
        // 초기화 시 슬롯 데이터를 직접 설정
        item = newMaterialItem;
        if (item != null)
        {
            itemImage.sprite = item.ItemData.ItemIcon;
            text_ItemAmount.text = item.CurItemStack.ToString();
            SetItemImageAlpha(1);
        }
        else
        {
            ClearSlot();
        }
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

        // 슬롯의 이미지는 그대로 유지하되, 투명도를 낮춰서 드래그 중임을 표시
        SetItemImageAlpha(0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 후 원래 슬롯 이미지의 투명도를 복원
        SetItemImageAlpha(1);
        DragSlot.instance.SetItemImageAlpha(0);
        DragSlot.instance.dragSlot = null;

        //if (item == null) return;
        if (!IsPointerInsideInventory(eventData))
            SetPopupDestroy();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot == null) return;

        // 만약 자기 자신(원래 슬롯)에 드랍되는 경우, 단순히 투명도를 복구하고 종료
        if (DragSlot.instance.dragSlot == this)
        {
            SetItemImageAlpha(1);
            return;
        }

        // 드래그 중인 아이템 가져오기
        MaterialItem draggedItem = DragSlot.instance.dragSlot.GetItem();
        if (draggedItem == null) return;

        // 현재 슬롯(대상)의 아이템 가져오기
        if (HasItem() && item.Data.ItemId == draggedItem.Data.ItemId)
        {
            // 같은 아이템일 경우 병합 처리
            AddItem(draggedItem);
            // 드래그 중인 슬롯(원본)을 명확히 비움
            DragSlot.instance.dragSlot.ClearSlot();
        }
        else
        {
            // 다른 아이템일 경우 교체 처리
            ChangeSlot();
        }

        // 상태 변경 후 서버로 전송
        InventoryUI invUI = FindObjectOfType<InventoryUI>();
        if (invUI != null)
        {
            invUI.OnInventoryStateChanged();
        }
    }

    /*  public void OnDrop(PointerEventData eventData)
      {
          if (DragSlot.instance.dragSlot == null) return;

          MaterialItem draggedItem = DragSlot.instance.dragSlot.GetItem();
          if (draggedItem == null) return;

          if (HasItem() && item.Data.ItemId == draggedItem.Data.ItemId)
          {
              AddItem(draggedItem);
              DragSlot.instance.dragSlot.ClearSlot();
          }  
          else ChangeSlot();
      }*/

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
                        emptySlot.AddItem(item, true);
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
		}
		#endregion

		#region Left Mouse Click
		if (eventData.button == PointerEventData.InputButton.Left)
        {
			if (Input.GetKey(KeyCode.LeftShift))
			{
				Debug.Log("Shift + Right Click");
				SeperatingItem();
			}
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
	private void SeperatingItem()
    {
        if (item.CurItemStack <= 1) return;

        sepeUICS.Seperating(item.CurItemStack);
        EventManager.Subscribe<int>("OnSeperationConfirm", OnSeperationConfirm);
    }

    private void OnSeperationConfirm(int seperatedStack)
    {
        item.CurItemStack -= seperatedStack;
        text_ItemAmount.text = item.CurItemStack.ToString();

        MaterialItem seperatedItem = new(item.ItemData, seperatedStack);

        ItemSlotUI emptySlot = InventoryUI.GetEmptySlot();
        if(emptySlot != null)
        {
            emptySlot.AddItem(seperatedItem);
        }
        else
        {
            Debug.Log("Inventory Slot Full");
        }
		EventManager.Unsubscribe<int>("OnSeperationConfirm", OnSeperationConfirm);
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
        if (DragSlot.instance.dragSlot == null)
            return;

        // 드래그 중인 슬롯(원본)에서 아이템을 가져옴
        MaterialItem draggedItem = DragSlot.instance.dragSlot.GetItem();
        if (draggedItem == null)
            return;

        // 현재 슬롯(대상)의 아이템을 임시 변수에 보관
        MaterialItem targetItem = this.item;

        // 원본 슬롯은 미리 클리어하여 중복 표시를 방지
        DragSlot.instance.dragSlot.ClearSlot();

        // 대상 슬롯에 드래그한 아이템을 할당 및 UI 갱신
        SetItem(draggedItem);

        // 대상 슬롯에 기존에 아이템이 있었다면 원본 슬롯으로 이동(또는 빈 슬롯으로 남김)
        if (targetItem != null)
        {
            // 원본(드래그 중인) 슬롯에 이전 대상 아이템을 설정합니다.
            // 만약 교체가 아닌 단순 이동이라면 이 부분을 생략하고 원본을 그냥 비워두면 됩니다.
            DragSlot.instance.dragSlot.SetItem(targetItem);
        }

        Debug.Log($"Item Slot Swap 완료: {this.name} ↔ {DragSlot.instance.dragSlot.name}");
    }

    // SetItem 메서드: 슬롯에 아이템을 할당하고 UI를 갱신하는 헬퍼 함수
    public void SetItem(MaterialItem newItem)
    {
        item = newItem;
        if (item != null)
        {
            itemImage.sprite = item.ItemData.ItemIcon;
            text_ItemAmount.text = item.CurItemStack.ToString();
            SetItemImageAlpha(1);
        }
        else
        {
            ClearSlot();
        }

        // 상태 변경 후 기본적으로 InventoryUI에 변경을 알림
        InventoryUI invUI = FindObjectOfType<InventoryUI>();
        if (invUI != null)
        {
            invUI.OnInventoryStateChanged();
        }
    }

    /*private void ChangeSlot()
	{
        if (DragSlot.instance.dragSlot == null) return;

        MaterialItem draggedItem = DragSlot.instance.dragSlot.GetItem();
        MaterialItem targetItem = item;

        if (draggedItem == null) return;

        *//* 같은 종류의 아이템일 경우 합침 *//*
        if(targetItem != null && targetItem.Data.ItemId == draggedItem.Data.ItemId)
        {
			AddItem(draggedItem);
		}
        else *//* 다른 종류의 아이템일 경우 교체 *//*
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
	}*/

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
        else popupUICs = PopupUI.GetComponent<PopupUI>();

        if (SepeUI == null)
        {
            Debug.Log("SepeUI null");
            return;
        }
        else sepeUICS = SepeUI.GetComponent<SeperateUI>();
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
