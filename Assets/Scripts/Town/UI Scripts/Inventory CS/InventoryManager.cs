using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }  // 싱글톤 인스턴스

    // 인벤토리 슬롯 데이터. key: 슬롯 인덱스, value: MaterialItem
    private Dictionary<int, MaterialItem> inventoryDictionary  = new Dictionary<int, MaterialItem>();

    public Dictionary<int, MaterialItem> GetCurrentInventoryDictionary()
    {
        return inventoryDictionary;
    }

    public List<MaterialItem> GetCurrentInventoryList()
    {
        List<MaterialItem> list = new List<MaterialItem>();
        foreach (var key in inventoryDictionary.Keys.OrderBy(k => k))
        {
            // 슬롯이 빈 경우는 null로 처리하거나, 필요한 기본값 처리 가능
            list.Add(inventoryDictionary[key]);
        }
        return list;
    }

    // InventoryUI.cs에서 인벤토리 슬롯들을 관리하는 컴포넌트 (InventoryUI.cs 파일 참조; 내용 확인 불가한 경우 Inspector에 할당)
    [SerializeField]
    private InventoryUI inventoryUI;

    private void Awake()
    {
        // 이미 인스턴스가 존재하면 중복 생성 방지
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // 필요한 초기화 코드 추가 가능
    }

    /// <summary>
    /// 서버에서 전달받은 S2CInventoryUpdate 패킷을 기반으로 인벤토리 데이터를 업데이트합니다.
    /// 각 슬롯에 대해 MaterialItem 인스턴스를 생성하여 inventoryDictionary에 저장하고,
    /// InventoryUI의 RefreshInventory 메서드를 호출하여 UI도 갱신합니다.
    /// </summary>
    /// <param name="pkt">S2CInventoryUpdate 패킷</param>
    public void UpdateInventoryData(S2CInventoryUpdate pkt)
    {
        inventoryDictionary.Clear();

        foreach (var slot in pkt.Slots)
        {
            if (slot == null || slot.ItemId == 0) // 빈 슬롯 처리
                continue;

            MaterialItemData itemData = ItemDataLoader.MaterialItemsList
                .Find(item => item.ItemId == slot.ItemId);
            if (itemData != null)
            {
                MaterialItem materialItem = new MaterialItem(itemData, slot.Stack);
                // 같은 키가 있을 경우 값을 업데이트함으로써 예외 발생을 방지
                inventoryDictionary[slot.SlotIdx] = materialItem;
            }
            else
            {
                Debug.LogWarning("아이템 데이터가 존재하지 않음. ItemId: " + slot.ItemId);
            }
        }

        Debug.Log("S2CInventoryUpdate 패킷 처리 완료: " + pkt);

        // UI 갱신 호출 (서버 전송 없이 초기화만 수행)
        if (inventoryUI != null)
        {
            inventoryUI.RefreshInventory(inventoryDictionary);
        }
    }

    public void UpdateInventorySlot(int slotIdx, MaterialItem newItem)
    {
        if (inventoryDictionary.ContainsKey(slotIdx))
        {
            // 이미 해당 슬롯에 데이터가 있다면 덮어씌웁니다.
            inventoryDictionary[slotIdx] = newItem;
        }
        else
        {
            // 해당 슬롯이 없으면 새 항목으로 추가합니다.
            inventoryDictionary.Add(slotIdx, newItem);
        }

        // (옵션) 인벤토리 UI를 갱신하려면 아래와 같이 InventoryUI에 RefreshInventory를 호출할 수 있습니다.
        if (inventoryUI != null)
        {
            inventoryUI.RefreshInventory(inventoryDictionary);
        }
    }

    // ----------------------------
    // 인벤토리 관련 패킷 전송 함수들
    // ----------------------------

    /// <summary>
    /// 아이템 획득 패킷 전송 함수
    /// </summary>
    /// <param name="slotIdx">슬롯 인덱스</param>
    /// <param name="itemId">아이템 아이디</param>
    public void SendItemObtained(int slotIdx, int itemId)
    {
        C2SItemObtained packet = new C2SItemObtained
        {
            SlotIdx = slotIdx,
            ItemId = itemId
        };
        GameManager.Network.Send(packet);
    }

    /// <summary>
    /// 아이템 분해 패킷 전송 함수
    /// </summary>
    public void SendItemDisassembly(int slotIdx, int itemId)
    {
        C2SItemDisassembly packet = new C2SItemDisassembly
        {
            SlotIdx = slotIdx,
            ItemId = itemId
        };
        GameManager.Network.Send(packet);
    }

    /// <summary>
    /// 아이템 파괴 패킷 전송 함수
    /// </summary>
    public void SendItemDestroy(int slotIdx, int itemId)
    {
        C2SItemDestroy packet = new C2SItemDestroy
        {
            SlotIdx = slotIdx,
            ItemId = itemId
        };
        GameManager.Network.Send(packet);
    }

    /// <summary>
    /// 인벤토리 정렬 패킷 전송 함수
    /// sortedSlots: 정렬된 인벤토리 슬롯 데이터를 리스트 형태로 전달
    /// </summary>
    public void SendInventorySort(List<MaterialItem> sortedSlots)
    {
        C2SInventorySort packet = new C2SInventorySort();
        foreach (var materialItem in sortedSlots)
        {
            // MaterialItem의 필요한 값을 사용해 InventorySlot 객체 생성
            InventorySlot newSlot = new InventorySlot
            {
                SlotIdx = materialItem.SlotIdx,
                ItemId = materialItem.ItemData.ItemId,
                Stack = materialItem.CurItemStack
            };
            packet.Slots.Add(newSlot);
        }
    GameManager.Network.Send(packet);
    }

    // 인벤토리 이동(변경) 패킷 전송 함수
    public void SendItemMove(List<MaterialItem> slotsStatus)
    {
        C2SItemMove packet = new C2SItemMove();

        // 인벤토리의 모든 25슬롯을 0부터 24까지 순서대로 전송
        // 각 슬롯에는 해당 인덱스의 MaterialItem이 존재하면 아이템 정보를, 없으면 빈 슬롯(아이템 0, 스택 0) 정보 삽입
        for (int i = 0; i < 25; i++)
        {
            // 슬롯 리스트에서 현재 인덱스와 일치하는 MaterialItem 검색 (없으면 null)
            MaterialItem materialItem = slotsStatus.FirstOrDefault(item => item != null && item.SlotIdx == i);
            InventorySlot newSlot = new InventorySlot
            {
                SlotIdx = i,
                ItemId = materialItem != null ? materialItem.ItemData.ItemId : 0,
                Stack = materialItem != null ? materialItem.CurItemStack : 0
            };

            packet.Slots.Add(newSlot);
        }

        GameManager.Network.Send(packet);
    }


    #region 인벤토리 클라 내부 로직
    public bool AddItemToInven(MaterialItem newItem)
    {
        if(inventoryUI == null)
        {
            Debug.LogError("Inven Manager : InventoryUI NULL Ref");
            return false;
        }
        bool isItemAdd = inventoryUI.AddItem(newItem);
        if(isItemAdd) return true;
        else
        {
            Debug.LogError("Inven Manager : Item Add Failed");
            return false;
        }
    }
    #endregion
}

