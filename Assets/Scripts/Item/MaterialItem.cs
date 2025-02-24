using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialItem : Item
{
	private int curItemStack;
	public MaterialItemData ItemData => base.Data as MaterialItemData;
	public int CurItemStack {
		get => curItemStack;
		set => curItemStack = Mathf.Clamp(value, 0, ItemData.ItemMaxStack);
	}
	public MaterialItem(MaterialItemData data, int amount = 1) : base(data)
	{
		curItemStack = amount;
	}
} 