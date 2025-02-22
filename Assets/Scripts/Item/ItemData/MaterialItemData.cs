using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Material Item")]
public class MaterialItemData : ItemData
{
	[Header("Material Item Attributes")]
	[SerializeField] private int itemMaxStack;

	#region Public Members
	public int ItemMaxStack => itemMaxStack;
	#endregion
}
