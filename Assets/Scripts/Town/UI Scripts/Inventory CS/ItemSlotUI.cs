using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, 
										IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
	[Header("Slot Position")]
	[Tooltip("�κ��丮UI ���� ������ ��� True ����/����UI ���� ������ ��� False ����")]
	[SerializeField] private bool isInven;

	[Header("Item Info")] 
    [SerializeField] private TMP_Text text_ItemAmount;  //������ ����
	[SerializeField] private Image itemImage;    //������ �̹���
	[SerializeField] [ReadOnly] private int itemCount;
    private Item item;  //ȹ�� ������ ��ü

	[Header("Slot Highlighter")]
	[SerializeField] private Image itemHighLighter;

	[Header("Popup UI")]
	[SerializeField] private GameObject PopupUI;
	private PopupUI popupUICs;
	 
	[SerializeField][ReadOnly]private TooltipUI tooltipUI;
	private RectTransform rectTransform;
	private GameObject DecomUI;
	private ItemSlotUI originSlot;

	private readonly string destroyText = "�ı��Ͻðڽ��ϱ�?";
	private static ItemSlotUI selectedSlotToDestroy;    // Ư�� ���Ը� �ı� �����ϵ��� �ڽ� ����
	private bool isSubscribe = false;					// ���� ������ �̺�Ʈ ���� ������ ���� 

	//[Header("Test Method : AddItem")]
	//[SerializeField] private GameObject itemLowLighter;

	// Start is called before the first frame update
	void Start()
    {
		tooltipUI = FindObjectOfType<TooltipUI>(true);
		rectTransform = GetComponent<RectTransform>();

		// DB���� �κ��丮 ���� �޾ƿ��� ����..?
		// AddItem(DB���� �޾ƿ� ����)?
	}

	// ������ ��� �� �̹��� ��ü�� ������ ������ ���� �޼���
	private void SetItemImageAlpha(float alpha)
	{
		Color newColor = itemImage.color;
		newColor.a = alpha;
		itemImage.color = newColor;
	}

	/// <summary>
	/// Add Item to Slot Method
	/// </summary>
	public void AddItem(Item insertItem, int insertItemCount = 1)
    {
        item = insertItem;
        itemCount = insertItemCount;
        /* //���� UI�� ������ �̹����� �߰��ϴ� ���� - Item �ڵ� �ϼ� �� DB �������� �ʿ�
		itemImage.sprite = insertItem.GetItemImage().sprite; */

        text_ItemAmount.text = insertItemCount.ToString();

        SetItemImageAlpha(1);
	}

	/// <summary>
	/// Destroy Item Method When EndDrag Over Inven UI
	/// </summary>
	private void OnDestroyItem()
	{
		if(selectedSlotToDestroy == this)
		{
			Debug.Log("Destory Item! " + item);
			// ClearSlot();
		}
		EventManager.Unsubscribe("OnDestroyItem", OnDestroyItem);
		selectedSlotToDestroy = null;
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
	private void ClearSlot()
	{
		item = null;
		itemCount = 0;
		itemImage.sprite = null;
		SetItemImageAlpha(0);

		text_ItemAmount.text = string.Empty;
	}

	private void ChangeSlot()
	{
		Item tempItem = item;
		int tempItemCount = itemCount;

		AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

		if (tempItem != null)
		{
			DragSlot.instance.dragSlot.AddItem(tempItem, tempItemCount);
		}
		else
		{
			DragSlot.instance.dragSlot.ClearSlot();
		}
		Debug.Log("Item Slot Change : " + this.name + DragSlot.instance.dragSlot.name);
	}

	private void UpdateTooltipUI()
	{
		//tooltipUI.SetItemDesc(item.GetItemData());
		tooltipUI.SetTooltipUIPos(rectTransform);
		tooltipUI.Show();
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
		if (item == null) return;

		// Image copy process when dragging starts from an item slot
		DragSlot.instance.dragSlot = this;
		DragSlot.instance.DragSetImage(itemImage);
		DragSlot.instance.transform.position = eventData.position;
	}
	public void OnDrag(PointerEventData eventData)
	{
		if (item == null) return;

		DragSlot.instance.transform.position = eventData.position;
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		DragSlot.instance.SetItemImageAlpha(0);
		DragSlot.instance.dragSlot = null;

		//if (item == null) return;
		if (!IsPointerInsideInventory(eventData)) SetPopupDestroy();
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (DragSlot.instance.dragSlot != null) ChangeSlot();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (item == null) return;

        if (eventData.button == PointerEventData.InputButton.Right)
		{
			DecomUI = GameObject.Find("UIDecomItem");
			
			if (isInven&&DecomUI.activeSelf) /* �κ��丮���� ��Ŭ�� ��  */
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
			else if(!isInven) /* ����â���� ��Ŭ�� �� */
			{
				Debug.Log("Right CLick On Decom Slot");
				if(originSlot != null)
				{
					originSlot.AddItem(item, itemCount);
					originSlot = null;
					ClearSlot();
				}
			}
		}
	}
	#endregion

	#region Getter
	public Item GetItem() {  return item; }
	public bool HasItem() {  return item != null; }
	#endregion

	#region Local Method

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
		else Debug.Log("ItemSlotUI : Not Found Inventory GameObject");

		if(tr_InvenUI == null) return false;

		return RectTransformUtility.RectangleContainsScreenPoint(
			tr_InvenUI, 
			eventData.position, 
			eventData.pressEventCamera);
	}

	private void SetPopupDestroy()
	{
		if (popupUICs == null)
		{
			Debug.Log("popupUICs null");
			return;
		}
		selectedSlotToDestroy = this;
		if(!isSubscribe)
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
		GameObject targetUI = _isInven ? GameObject.Find("Inventory") : GameObject.Find("UIDecomItem");
		if (targetUI == null) return null;

		ItemSlotUI[] slots = targetUI.GetComponentsInChildren<ItemSlotUI>(); // UI ������ ��� ���� ��������
		foreach (var slot in slots)
		{
			if (!slot.HasItem()) return slot; // �� ���� ã��
		}
		return null; // �� ���� ����
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
	}
	private void OnDisable()
	{
		if(selectedSlotToDestroy == this && isSubscribe)
		{
			EventManager.Unsubscribe("OnDestroyItem", OnDestroyItem);
			isSubscribe = false;
		}
	}
}