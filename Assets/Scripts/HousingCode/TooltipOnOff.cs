using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipOnOff : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
	[SerializeField] private GameObject TooltipUI;

	public void OnPointerEnter(PointerEventData eventData)
	{
		TooltipUI.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		TooltipUI.SetActive(false);
	}
}
