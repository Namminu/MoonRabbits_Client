using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotData
{
    public int slotIdx;  // 슬롯 인덱스
    public int itemId;   // 아이템 아이디
    public int stack;    // 아이템 개수

    public InventorySlotData(int slotIdx, int itemId, int stack)
    {
        this.slotIdx = slotIdx;
        this.itemId = itemId;
        this.stack = stack;
    }
}
