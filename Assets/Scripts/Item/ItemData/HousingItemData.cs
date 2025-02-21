using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Housing Item")]
public class HousingItemData : ItemData
{
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private Vector2Int gridSize;

	public GameObject ItemPrefab => itemPrefab;
	public Vector2Int GridSize => gridSize;
}
