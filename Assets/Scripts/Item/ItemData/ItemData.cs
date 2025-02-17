using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public abstract class ItemData : ScriptableObject
{
	[SerializeField] private string itemName;
	[SerializeField] private int itemId;
	[SerializeField] private Sprite itemIcon;
	[SerializeField] private string itemDescription;

	public string ItemName => itemName;
	public int ItemId => itemId;
	public Sprite ItemIcon => itemIcon;
	public string Description => itemDescription;
}