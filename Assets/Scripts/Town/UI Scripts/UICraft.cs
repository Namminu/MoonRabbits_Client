using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICraft : MonoBehaviour
{
    public static UICraft instance { get; private set; }

    public GameObject scrollViewContent;
    public GameObject recipeBtnPrefab;
    private Dictionary<int, GameObject> recipeBtns = new Dictionary<int, GameObject>();

    public GameObject uiDetail;
    public Button btnFold;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI materialText;
    public Button btnCraft;
    private int craftCount = 0;
    public TextMeshProUGUI craftCountText;
    public Button btnIncrease;
    public Button btnDecrease;
    public TextMeshProUGUI alarmText;
    private Recipe selectedRecipe;
    private Dictionary<int, int> itemSlotById = new Dictionary<int, int>();
    private Dictionary<int, int> itemStackById = new Dictionary<int, int>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        btnFold.onClick.AddListener(OnFoldBtnClick);
        btnIncrease.onClick.AddListener(OnIncreaseBtnClick);
        btnDecrease.onClick.AddListener(OnDecreaseBtnClick);
        if (GameManager.Instance.recipeContainer == null)
        {
            Debug.Log("레시피 로드 안 됨");
            return;
        }
        ;

        foreach (Recipe recipe in GameManager.Instance.recipeContainer.data)
        {
            GameObject newRecipeBtn = Instantiate(recipeBtnPrefab, scrollViewContent.transform);
            if (newRecipeBtn == null)
            {
                Debug.LogError("레시피 버튼 인스턴스화 실패");
                continue;
            }
            TextMeshProUGUI recipeBtnText = newRecipeBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (recipeBtnText == null)
            {
                Debug.LogError("레시피 버튼 프리팹에 TextMeshProUGUI 컴퓨넌트 없음");
                continue;
            }

            List<ItemJson> itemData = GameManager.Instance.materialItemContainer.data;
            ItemJson item = itemData.FirstOrDefault(d => d.item_id == recipe.craft_item_id);
            string itemName = item?.item_name;
            if (itemData == null || item == null || itemName == null)
            {
                Debug.LogError("UICraft 초기화중 아이템 못 찾음");
                return;
            }
            recipe.craft_item_name = itemName;
            recipeBtnText.text = itemName;

            Recipe r = recipe;
            newRecipeBtn.GetComponent<Button>().onClick.AddListener(() => OnRecipeBtnClick(r));
            recipeBtns.Add(recipe.recipe_id, newRecipeBtn);
        }
    }

    public void OnRecipeBtnClick(Recipe recipe)
    {
        selectedRecipe = recipe;

        int recipeId = recipe.recipe_id;
        Debug.Log($"클릭된 레시피ID:{recipeId}");

        initDetailRecipe();
        GetInventorySlotByItemId();
        //ShowDetailRecipe(recipe);
    }

    private void initDetailRecipe()
    {
        titleText.text = "";
        materialText.text = "";
        craftCount = 0;
        craftCountText.text = "0개";
    }

    private void ShowDetailRecipe(Recipe recipe)
    {
        if (!uiDetail.activeSelf) uiDetail.SetActive(true);

        // 제목
        string title = recipe.craft_item_name;   //item.json에서 아이템 이름 읽어야함
        titleText.text = title;

        // 재료 목록
        StringBuilder materialSb = new StringBuilder();
        bool canCraft = true;
        foreach (var material in recipe.material_items)
        {
            int materialItemId = material.item_id;
            int count = material.count;
            if (material.name == null)
            {
                List<ItemJson> itemData = GameManager.Instance.materialItemContainer.data;
                ItemJson item = itemData.FirstOrDefault(d => d.item_id == material.item_id);
                string itemName = item?.item_name;
                if (itemData == null || item == null || itemName == null)
                {
                    Debug.LogError("UICraftDetail 초기화중 재료아이템 못 찾음");
                    return;
                }
                material.name = itemName;
            }
            string name = material.name;
            int inventoryStack = itemStackById[materialItemId];
            // 인벤토리에서 itemId가 materialItemId인 인벤토리의 stack
            materialSb.Append($"{name} : {craftCount * count}/{inventoryStack} 개\n");

            // 재료 부족
            if (craftCount * count > 0) canCraft = false;
        }
        alarmText.text = canCraft ? "" : "재료가 부족합니다";
        materialText.text = materialSb.ToString();

        // 제작 개수
        craftCountText.text = $"{craftCount}개";
    }

    public void OnFoldBtnClick()
    {
        initDetailRecipe();
        uiDetail.SetActive(false);
    }

    public void OnCraftBtnClick()
    {
        Debug.Log("제작 구현중");
    }

    public void OnDecreaseBtnClick()
    {

        // #FIX 아래는 서버에서 응답이 오면 실행하자
        if (craftCount > 0) craftCount--;
        GetInventorySlotByItemId();
        //ShowDetailRecipe(selectedRecipe);
    }

    public void OnIncreaseBtnClick()
    {
        craftCount++;
        GetInventorySlotByItemId();
        // 패킷 갔다오면 인벤토리에 재료아이템을 가지고 있는지, 몇 개인지 알려줌

        // #FIX 아래는 서버에서 응답이 오면 실행하자
    }

    private void GetInventorySlotByItemId()
    {
        var pkt = new C2SGetInventorySlotByItemId();
        foreach(var material in selectedRecipe.material_items){
            pkt.ItemIds.Add(material.item_id);
        }
        GameManager.Network.Send(pkt);
    }

    // 서버로부터 원하는 아이템의 인벤토리 슬롯, 수량 응답
    public void GetInventorySlotByItemId(S2CGetInventorySlotByItemId pkt)
    {
        itemSlotById.Clear();
        itemStackById.Clear();
        foreach(var slot in pkt.Slots)
        {
            itemSlotById[slot.ItemId] = slot.SlotIdx;
            itemStackById[slot.ItemId] = slot.ItemId;
        }
        ShowDetailRecipe(selectedRecipe);
    }
}
