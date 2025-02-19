using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Transform contentArea;

    [SerializeField]
    private TMP_Text goldText;
    private int goldAmount;

    [SerializeField]
    [ReadOnly]
    private List<ItemSlotUI> itemSlots;

    private void Awake()
    {
        if (contentArea != null)
            itemSlots = new List<ItemSlotUI>(contentArea.GetComponentsInChildren<ItemSlotUI>());
    }

	private class ItemSortComparer : IComparer<Item>
	{
		public int Compare(Item x, Item y)
		{
			int itemX = x.GetItemData().ItemId;
			int itemY = y.GetItemData().ItemId;

			return itemX - itemY;
		}
	}
	private readonly static ItemSortComparer _sortComparer = new();

            return itemY - itemX;
        }

        public int GetSortWeight(Item item)
        {
            int weight = 0;
            if (sortWeightDict.TryGetValue(item.GetType(), out int typeWeight))
            {
                weight += typeWeight;
            }
            return weight;
        }
    }

    private static readonly ItemSortComparer _sortComparer = new();

    private int UpdateGold(int newGoldAmount)
    {
        if (newGoldAmount < 0)
        {
            Debug.Log("Gold Cant Under Zero");
            goldAmount = 0;
            return -1;
        }
        else
            goldAmount = newGoldAmount;

        goldText.text = goldAmount.ToString();
        return goldAmount;
    }

			// Sorting process validation code 
			for (int i = 0; i < filledSlots.Count - 1; i++)
			{
				int currentId = filledSlots[i].GetItem().GetItemData().ItemId;
				int nextId = filledSlots[i + 1].GetItem().GetItemData().ItemId;

				if (currentId > nextId)
				{
					Debug.LogWarning("Item Sort done to Not Correct Order");
					SortItemList();
					return -1;
				}
			}

            filledSlots.Sort((a, b) => _sortComparer.Compare(a.GetItem(), b.GetItem()));

			return 0;
		}
		catch (Exception ex)
		{
			Debug.LogError("Sort Item Method Error" + ex);
			return -1;
		}
	}
}
