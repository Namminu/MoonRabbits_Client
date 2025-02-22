using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialItem : Item
{
	public MaterialItemData MtItemData;
	private int curItemStack;

	public MaterialItem(MaterialItemData data, int amount = 1) : base(data)
	{
		MtItemData = data;
		curItemStack = amount;
	}
} 