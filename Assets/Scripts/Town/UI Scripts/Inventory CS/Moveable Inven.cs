using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveableInven : MonoBehaviour, IPointerDownHandler, IDragHandler
{
	[SerializeField] private Transform targetUI;
	[Space]
	[SerializeField] private Button btn_Sort;
	[SerializeField] private Button btn_Decom;
	[SerializeField] private Button btn_Close;

	// �κ� UI�� ��ġ �̵��� ���� ����
	private Vector2 beginPos;
	private Vector2 moveBegin;
	[SerializeField] [ReadOnly] private GameObject InvenUI;
	// �κ� UI�� ��ġ ���󺹱͸� ���� ����
	private Vector2 initPos;

	private void Awake()
	{
		Debug.Log("�κ��丮 ��� Ȱ��ȭ");

		if (targetUI == null) targetUI = transform.parent;
		if (InvenUI == null) InvenUI = targetUI.parent.gameObject;

		btn_Sort.onClick.AddListener(SortButton);
		btn_Decom.onClick.AddListener(DecomButton);
		btn_Close.onClick.AddListener(CloseInvenUI);

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
		if(InvenUI != null) InvenUI.SetActive(false);
	}

	private void SortButton()
	{
		Debug.Log("Item Sort Btn Click");
		InvenUI.GetComponent<InventoryUI>().SortItemList();
	}

	private void DecomButton()
	{
		Debug.Log("Item Decom Btn Click");
		InvenUI.GetComponent<InventoryUI>().DecomItems();
	}

	private void OnEnable()
	{
		targetUI.position = initPos;
	}
}
