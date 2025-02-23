using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecomUI : MonoBehaviour
{
	[SerializeField] private GameObject SlotArea;
	[SerializeField] private Button btn_Decom;

	[SerializeField][ReadOnly] private List<ItemSlotUI> itemSlots;

	[SerializeField] private GameObject PopupUI;
	private PopupUI popupUICs;

	private readonly string decomText = "분해하시겠습니까?";

	private void Awake()
	{
		btn_Decom.onClick.AddListener(SetPopupToDecom);
		if (SlotArea != null) itemSlots = new List<ItemSlotUI>(SlotArea.GetComponentsInChildren<ItemSlotUI>());
	}

	private void SetPopupToDecom()
	{
		if (popupUICs == null)
		{
			Debug.Log("popupUICs NULL");
			return;
		}
		Debug.Log("Decom Call Popup");
		popupUICs.SetPopupUI(decomText, "OnDecomItem");
	}

	private void OnDecomItem()
	{
		Debug.Log("Decom UI : Item Decom Start");
		foreach(var slot in itemSlots)
		{
			slot.ClearSlot();
		}
		/* 아이템 분해 로직 */
	}

	private void ReturnItemsToInven()
	{
		Debug.Log("Decom UI : Item Return To Inven");
		foreach (var slot in itemSlots)
		{
			if (slot.HasItem()) slot.ReturnToOriginSlot();
		}
	}

	private void OnEnable()
	{
		popupUICs = PopupUI.GetComponent<PopupUI>();
		EventManager.Subscribe("OnDecomItem", OnDecomItem);
	}

	private void OnDisable()
	{
		ReturnItemsToInven();
		EventManager.Unsubscribe("OnDecomItem", OnDecomItem);
	}
}
