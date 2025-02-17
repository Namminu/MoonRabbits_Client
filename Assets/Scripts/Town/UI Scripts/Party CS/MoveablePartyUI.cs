using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveablePartyUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
	[SerializeField] private Transform targetUI;

	// �κ� UI�� ��ġ �̵��� ���� ����
	private Vector2 beginPos;
	private Vector2 moveBegin;
	[SerializeField][ReadOnly] private GameObject PartyUI;
	// �κ� UI�� ��ġ ���󺹱͸� ���� ����
	private Vector2 initPos;

	private void Awake()
	{
		Debug.Log("�κ��丮 ��� Ȱ��ȭ");

		if (targetUI == null) targetUI = transform.parent;
		if (PartyUI == null) PartyUI = targetUI.parent.gameObject;

		initPos = targetUI.position;
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		beginPos = targetUI.position;
		moveBegin = eventData.position;
	}

	void IDragHandler.OnDrag(PointerEventData eventData)
	{
		targetUI.position = beginPos + (eventData.position - moveBegin);
	}

	void CloseInvenUI()
	{
		if (PartyUI != null) PartyUI.SetActive(false);
	}

	private void SortButton()
	{
		Debug.Log("Item Sort Btn Click");
		PartyUI.GetComponent<InventoryUI>().SortItemList();
	}

	private void DecomButton()
	{
		Debug.Log("Item Decom Btn Click");
		PartyUI.GetComponent<InventoryUI>().DecomItems();
	}

	private void OnEnable()
	{
		targetUI.position = initPos;
	}
}
