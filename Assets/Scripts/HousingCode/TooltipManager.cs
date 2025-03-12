using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [SerializeField] private GameObject tooltipPanel; // 캔버스에 배치한 툴팁 패널
    [SerializeField] private TMP_Text tooltipText;          // 패널 내 텍스트 컴포넌트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬 이동 시에도 유지하려면 DontDestroyOnLoad(gameObject); 사용 가능
        }
        else
        {
            Destroy(gameObject);
        }
        tooltipPanel.SetActive(false);
    }

    // myItemId에 해당하는 데이터를 조회하여 툴팁 텍스트를 구성하는 로직을 작성합니다.
    public void ShowTooltip(int itemId, Vector2 pointerPosition)
    {
        // 예시: itemId로 아이템 정보를 조회하는 로직. 실제로는 데이터베이스나 ScriptableObject에서 가져올 수 있습니다.
        string itemInfo = GetItemInfo(itemId);
        tooltipText.text = itemInfo;
        tooltipPanel.SetActive(true);

        // 포인터 위치에 툴팁을 표시(캔버스의 좌표계에 맞게 조정 필요)
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        // 포인터 위치의 오른쪽 위(예: x 10, y 10 픽셀 오프셋)를 적용합니다.
        Vector2 offset = new Vector2(50f, 150f);
        tooltipRect.position = pointerPosition + offset;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    // 아이템 정보 생성 및 표시
    // 인벤토리 내에서 특정 itemId를 가진 아이템의 총 보유 개수를 계산합니다.
    private int GetOwnedItemCount(int itemId)
    {
        int totalCount = 0;
        // InventoryManager의 현재 인벤토리 Dictionary 사용 (key: 슬롯 인덱스, value: MaterialItem)
        Dictionary<int, MaterialItem> inv = InventoryManager.instance.GetCurrentInventoryDictionary();
        foreach (var kvp in inv)
        {
            MaterialItem matItem = kvp.Value;
            if (matItem != null && matItem.ItemData.ItemId == itemId)
            {
                totalCount += matItem.CurItemStack;
            }
        }
        return totalCount;
    }

    // itemId(예를 들어, 가구의 craftItemId)를 받아 해당 아이템 제작에 필요한 재료와 인벤토리 상태를 문자열로 구성
    public string GetItemInfo(int itemId)
    {
        // RecipeManager를 통해 itemId에 해당하는 제작 레시피 정보를 조회합니다.
        var recipe = RecipeManager.instance.GetRecipeByCraftItemId(itemId);
        if (recipe == null)
        {
            return "<color=red>레시피 정보를 찾을 수 없습니다.</color>";
        }

        StringBuilder infoBuilder = new StringBuilder();

        // 가구 이름을 가져오기 위해 ItemDataLoader의 HousingItemsList를 사용
        string furnitureName = "Unknown Furniture";
        var furnitureData = ItemDataLoader.HousingItemsList.Find(x => x.ItemId == itemId);
        if (furnitureData != null)
        {
            furnitureName = furnitureData.ItemName;
        }
        // 가구 이름을 굵게 표시
        infoBuilder.AppendLine($"<b>{furnitureName}</b>\n");
        infoBuilder.AppendLine("필요한 재료:");

        // 레시피에 포함된 각 재료(material)가 요구하는 개수와 인벤토리 보유량을 비교
        foreach (var material in recipe.material_items)
        {
            int ownedCount = GetOwnedItemCount(material.item_id);
            string itemName = "Unknown";
            var matData = ItemDataLoader.MaterialItemsList.Find(x => x.ItemId == material.item_id);
            if (matData != null)
            {
                itemName = matData.ItemName;
            }

            if (ownedCount < material.count)
            {
                infoBuilder.AppendLine($"<color=red>{itemName} : {ownedCount} / {material.count}</color>");
            }
            else
            {
                infoBuilder.AppendLine($"<color=green>{itemName} : {ownedCount} / {material.count}</color>");
            }
        }

        return infoBuilder.ToString();
    }
}
