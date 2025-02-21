using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialItem : Item
{
	public MaterialItemData MtItemData { get; set; }
	public MaterialItem(MaterialItemData data, int amount = 1) : base(data)
	{
		MtItemData = data;
	}
} 