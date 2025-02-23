using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform contentArea;

    [SerializeField] private TMP_Text goldText;
    private int goldAmount;

    [SerializeField] [ReadOnly] private List<ItemSlotUI> itemSlots;

    private void Awake()
    {
        /* contentArea 내의 인벤토리 슬롯들을 List : itemSlots 에 할당 */
        if (contentArea != null)
            itemSlots = new List<ItemSlotUI>(contentArea.GetComponentsInChildren<ItemSlotUI>());
		
        /* 슬롯들에 인덱스 넘버 부여 과정 */
		AssignSlotIndex();

        // DB에서 인벤토리 정보 받아오는 과정..?
		// AddItem(DB에서 받아온 정보)?

	}

    private class ItemSortComparer : IComparer<Item>
    {
        public int Compare(Item x, Item y)
        {
            int itemX = x.Data.ItemId;
            int itemY = y.Data.ItemId;

            return itemX - itemY;
        }
    }

    private static readonly ItemSortComparer _sortComparer = new();

    private void AssignSlotIndex()
    {
        for(int i = 0; i<itemSlots.Count; i++)
        {
            itemSlots[i].SetItemIndex(i);
        }
    }

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

            if (filledSlots.Count == 0)
            {
                Debug.Log("No Item in Slots");
                return -1;
            }

            filledSlots.Sort((a, b) => _sortComparer.Compare(a.GetItem(), b.GetItem()));

            // Sorting process validation code
            for (int i = 0; i < filledSlots.Count - 1; i++)
            {
                int currentId = filledSlots[i].GetItem().Data.ItemId;
                int nextId = filledSlots[i + 1].GetItem().Data.ItemId;

                if (currentId > nextId)
                {
                    Debug.LogWarning("Item Sort done to Not Correct Order");
                    SortItemList();
                    return -1;
                }
            }

            int index = 0;
            foreach (var slot in filledSlots)
                slot.transform.SetSiblingIndex(index++);
            foreach (var slot in emptySlots)
                slot.transform.SetSiblingIndex(index++);

            AssignSlotIndex();
			return 0;
        }
        catch (Exception ex)
        {
            Debug.LogError("Sort Item Method Error" + ex);
            return -1;
        }
    }

	#region
    public ItemSlotUI GetItemSlotByIndex(int index)
    {
        if(index < 0 || index > itemSlots.Count)
        {
            Debug.Log("Index Out of Range : ItemSlots" + index);
            return null;
        }
        return itemSlots[index];
    }
	#endregion
}
