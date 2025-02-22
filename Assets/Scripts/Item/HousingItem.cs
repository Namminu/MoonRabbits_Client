using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingItem : Item
{
	public HousingItemData HsItemData;

	public HousingItem(HousingItemData data) : base(data)
	{
		HsItemData = data;
	}
}