using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Housing Item")]
public class HousingItemData : ItemData
{
	[Header("Housing Item Attributes")]
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private Vector2Int itemGridSize;
	[SerializeField] private ItemDataTypes itemDataType;
	#region Public Members
	public GameObject ItemPrefab { get => itemPrefab; set => itemPrefab = value; }
	public Vector2Int ItemGridSize { get => itemGridSize; set => itemGridSize = value; }
	public ItemDataTypes ItemDataType { get => itemDataType; set => itemDataType = value; }
	#endregion
}
