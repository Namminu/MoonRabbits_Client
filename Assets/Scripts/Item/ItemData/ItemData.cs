using UnityEngine;

public abstract class ItemData : ScriptableObject
{
	[SerializeField] private int itemId;
	[SerializeField] private string itemName;
	[SerializeField] private string itemDescription;

	public int ItemId => itemId;					// ������ ID
	public string ItemName => itemName;				// ������ �̸�
	public string Description => itemDescription;	// ������ ����
}