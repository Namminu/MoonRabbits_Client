using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HSItemButton : MonoBehaviour
{
	[SerializeField] private Image hsItemIcon;
	[SerializeField] public Button thisBtn;

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

		if (thisBtn.onClick == null) Debug.Log("onclick null error");

		thisBtn.onClick.RemoveAllListeners();
		thisBtn.onClick.AddListener(OnClickButtonEvent);
	}
	#endregion

	#region Private Method
	private void OnClickButtonEvent()
	{
		if (placementSystem == null)
		{
			Debug.Log("placementSystem Not Found");
			return;
		}
		placementSystem.StartPlacement(myItemId);
	}
	#endregion
}
