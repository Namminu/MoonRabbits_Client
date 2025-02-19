using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
	public static DragSlot instance;
	public ItemSlotUI dragSlot;

	[SerializeField] private Image itemImage;

	private void Awake()
	{
		instance = this;
	}

	public void DragSetImage(Image _itemImage)
	{
		itemImage.sprite = _itemImage.sprite;
		SetItemImageAlpha(1);
	}

	public void SetItemImageAlpha(float alpha)
	{
		Color newColor = itemImage.color;
		newColor.a = alpha;
		itemImage.color = newColor;
	}
}
