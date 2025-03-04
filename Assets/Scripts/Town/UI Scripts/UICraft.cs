using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        if(GameManager.Instance.recipeContainer==null)
        {
            Debug.Log("레시피 로드 안 됨");
            return;
        };

        foreach(Recipe recipe in GameManager.Instance.recipeContainer.data) {
            GameObject newRecipeBtn = Instantiate(recipeBtnPrefab, scrollViewContent.transform);
            if(newRecipeBtn == null)
            {
                Debug.LogError("레시피 버튼 인스턴스화 실패");
                continue;
            }
            TextMeshProUGUI recipeBtnText = newRecipeBtn.GetComponentInChildren<TextMeshProUGUI>();
            if(recipeBtnText == null)
            {
                Debug.LogError("레시피 버튼 프리팹에 TextMeshProUGUI 컴퓨넌트 없음");
                continue;
            }

            recipeBtnText.text = $"아이템{recipe.craft_item_id}";   //item.json에서 아이템 이름 읽어야함

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
        ShowDetailRecipe(recipe);
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
        if(!uiDetail.activeSelf) uiDetail.SetActive(true);

        // 제목
        string title = $"아이템{recipe.craft_item_id}";   //item.json에서 아이템 이름 읽어야함
        titleText.text = title;

        // 재료 목록
        StringBuilder materialSb = new StringBuilder();
        bool canCraft = true;
        foreach(var material in recipe.material_items) {
            int materialItemId = material.item_id;
            int count = material.count;
            string name = $"아이템{materialItemId}";
            int inventoryStack = 0;
            // 인벤토리에서 itemId가 materialItemId인 인벤토리의 stack
            materialSb.Append($"{name} : {craftCount*count}/{inventoryStack} 개\n");

            // 재료 부족
            if(craftCount*count > 0) canCraft = false;
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
        if(craftCount>0) craftCount--;
        ShowDetailRecipe(selectedRecipe);
    }
    
    public void OnIncreaseBtnClick()
    {
        craftCount++;
        ShowDetailRecipe(selectedRecipe);
    }

    public void OnNaverLoginBtnClick()
    {
        
    }
}
