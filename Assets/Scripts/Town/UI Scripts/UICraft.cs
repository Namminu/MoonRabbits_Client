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

    public GameObject detailFrame;
    public Button btnFold;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI materialText;
    public Button btnCraft;
    private int craftCount = 1;
    public TextMeshProUGUI craftCountText;
    public Button btnIncrease;
    public Button btnDecrease;
    public TextMeshProUGUI alarmText;
    private Recipe selectedRecipe;
    private Dictionary<int, int> itemSlotById = new Dictionary<int, int>();
    private Dictionary<int, int> itemStackById = new Dictionary<int, int>();
    private bool canCraft;
    public Button btnAddWood;
    public Button btnClose;
    public GameObject uiCraftPanel;
    public GameObject progressFrame;
    public TextMeshProUGUI progressTitle;
    public Slider progressBar;
    public TextMeshProUGUI successText;
    public Button confirmButton;
    public GameObject disableMask;

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
        btnCraft.onClick.AddListener(OnCraftBtnClick);
        btnAddWood.onClick.AddListener(OnAddWoodClick);
        btnClose.onClick.AddListener(OnCloseBtnClick);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    public void InitUiCraft()
    {
        ClearUiCraft();
        InitDetailRecipe();
        if (GameManager.Instance.recipeContainer.data == null)
        {
            Debug.Log("레시피 로드 안 됨");
            return;
        };

        // 스크롤 공간의 높이 설정
        float totalHeight = 65 * GameManager.Instance.recipeContainer.data.Count + 100;
        scrollViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollViewContent.GetComponent<RectTransform>().sizeDelta.x, totalHeight);
        
        foreach (Recipe recipe in GameManager.Instance.recipeContainer.data)
        {
            GameObject newRecipeBtn = Instantiate(recipeBtnPrefab, scrollViewContent.transform);
            TextMeshProUGUI recipeBtnText = newRecipeBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (newRecipeBtn == null || recipeBtnText == null)
            {
                Debug.LogError("레시피 버튼 생성 및 제목 할당 오류");
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
        }
    }

    private void ClearUiCraft()
    {
        for(int i = scrollViewContent.transform.childCount -1; i >= 0; i--)
        {
            Transform child = scrollViewContent.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void OnRecipeBtnClick(Recipe recipe)
    {
        selectedRecipe = recipe;

        int recipeId = recipe.recipe_id;
        Debug.Log($"클릭된 레시피ID:{recipeId}");
        InitDetailRecipe();
        GetInventorySlotByItemId();
    }

    private void InitDetailRecipe()
    {
        titleText.text = "";
        materialText.text = "";
        craftCount = 1;
        craftCountText.text = "1개";
    }

    private void ShowDetailRecipe(Recipe recipe)
    {
        if (progressFrame.activeSelf) progressFrame.SetActive(false);
        if (!detailFrame.activeSelf) detailFrame.SetActive(true);
        
        // 제목
        string title = recipe.craft_item_name;   //item.json에서 아이템 이름 읽어야함
        titleText.text = title;

        // 재료 목록
        StringBuilder materialSb = new StringBuilder();
        canCraft = true;
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

            if(!itemStackById.TryGetValue(materialItemId, out int stack))
            {
                stack = 0;
            }
            materialSb.Append($"{name} : {stack}/{craftCount * count} 개\n");
            
            // 재료 부족
            if (craftCount * count > stack) canCraft = false;
        }
        alarmText.text = canCraft ? "" : "재료가 부족합니다";
        materialText.text = materialSb.ToString();

        // 제작 개수
        craftCountText.text = $"{craftCount}개";
    }

    public void OnFoldBtnClick()
    {
        //InitDetailRecipe();
        detailFrame.SetActive(false);
    }

    public void OnCraftBtnClick()
    {
        if(true) {
            Debug.LogWarning("제작 버튼!");
            var pkt = new C2SCraft{
                RecipeId = selectedRecipe.recipe_id,
                Count = craftCount
            };
            GameManager.Network.Send(pkt);
        }else {
            Debug.LogWarning("제작 불가");
        }
    }

    public void OnDecreaseBtnClick()
    {
        if (craftCount <= 1) return; 
        craftCount--;
        GetInventorySlotByItemId();
    }

    public void OnIncreaseBtnClick()
    {
        craftCount++;
        GetInventorySlotByItemId();
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
            itemStackById[slot.ItemId] = slot.Stack;
        }
        ShowDetailRecipe(selectedRecipe);
    }

    public void OnAddWoodClick()
    {
        Debug.LogWarning("아이템 획득 간다");
        var pkt = new C2SItemObtained{
            ItemId = 20001,
            SlotIdx = 1
        };
        GameManager.Network.Send(pkt);
    }

    private void OnCloseBtnClick()
    {
        //InitDetailRecipe();
        CanvasManager.Instance.uiCraft.gameObject.SetActive(false);
    }

    private void OnConfirmButtonClick()
    {
        progressFrame.SetActive(false);
    }

    public void CraftResult(S2CCraft pkt)
    {
        List<ItemJson> data = GameManager.Instance.materialItemContainer.data;
        ItemJson resultItem = data.Find(d => d.item_id == pkt.CraftedItemId);

        int craftedCount = 0;   // 현재 제작 개수를 저장할 변수

        detailFrame.SetActive(false); // 제작설명서 끄기
        confirmButton.gameObject.SetActive(false);
        progressFrame.SetActive(true); // 제작진행창 띄우기
        progressTitle.text = $"{resultItem.item_name}";
        successText.text = $"{craftedCount}/{pkt.Count}개 제작 완료"; // 하나 제작 완료될때마다 갱신
        disableMask.SetActive(true); // 제작하는동안 제작종류선택UI 클릭못하게 막는용도
        
        // progressBar 코루틴 실행
        StartCoroutine(DoCraft(pkt.Count, () => {
            // 모두 완료시 확인버튼 띄우기, 마스크 제거하기
            confirmButton.gameObject.SetActive(true);
            disableMask.SetActive(false);
            
            GameManager.Instance.SManager.UiChat.PushMessage(
                    "System",
                    $"{resultItem.item_name} {pkt.Count}개 제작에 성공하였습니다.",
                    "System",
                    true
                );
        }));
    }

    private IEnumerator DoCraft(int count, System.Action onComplete)
    {
        int craftedCount = 0;

        for(int i=0; i<count; i++){
            yield return StartCoroutine(UpdateProgressBar(3f, ()=>
            {
                craftedCount++;
                successText.text = $"{craftedCount}/{count}개 제작 완료";
            }));

            if(i < count -1)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        onComplete?.Invoke();
    }

    private IEnumerator UpdateProgressBar(float duration, System.Action onProgressComplete)
    {
        float elapsed = 0f;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressBar.maxValue = duration;
            progressBar.value = elapsed;
            yield return null;
        }

        onProgressComplete?.Invoke();
    }
}
