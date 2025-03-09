using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class ItemDataLoader
{
    /* Sprite : ID 매칭 클래스 */
    private static SpriteDataBase spriteDataBase;

    /* 데이터 저장 리스트 */
    public static List<MaterialItemData> MaterialItemsList { get; private set; } =
        new List<MaterialItemData>();
    public static List<HousingItemData> HousingItemsList { get; private set; } =
        new List<HousingItemData>();

    /* Json 파일 읽기 경로 */
    private static readonly string MTItemjsonFilePath = "material_item_data.json";
    private static readonly string HSItemjsonFilePath = "housing_item_data.json";

    public static async Task GenerateAllItems()
    {
        string mtJsonText = await LoadJsonFromStreamingAssets(MTItemjsonFilePath);
        string hsJsonText = await LoadJsonFromStreamingAssets(HSItemjsonFilePath);

        if (string.IsNullOrEmpty(mtJsonText) || string.IsNullOrEmpty(hsJsonText))
        {
            Debug.LogError("JSON 파일을 불러오는 데 실패했습니다.");
            return;
        }

        MaterialItemJsonWrapper mtItemWrapper = JsonUtility.FromJson<MaterialItemJsonWrapper>(mtJsonText);
        HousingItemJsonWrapper hsItemWrapper = JsonUtility.FromJson<HousingItemJsonWrapper>(hsJsonText);

        if (
            mtItemWrapper == null
            || hsItemWrapper == null
            || mtItemWrapper.data == null
            || hsItemWrapper.data == null
        )
        {
            Debug.LogError("JSON 데이터를 읽는 데 실패했습니다.");
            return;
        }

        LoadSpriteDataBase();
        GenerateMaterialItems(mtItemWrapper.data);
        GenerateHousingItems(hsItemWrapper.data);
    }

    private static void GenerateMaterialItems(List<MaterialItemJsonData> items)
    {
        foreach (var item in items)
        {
            if (item.item_type != 1)
                continue;

            MaterialItemData newItem = ScriptableObject.CreateInstance<MaterialItemData>();

            newItem.ItemId = item.item_id;
            newItem.ItemName = item.item_name;
            newItem.ItemDescription = item.item_description;
            newItem.ItemType = (ItemTypes)item.item_type;
            newItem.ItemIcon = GetSpriteByItemId(item.item_id);
            newItem.ItemMaxStack = 9999;

            MaterialItemsList.Add(newItem);
        }
        Debug.Log("재료 아이템 생성 갯수 : " + MaterialItemsList.Count);
    }

    private static void GenerateHousingItems(List<HousingItemJsonData> items)
    {
        foreach (var item in items)
        {
            if (item.item_type != 0)
                continue;

            HousingItemData newItem = ScriptableObject.CreateInstance<HousingItemData>();

            newItem.ItemId = item.item_id;
            newItem.ItemName = item.item_name;
            newItem.ItemDescription = item.item_description;
            newItem.ItemType = (ItemTypes)item.item_type;
            newItem.ItemIcon = GetSpriteByItemId(item.item_id);
            newItem.ItemPrefab = GetPrefabByName(item.item_prefab);
            newItem.ItemDataType = (ItemDataTypes)item.item_dataType;
			if (item.item_gridSize != null && item.item_gridSize.Length == 2)
			{
				newItem.ItemGridSize = new Vector2Int(item.item_gridSize[0], item.item_gridSize[1]);
			}
			else
			{
				newItem.ItemGridSize = Vector2Int.zero; // 기본값 처리
				Debug.LogError($"{newItem.ItemName} : Wrong item_gridSize Value: {item.item_gridSize}");
			}

			HousingItemsList.Add(newItem);
        }
        Debug.Log("하우징 아이템 생성 갯수 : " + HousingItemsList.Count);
    }

    public static Sprite GetSpriteByItemId(int itemId)
    {
        LoadSpriteDataBase();
        return spriteDataBase?.GetSprite(itemId);
    }

    private static GameObject GetPrefabByName(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
            return null;
        return Resources.Load<GameObject>($"Prefabs/3DObjects/{prefabName}");
    }

    private static void LoadSpriteDataBase()
    {
        if (spriteDataBase == null)
        {
            spriteDataBase = Resources.Load<SpriteDataBase>("DataBase/SpriteDataBase");
        }
    }

    private static async Task<string> LoadJsonFromStreamingAssets(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (Application.platform == RuntimePlatform.Android)
        {
            path = "file://" + path;
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield(); // 비동기적으로 대기
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"JSON 로드 실패: {request.error}");
                    return null;
                }
            }
        }
        else
        {
            return File.ReadAllText(path);
        }
    }
}
