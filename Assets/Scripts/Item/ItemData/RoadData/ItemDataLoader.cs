using System.IO;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class ItemDataLoader
{
	/* Sprite : ID ��Ī Ŭ���� */
	private static SpriteDataBase spriteDataBase;

	/* Json ���� ��� */
	private static readonly string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "item_data.json");
	private static readonly string materialOutputPath = "Assets/Resources/ItemDataFile/MaterialItemData";
	private static readonly string housingOutputPath = "Assets/Resources/ItemDataFile/HousingItemData";

	[MenuItem("Tools/Generate All Items")]
	public static async void GenerateAllItems()
	{
		string jsonText = await LoadJsonFromStreamingAssets();

		if (string.IsNullOrEmpty(jsonText))
		{
			Debug.LogError("JSON ������ �ҷ����� �� �����߽��ϴ�.");
			return;
		}

		ItemJsonWrapper itemWrapper = JsonUtility.FromJson<ItemJsonWrapper>(jsonText);

		if (itemWrapper == null || itemWrapper.data == null)
		{
			Debug.LogError("JSON �����͸� �д� �� �����߽��ϴ�.");
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
			Debug.Log($"Material ������ ������: {assetPath}");
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
			Debug.Log($"Housing ������ ������: {assetPath}");
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
					await Task.Yield(); // �񵿱������� ���
				}

				if (request.result == UnityWebRequest.Result.Success)
				{
					return request.downloadHandler.text;
				}
				else
				{
					Debug.LogError($"JSON �ε� ����: {request.error}");
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