using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HSItemButton : MonoBehaviour
{
	[SerializeField] private Image hsItemIcon;
	[SerializeField] private Button thisBtn;

	private PlacementSystem placementSystem;
	private int myItemId; 

	private void Awake()
	{
		placementSystem = GameObject.Find("PlacementSystem").GetComponent<PlacementSystem>();	
	}

	#region Public Method
	public void InitializeButton(Sprite newSprite, int itemId)
	{
		hsItemIcon.sprite = newSprite;
		myItemId = itemId;

		//thisBtn.onClick.RemoveAllListeners();
		thisBtn.onClick.AddListener(OnClickButtonEvent);

		Debug.Log($"[DEBUG] ��ư �̺�Ʈ ��� Ȯ�� - ���� �̺�Ʈ ����: {thisBtn.onClick.GetPersistentEventCount()}");
	}
	#endregion

	#region Private Method
	private void OnClickButtonEvent()
	{
		Debug.Log("������Ʈ ������ ����" + myItemId);
		if (placementSystem == null)
		{
			Debug.Log("placementSystem Not Found");
			return;
		}
		placementSystem.StartPlacement(myItemId);

	}
	#endregion
}
