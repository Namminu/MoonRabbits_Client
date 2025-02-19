// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

public class MoveableInven : MonoBehaviour, IPointerDownHandler, IDragHandler
{
	[SerializeField] private Transform targetUI;
	[SerializeField] private GameObject DecomUI;
	[Space]
	[SerializeField] private Button btn_Sort;
	[SerializeField] private Button btn_Decom;
	[SerializeField] private Button btn_Close;

// 	// �κ� UI�� ��ġ �̵��� ���� ����
// 	private Vector2 beginPos;
// 	private Vector2 moveBegin;
// 	[SerializeField] [ReadOnly] private GameObject InvenUI;
// 	// �κ� UI�� ��ġ ���󺹱͸� ���� ����
// 	private Vector2 initPos;

//     // 인벤 UI의 위치 이동을 위한 변수
//     private Vector2 beginPos;
//     private Vector2 moveBegin;
//     private GameObject InvenUI;

// 		if (targetUI == null) targetUI = transform.parent;
// 		if (InvenUI == null) InvenUI = targetUI.parent.gameObject;

// 		btn_Sort.onClick.AddListener(SortButton);
// 		btn_Decom.onClick.AddListener(DecomButton);
// 		btn_Close.onClick.AddListener(CloseInvenUI);

//     private void Awake()
//     {
//         Debug.Log("인벤토리 헤더 활성화");

//         if (targetUI == null)
//             targetUI = transform.parent;
//         if (InvenUI == null)
//             InvenUI = targetUI.parent.gameObject;
//         btn_Close.onClick.AddListener(CloseInvenUI);

//         initPos = targetUI.position;
//     }

//     void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
//     {
//         beginPos = targetUI.position;
//         moveBegin = eventData.position;
//     }

// 	private void SortButton()
// 	{
// 		Debug.Log("Item Sort Btn Click");
// 		InvenUI.GetComponent<InventoryUI>().SortItemList();
// 	}

	private void DecomButton()
	{
		if(DecomUI != null)
		{
			if(DecomUI.activeSelf)
			{
				DecomUI.SetActive(false);
			}
			else DecomUI.SetActive(true);
		}
	}

// 	private void OnEnable()
// 	{
// 		targetUI.position = initPos;
// 	}
// }
