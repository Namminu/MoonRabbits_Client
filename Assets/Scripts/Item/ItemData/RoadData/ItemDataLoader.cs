using System.IO;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class ItemDataLoader
{
	/* Sprite : ID 매칭 클래스 */
	private static SpriteDataBase spriteDataBase;

	/* Json 파일 경로 */
	private static readonly string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "item_data.json");
	private static readonly string materialOutputPath = "Assets/Resources/ItemDataFile/MaterialItemData";
	private static readonly string housingOutputPath = "Assets/Resources/ItemDataFile/HousingItemData";

	[MenuItem("Tools/Generate All Items")]
	public static async void GenerateAllItems()
	{
		string jsonText = await LoadJsonFromStreamingAssets();

		if (string.IsNullOrEmpty(jsonText))
		{
			Debug.LogError("JSON 파일을 불러오는 데 실패했습니다.");
			return;
		}

		ItemJsonWrapper itemWrapper = JsonUtility.FromJson<ItemJsonWrapper>(jsonText);

		if (itemWrapper == null || itemWrapper.data == null)
		{
			Debug.LogError("JSON 데이터를 읽는 데 실패했습니다.");
			return;
		}

		LoadSpriteDataBase();
		GenerateMaterialItems(itemWrapper.data);
		//GenerateHousingItems(itemWrapper.data);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}


	private static void GenerateMaterialItems(List<ItemJsonData> items)
	{
		if (!Directory.Exists(materialOutputPath)) Directory.CreateDirectory(materialOutputPath);

		foreach (var item in items)
		{
			if (item.item_type != "Material") continue;

			MaterialItemData newItem = ScriptableObject.CreateInstance<MaterialItemData>();
			SerializedObject serializedObject = new SerializedObject(newItem);

			serializedObject.FindProperty("itemId").intValue = item.item_id;
			serializedObject.FindProperty("itemName").stringValue = item.item_name;
			serializedObject.FindProperty("itemDescription").stringValue = item.item_description;
			serializedObject.FindProperty("itemType").enumValueIndex = (int)ItemTypes.MaterialItem;
			serializedObject.FindProperty("itemIcon").objectReferenceValue = GetSpriteByItemId(item.item_id);
			serializedObject.FindProperty("itemName").intValue = newItem.ItemMaxStack;

			serializedObject.ApplyModifiedProperties();

			string assetPath = materialOutputPath + $"MaterialItem_{item.item_id}.asset";
			AssetDatabase.CreateAsset(newItem, assetPath);
			Debug.Log($"Material 아이템 생성됨: {assetPath}");
		}
	}

	private static void GenerateHousingItems(List<ItemJsonData> items)
	{
		if (!Directory.Exists(housingOutputPath)) Directory.CreateDirectory(housingOutputPath);

		foreach (var item in items)
		{
			if (item.item_type != "Housing") continue;

			HousingItemData newItem = ScriptableObject.CreateInstance<HousingItemData>();
			SerializedObject serializedObject = new SerializedObject(newItem);

			serializedObject.FindProperty("itemId").intValue = item.item_id;
			serializedObject.FindProperty("itemName").stringValue = item.item_name;
			serializedObject.FindProperty("itemDescription").stringValue = item.item_description;
			serializedObject.FindProperty("itemType").enumValueIndex = (int)ItemTypes.HousingItem;
			serializedObject.FindProperty("itemIcon").objectReferenceValue = GetSpriteByItemId(item.item_id);

			serializedObject.ApplyModifiedProperties();

			string assetPath = housingOutputPath + $"HousingItem_{item.item_id}.asset";
			AssetDatabase.CreateAsset(newItem, assetPath);
			Debug.Log($"Housing 아이템 생성됨: {assetPath}");
		}
	}

	private static Sprite GetSpriteByItemId(int itemId)
	{
		LoadSpriteDataBase();
		return spriteDataBase?.GetSprite(itemId);
	}

	private static void LoadSpriteDataBase()
	{
		if (spriteDataBase == null)
		{
			spriteDataBase = Resources.Load<SpriteDataBase>("SpriteDataBase");
		}
	}
	private static async Task<string> LoadJsonFromStreamingAssets()
	{
		string path = jsonFilePath;

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