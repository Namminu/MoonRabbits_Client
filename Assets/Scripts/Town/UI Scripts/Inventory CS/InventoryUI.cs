using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
// using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
	[SerializeField] private Transform contentArea;

	[SerializeField] private TMP_Text goldText;
	private int goldAmount;

	[SerializeField] [ReadOnly] private List<ItemSlotUI> itemSlots;

	private void Awake()
	{
		if (contentArea != null) itemSlots = new List<ItemSlotUI>(contentArea.GetComponentsInChildren<ItemSlotUI>());	
	}

	/// <summary> Definition Variable of Sort Weight </summary>
	private readonly static Dictionary<Type, int> sortWeightDict = new()
	{
		{typeof(CountableItem), 0 },
		{typeof(UncountableItem), 1},
	};
	private class ItemSortComparer : IComparer<Item>
	{
		public int Compare(Item x, Item y)
		{
			int itemX = GetSortWeight(x);
			int itemY = GetSortWeight(y);

			return itemY - itemX;
		}
		public int GetSortWeight(Item item)
		{
			int weight = 0;
			if(sortWeightDict.TryGetValue(item.GetType(), out int typeWeight))
			{
				weight += typeWeight;
			}
			return weight;
		}
	}
	private readonly static ItemSortComparer _sortComparer = new();

	private int UpdateGold(int newGoldAmount)
	{
		if(newGoldAmount < 0)
		{
			Debug.Log("Gold Cant Under Zero");
			goldAmount = 0;
			return -1;
		}
		else goldAmount = newGoldAmount;

		goldText.text = goldAmount.ToString();
		return goldAmount;
	}

	public int SortItemList()
	{
		Debug.Log("Item Sort Start");
		try
		{
			if (itemSlots == null || itemSlots.Count == 0)
			{
				Debug.Log("Item Slots List NULL");
				return -1;
			}
			List<ItemSlotUI> filledSlots = itemSlots.Where(slot => slot.HasItem()).ToList();
			List<ItemSlotUI> emptySlots = itemSlots.Where(slot => !slot.HasItem()).ToList();

			if(filledSlots.Count == 0)
			{
				Debug.Log("No Item in Slots");
				return -1;
			}

			filledSlots.Sort((a, b) => _sortComparer.Compare(a.GetItem(), b.GetItem()));

			// Sorting process validation code 
			for (int i = 0; i < filledSlots.Count - 1; i++)
			{
				int currentWeight = _sortComparer.GetSortWeight(filledSlots[i].GetItem());
				int nextWeight = _sortComparer.GetSortWeight(filledSlots[i + 1].GetItem());

				if (currentWeight < nextWeight)
				{
					Debug.LogWarning("Not Correct Order in Sorting");
					return -1;
				}
			}

			int index = 0;
			foreach (var slot in filledSlots) slot.transform.SetSiblingIndex(index++);
			foreach (var slot in emptySlots) slot.transform.SetSiblingIndex(index++);

			return 0;
		}
		catch (Exception ex)
		{
			Debug.LogError("Sort Item Method Error" + ex);
			return -1;
		}
	}

	public int DecomItems()
	{
		Debug.Log("Item Decom : " /* + item.name */);
		return -1;
	}
}
