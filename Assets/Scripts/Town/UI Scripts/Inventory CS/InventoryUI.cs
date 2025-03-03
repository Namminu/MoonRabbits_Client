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

			/* 같은 아이템 합치기 */
			Dictionary<int, int> itemStackCount = new();
			Dictionary<int, MaterialItemData> itemDataMap = new();

			foreach (var item in itemList)
			{
				int itemId = item.Data.ItemId;
				if (itemStackCount.ContainsKey(itemId))
				{
					itemStackCount[itemId] += item.CurItemStack; // 아이템 개수 합산
				}
				else
				{
					itemStackCount[itemId] = item.CurItemStack;
					itemDataMap[itemId] = (MaterialItemData)item.Data; // 아이템 데이터 저장
				}
			}

			/* 병합된 아이템 리스트 생성 */
			List<MaterialItem> mergedItemList = itemStackCount
				.Select(kvp => new MaterialItem(itemDataMap[kvp.Key], kvp.Value))
				.ToList();

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
