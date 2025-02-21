using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingItem : Item
{
	public HousingItemData HsItemData { get; private set; }

	public HousingItem(HousingItemData data, int amount = 1) : base(data)
	{
		HsItemData = data;
	}
}