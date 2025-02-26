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
            Debug.Log("Gold Can't Under Zero");
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

            List<MaterialItem> itemList = itemSlots.Where(slot => slot.HasItem()).Select(slot => slot.GetItem()).ToList();
            itemList.Sort(_sortComparer.Compare);

            int index = 0;
            foreach(var item in itemList)
            {
                itemSlots[index].AddItem(item);
                index++;
            }

            for(int i = index; i<itemSlots.Count; i++)
            {
                itemSlots[i].ClearSlot();
            }

			return 0;
		}
		catch (Exception ex)
		{
			Debug.LogError("Sort Item Method Error: " + ex);
			return -1;
		}
	}

        /// <summary>
    /// InventoryManager에서 넘겨받은 inventoryDictionary를 바탕으로 UI를 갱신합니다.
    /// 각 ItemSlotUI의 슬롯 번호(SlotIndex)에 따라 해당 슬롯의 itemId와 stack을 업데이트하거나,
    /// 데이터가 없는 슬롯은 클리어합니다.
    /// </summary>
    /// <param name="inventoryDictionary">키가 슬롯 인덱스, 값이 InventorySlotData인 딕셔너리</param>
    public void RefreshInventory(Dictionary<int, MaterialItem> inventoryItems)
    {
        foreach (var slotUI in itemSlots)
        {
            int slotIndex = slotUI.GetItemIndex();
            if (inventoryItems.TryGetValue(slotIndex, out MaterialItem materialItem))
            {
                // MaterialItem을 전달하면 UpdateSlot 내부에서 ItemData를 활용하여 아이템 ID, 아이콘 등을 갱신합니다.
                slotUI.UpdateSlot(materialItem, materialItem.CurItemStack);
            }
            else
            {
                slotUI.ClearSlot();
            }
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
