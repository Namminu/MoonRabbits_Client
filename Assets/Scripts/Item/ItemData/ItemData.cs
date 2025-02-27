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
	public int ItemId { get => itemId; set => itemId = value; }
	public string ItemName { get => itemName; set => itemName = value; }
	public string ItemDescription { get => itemDescription; set => itemDescription = value; }
	public ItemTypes ItemType { get => itemType; set => itemType = value; }
	public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
	#endregion
}