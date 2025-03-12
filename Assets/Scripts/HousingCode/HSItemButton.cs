using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HSItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TooltipManager를 통해 아이템 정보를 보여줍니다. 실제 아이템 데이터(이름, 설명 등)는 myItemId로 조회할 수 있습니다.
        TooltipManager.Instance.ShowTooltip(myItemId, eventData.position);
    }

    // IPointerExitHandler 구현: 마우스가 버튼을 벗어나면 툴팁 숨기기
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
