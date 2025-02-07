using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
	[Header("Inven Options")]
	[Range(0, 10)]
	[SerializeField] private int heightSlotCount = 0;
	[Range(0, 10)]
	[SerializeField] private int widthSlotCount = 0;
	[SerializeField] private float slotMargin = 8f;
	[SerializeField] private float slotInitPadding = 10f;
	[Range(32, 64)]
	[SerializeField] private float slotSize = 64f;

	[Header("Connected Objects")]
	[SerializeField] private RectTransform contentAreaRT;
	[SerializeField] private GameObject slotPrefab;

	private void InitSlots()
	{
		slotPrefab.TryGetComponent(out RectTransform slotRect);
		slotRect.sizeDelta = new Vector2(slotSize, slotSize);

		slotPrefab.TryGetComponent(out ItemSlotUI itemSlot);
		if (itemSlot == null) slotPrefab.AddComponent<ItemSlotUI>();

		slotPrefab.SetActive(false);
	}
}
