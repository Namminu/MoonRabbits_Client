using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Build;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform contentArea;

    [SerializeField] private TMP_Text goldText;
    private int goldAmount;

    [SerializeField] [ReadOnly] private List<ItemSlotUI> itemSlots;
    private bool hasInitialized = false;

    private void Awake()
    {
        if (contentArea != null)
            itemSlots = new List<ItemSlotUI>(contentArea.GetComponentsInChildren<ItemSlotUI>());

		AssignSlotIndex();
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

    // public int SortItemList()
    // {
    //     Debug.Log("Item Sort Start");
    //     try
    //     {
    //         if (itemSlots == null || itemSlots.Count == 0)
    //         {
    //             Debug.Log("Item Slots List NULL");
    //             return -1;
    //         }

    //         // 1. 슬롯에 존재하는 MaterialItem들을 가져와 정렬합니다.
    //         List<MaterialItem> itemList = itemSlots
    //             .Where(slot => slot.HasItem())
    //             .Select(slot => slot.GetItem())
    //             .ToList();
    //         itemList.Sort(_sortComparer.Compare);

    //         // 2. 정렬된 아이템들을 순서대로 슬롯에 배치하고 슬롯 인덱스도 갱신합니다.
    //         int index = 0;
    //         foreach (var item in itemList)
    //         {
    //             itemSlots[index].AddItem(item);
    //             itemSlots[index].SetItemIndex(index);  // 슬롯 인덱스 재할당 (필요한 경우)
    //             index++;
    //         }

    //         // 3. 남은 슬롯은 초기화합니다.
    //         for (int i = index; i < itemSlots.Count; i++)
    //         {
    //             itemSlots[i].ClearSlot();
    //         }

    //         // 4. 정렬된 리스트를 InventoryManager의 SendInventorySort를 통해 서버에 전송합니다.
    //         if (InventoryManager.instance != null)
    //         {
    //             InventoryManager.instance.SendInventorySort(itemList);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("InventoryManager instance is null. Cannot send sorted inventory.");
    //         }

    //         return 0;
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError("Sort Item Method Error: " + ex);
    //         return -1;
    //     }
    // }

    /// <summary>
    /// InventoryManager에서 전달받은 인벤토리 데이터를 기반으로 UI 슬롯을 갱신합니다.
    /// inventoryItems는 각 슬롯 번호에 매핑된 MaterialItem 인스턴스를 포함합니다.
    /// </summary>
    /// <param name="inventoryItems">키: 슬롯 번호, 값: MaterialItem 인스턴스</param>
    public void RefreshInventory(Dictionary<int, MaterialItem> inventoryItems)
	public int SortItemList()
	{
		try
		{
			if (itemSlots == null || itemSlots.Count == 0)
			{
				Debug.Log("Item Slots List NULL");
				return -1;
			}

			/* 인벤토리 내 아이템 탐색 */
			List<MaterialItem> itemList = itemSlots.Where(slot => slot.HasItem()).Select(slot => slot.GetItem()).ToList();

			/* 병합 아이템 리스트 */
			List<MaterialItem> mergedItemList = new List<MaterialItem>();
			foreach(var item in itemList)
			{
				int itemId = item.Data.ItemId;
				int maxStack = item.ItemData.ItemMaxStack;
				int remainStack = item.CurItemStack;

				bool isMerged = false;
				foreach (var mergedItem in mergedItemList)
				{
					if (mergedItem.Data.ItemId == itemId && mergedItem.CurItemStack < maxStack)
					{
						int spaceLeft = maxStack - mergedItem.CurItemStack;
						if (remainStack <= spaceLeft)
						{
							mergedItem.CurItemStack += remainStack;
							isMerged = true;
							break;
						}
						else
						{
							mergedItem.CurItemStack = maxStack;
							remainStack -= spaceLeft;
						}
					}
				}
				// 남은 개수가 있으면 새 스택 생성
				while (remainStack > 0)
				{
					int stackToAdd = Mathf.Min(remainStack, maxStack);
					mergedItemList.Add(new MaterialItem((MaterialItemData)item.Data, stackToAdd));
					remainStack -= stackToAdd;
				}
			} 

			/* 정렬 */
			mergedItemList.Sort(_sortComparer.Compare);

			/* 기존 슬롯 초기화 */
			foreach (var slot in itemSlots)
			{
				slot.ClearSlot(); // 기존 아이템을 먼저 지움
			}

			/* 정렬된 아이템을 첫 번째 슬롯부터 할당 */
			int index = 0;
			foreach (var item in mergedItemList)
			{
				itemSlots[index].AddItem(item); 
				index++;
			}
			return 0;
		}
		catch (Exception ex)
		{
			Debug.LogError("Sort Item Method Error: " + ex);
			return -1;
		}
	}

	public bool AddItem(MaterialItem item)
	{
		foreach (var slot in itemSlots)
		{
			/* 슬롯에 추가되는 아이템과 동일한 아이템이 있을 경우 */
			if (slot.HasItem() && slot.GetItem().Data.ItemId == item.ItemData.ItemId)
			{
				MaterialItem existItem = slot.GetItem();
				int maxStack = item.ItemData.ItemMaxStack;

				if (existItem.CurItemStack < maxStack)
				{
					int restStack = maxStack - existItem.CurItemStack;
					if (item.CurItemStack <= restStack)
					{
						existItem.CurItemStack += item.CurItemStack;
						slot.UpdateItemCount(existItem.CurItemStack);
						return true;
					}
					else
					{
						existItem.CurItemStack = maxStack;
						slot.UpdateItemCount(maxStack);
						item.CurItemStack -= restStack;
						break;
					}
				}
			}
		}

		// 빈 슬롯을 찾아 아이템 추가
		foreach (var slot in itemSlots)
		{
			if (!slot.HasItem())
			{
				slot.AddItem(item);
				return true;
			}
		}

		Debug.Log("Inventory Full. Cannot Add Item.");
		return false;
	}

	public void AddRemainingItems(MaterialItem remainingItem)
	{
		foreach (var slot in itemSlots)
		{
			if (!slot.HasItem()) // 빈 슬롯 찾기
			{
				slot.AddItem(remainingItem);
				return;
			}
		}
		Debug.Log("Inventory Full. Cannot Add Remaining Item.");
	}

	public void RefreshInventory(Dictionary<int, MaterialItem> inventoryItems)
    {
        foreach (var slotUI in itemSlots)
        {
            int slotIndex = slotUI.GetItemIndex();
            if (inventoryItems.TryGetValue(slotIndex, out MaterialItem materialItem))
            {
                // MaterialItem을 전달하면 UpdateSlot 내부에서 ItemData를 활용하여 아이템 ID, 아이콘 등을 갱신합니다.
                slotUI.AddItem(materialItem);
            }
            else
          {
                slotUI.ClearSlot();
          }
        }
    }

    private void OnEnable()
    {
        if (hasInitialized)
            return;  // 이미 초기화된 경우 실행하지 않음

        hasInitialized = true;

        // InventoryManager에 저장된 인벤토리 데이터를 불러와 갱신
        if (InventoryManager.instance != null)
        {
            RefreshInventory(InventoryManager.instance.GetCurrentInventoryDictionary());
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
