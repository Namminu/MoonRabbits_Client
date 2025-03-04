using Google.Protobuf.Protocol;
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
        DontDestroyOnLoad(gameObject);

        if (contentArea != null)
            itemSlots = new List<ItemSlotUI>(contentArea.GetComponentsInChildren<ItemSlotUI>());

		AssignSlotIndex();
	}

    /// <summary>
    /// 현재 인벤토리 상태를 수집해 서버로 전송하는 메서드
    /// </summary>

    public void OnInventoryStateChanged()
    {
        // itemSlots에는 UI상의 모든 슬롯(예: 25칸)이 들어있다고 가정합니다.
        List<MaterialItem> currentItems = new List<MaterialItem>();

        for (int i = 0; i < itemSlots.Count; i++)
        {
            // 각 슬롯에 대해 item이 있다면 해당 아이템을, 없으면 기본값을 생성합니다.
            MaterialItem matItem = null;
            if (itemSlots[i].HasItem())
            {
                matItem = itemSlots[i].GetItem();
            }

            // 빈 슬롯이면 기본 MaterialItem 생성 (아이템 ID 0, 수량 0)
            // MaterialItem 생성자나 프로퍼티가 없다면 아래와 같이 null 대신 별도의 래퍼 클래스로 처리할 수 있음
            if (matItem == null)
            {
                // 임시로 MaterialItemData를 생성해 기본값(ItemId 0)을 전달합니다.
                matItem = new MaterialItem(new MaterialItemData { ItemId = 0 }, 0);
            }

            // 각 아이템에 현재 슬롯 인덱스를 반드시 기록합니다.
            matItem.SlotIdx = i;
            currentItems.Add(matItem);
        }

        if (InventoryManager.instance != null)
            InventoryManager.instance.SendItemMove(currentItems);
        else
            Debug.LogWarning("InventoryManager 인스턴스가 존재하지 않습니다.");
    }

    /*public void OnInventoryStateChanged()
    {
        List<MaterialItem> currentItems = new List<MaterialItem>();
        foreach (var slot in itemSlots)
        {
            if (slot.HasItem())
                currentItems.Add(slot.GetItem());
        }

        if (InventoryManager.instance != null)
            InventoryManager.instance.SendItemMove(currentItems);
        else
            Debug.LogWarning("InventoryManager 인스턴스가 존재하지 않습니다.");
    }*/

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
            // 현재 아이템 리스트 가져오기
            List<MaterialItem> itemList = itemSlots
                .Where(slot => slot.HasItem())
                .Select(slot => slot.GetItem())
                .ToList();

            // 병합 아이템 리스트 생성 및 병합 로직
            List<MaterialItem> mergedItemList = new List<MaterialItem>();
            foreach (var item in itemList)
            {
                int itemId = item.Data.ItemId;
                int maxStack = item.ItemData.ItemMaxStack;
                int remainStack = item.CurItemStack;

                // 기존 병합 리스트에서 동일한 아이템 찾기
                var targetItem = mergedItemList.FirstOrDefault(mergedItem =>
                    mergedItem.Data.ItemId == itemId && mergedItem.CurItemStack < maxStack);

                if (targetItem != null)
                {
                    int spaceLeft = maxStack - targetItem.CurItemStack;
                    if (remainStack <= spaceLeft)
                    {
                        targetItem.CurItemStack += remainStack; // 병합 성공
                        remainStack = 0;
                    }
                    else
                    {
                        targetItem.CurItemStack = maxStack; // 최대 스택 채움
                        remainStack -= spaceLeft;
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

            // 정렬 수행 (아이템 ID 기준)
            mergedItemList.Sort((x, y) => x.Data.ItemId.CompareTo(y.Data.ItemId));

            // 기존 슬롯 초기화
            foreach (var slot in itemSlots)
            {
                slot.ClearSlot();
            }

            // 정렬된 아이템을 첫 번째 슬롯부터 할당
            for (int i = 0; i < mergedItemList.Count; i++)
            {
                if (i < itemSlots.Count)
                {
                    itemSlots[i].AddItem(mergedItemList[i]);
                }
                else
                {
                    Debug.LogWarning("슬롯 개수가 부족하여 일부 아이템이 할당되지 않았습니다.");
                    break;
                }
            }

            // 정렬된 결과를 서버로 전송
            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.SendInventorySort(mergedItemList);
            }
            else
            {
                Debug.LogWarning("InventoryManager 인스턴스가 존재하지 않음");
            }

            return 0; // 성공
        }
        catch (Exception ex)
        {
            Debug.LogError("Sort Item Method Error: " + ex);
            return -1; // 실패
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
        foreach (var slot in itemSlots)
        {
            int slotIndex = slot.GetItemIndex();
            if (inventoryItems.TryGetValue(slotIndex, out MaterialItem materialItem))
            {
                // 초기 로딩 시에는 서버 전송 없이 UI 갱신만 수행
                slot.SetItemDirectly(materialItem);
            }
            else
            {
                slot.ClearSlot();
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
