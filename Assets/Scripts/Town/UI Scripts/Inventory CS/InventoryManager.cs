using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }  // 싱글톤 인스턴스

    // 인벤토리 슬롯 데이터. key: 슬롯 인덱스, value: InventorySlotData
    private Dictionary<int, MaterialItem> inventoryDictionary  = new Dictionary<int, MaterialItem>();

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
    // public void UpdateInventoryData(S2CInventoryUpdate pkt)
    // {
    //     inventoryDictionary.Clear();

    //     foreach (var slot in pkt.Slots)
    //     {
    //         // slot.ItemId와 slot.Stack이 전달됩니다.
    //         // 여기서는 예시로 ItemDatabase.GetMaterialItemData()를 호출해 MaterialItemData를 조회한다고 가정합니다.

    //         MaterialItemData itemData = ItemDataLoader; // [3][4]

    //         if (itemData != null)
    //         {
    //             // MaterialItem 생성자에서 ItemData와 초기 stack을 할당합니다. [1]
    //             MaterialItem materialItem = new MaterialItem(itemData, slot.Stack);
    //             inventoryDictionary.Add(slot.SlotIdx, materialItem);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("아이템 데이터가 존재하지 않음. ItemId: " + slot.ItemId);
    //         }
    //     }
    //     Debug.Log("S2CInventoryUpdate 패킷 처리 완료: " + pkt);

    //     // 갱신된 인벤토리 데이터를 UI에 반영합니다.
    //     if (inventoryUI != null)
    //     {
    //         inventoryUI.RefreshInventory(inventoryDictionary);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("InventoryUI 참조가 없음");
    //     }
    // }

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
    public void SendInventorySort(List<InventorySlotData> sortedSlots)
    {
        C2SInventorySort packet = new C2SInventorySort();
        foreach (var slotData in sortedSlots)
        {
            InventorySlot newSlot = new InventorySlot
            {
                SlotIdx = slotData.slotIdx,
                ItemId = slotData.itemId,
                Stack = slotData.stack
            };
            packet.Slots.Add(newSlot);
        }
        GameManager.Network.Send(packet);
    }
}

