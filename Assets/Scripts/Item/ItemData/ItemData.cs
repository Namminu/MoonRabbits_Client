using UnityEngine;

public enum ItemTypes
{
	HousingItem,
	MaterialItem
}

public abstract class ItemData : ScriptableObject
{
	[Header("Item Info")]
	[SerializeField] private int itemId;
	[SerializeField] private string itemName;
	[SerializeField] private string itemDescription;
	[SerializeField] private ItemTypes itemType;

	[Header("Item Sprite")]
	[SerializeField] private Sprite itemIcon;


	#region Public Members
	public int ItemId => itemId;
	public string ItemName => itemName;
	public string ItemDescription => itemDescription;
	public ItemTypes ItemType => itemType;
	public Sprite ItemIcon => itemIcon;
	#endregion
}