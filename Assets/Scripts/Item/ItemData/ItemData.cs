using UnityEngine;

public abstract class ItemData : ScriptableObject
{
	[SerializeField] private int itemId;
	[SerializeField] private string itemName;
	[SerializeField] private string itemDescription;

	public int ItemId => itemId;					// 아이템 ID
	public string ItemName => itemName;				// 아이템 이름
	public string Description => itemDescription;	// 아이템 설명
}