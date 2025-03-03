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


	/// <summary>
	/// InventoryManager���� �Ѱܹ��� inventoryDictionary�� �������� UI�� �����մϴ�.
	/// �� ItemSlotUI�� ���� ��ȣ(SlotIndex)�� ���� �ش� ������ itemId�� stack�� ������Ʈ�ϰų�,
	/// �����Ͱ� ���� ������ Ŭ�����մϴ�.
	/// </summary>
	/// <param name="inventoryDictionary">Ű�� ���� �ε���, ���� InventorySlotData�� ��ųʸ�</param>
	public void RefreshInventory(Dictionary<int, MaterialItem> inventoryItems)
    {
        foreach (var slotUI in itemSlots)
        {
            int slotIndex = slotUI.GetItemIndex();
            if (inventoryItems.TryGetValue(slotIndex, out MaterialItem materialItem))
            {
                // MaterialItem�� �����ϸ� UpdateSlot ���ο��� ItemData�� Ȱ���Ͽ� ������ ID, ������ ���� �����մϴ�.
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
