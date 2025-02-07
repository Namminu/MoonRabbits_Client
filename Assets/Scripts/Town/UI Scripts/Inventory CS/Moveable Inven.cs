using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveableInven : MonoBehaviour, IPointerDownHandler, IDragHandler
{
	[SerializeField] private Transform targetUI;

	private Vector2 beginPos;
	private Vector2 moveBegin;
	
	private void Awake()
	{
		if (targetUI == null) targetUI = transform.parent;
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
}
