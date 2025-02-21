using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Material Item")]
public class MaterialItemData : ItemData
{
	[SerializeField] private Sprite itemIcon;

	public Sprite ItemIcon => itemIcon;
}
