using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public abstract class ItemData : ScriptableObject
{
	[SerializeField] private int itemId;
	[SerializeField] private string itemName;
	[SerializeField] private Sprite itemIcon;
	[SerializeField] private string itemDescription;

	public int ItemId => itemId;
	public string ItemName => itemName;
	public Sprite ItemIcon => itemIcon;
	public string Description => itemDescription;
}