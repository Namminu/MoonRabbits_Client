using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class RecipeMaterialItem
{
    public int item_id;
    public int count;
}

[System.Serializable]
public class FurnitureRecipe
{
    public int recipe_id;
    public List<RecipeMaterialItem> material_items;
    public int craft_item_id;
}

[System.Serializable]
public class FurnitureRecipeList
{
    public string name;
    public string version;
    public List<FurnitureRecipe> data;
}

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance { get; private set; }
    public List<FurnitureRecipe> furnitureRecipes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadFurnitureRecipes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadFurnitureRecipes()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "furniture_recipe.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            FurnitureRecipeList recipeList = JsonUtility.FromJson<FurnitureRecipeList>(jsonData);
            if (recipeList != null && recipeList.data != null && recipeList.data.Count > 0)
            {
                furnitureRecipes = recipeList.data;
                Debug.Log("Furniture recipes loaded successfully.");
            }
            else
            {
                Debug.LogError("furniture_recipe.json 파싱 실패: 데이터가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("furniture_recipe.json 파일을 찾을 수 없습니다: " + filePath);
        }
    }

    public FurnitureRecipe GetRecipeByCraftItemId(int craftItemId)
    {
        return furnitureRecipes.Find(recipe => recipe.craft_item_id == craftItemId);
    }
}
