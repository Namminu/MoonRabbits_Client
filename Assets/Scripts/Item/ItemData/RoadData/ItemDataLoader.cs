using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEditor;

public static class ItemDataLoader
{
	/* Sprite : ID ��Ī Ŭ���� */
	private static SpriteDataBase spriteDataBase;

	/* ������ ���� ����Ʈ */
	public static List<MaterialItemData> MaterialItemsList { get; private set; }  = new List<MaterialItemData>();
	public static List<HousingItemData> HousingItemsList { get; private set; } = new List<HousingItemData>();

	/* Json ���� �б� ��� */
	private static readonly string MTItemjsonFilePath = "material_item_data.json";
	private static readonly string HSItemjsonFilePath = "housing_item_data.json";
	 
	public static async Task GenerateAllItems()
	{
		string mtJsonText = await LoadJsonFromStreamingAssets(MTItemjsonFilePath);
		string hsJsonText = await LoadJsonFromStreamingAssets(HSItemjsonFilePath);

		if (string.IsNullOrEmpty(mtJsonText)|| string.IsNullOrEmpty(hsJsonText))
		{
			Debug.LogError("JSON ������ �ҷ����� �� �����߽��ϴ�.");
			return;
		}

		MaterialItemJsonWrapper mtItemWrapper = JsonUtility.FromJson<MaterialItemJsonWrapper>(mtJsonText);
		HousingItemJsonWrapper hsItemWrapper = JsonUtility.FromJson<HousingItemJsonWrapper>(hsJsonText);

		if (mtItemWrapper == null || hsItemWrapper == null || mtItemWrapper.data == null || hsItemWrapper.data == null)
		{
			Debug.LogError("JSON �����͸� �д� �� �����߽��ϴ�.");
			return;
		}

		LoadSpriteDataBase();
		GenerateMaterialItems(mtItemWrapper.data);
		GenerateHousingItems(hsItemWrapper.data);
	}

	private static void GenerateMaterialItems(List<MaterialItemJsonData> items)
	{
		foreach(var item in items)
		{
			if (item.item_type != 1) continue;

			MaterialItemData newItem = ScriptableObject.CreateInstance<MaterialItemData>();

			newItem.ItemId = item.item_id;
			newItem.ItemName = item.item_name;
			newItem.ItemDescription = item.item_description;
			newItem.ItemType = (ItemTypes)item.item_type;
			newItem.ItemIcon = GetSpriteByItemId(item.item_id);
			newItem.ItemMaxStack = 99;

			MaterialItemsList.Add(newItem);
		}
		Debug.Log("��� ������ ���� ���� : " + MaterialItemsList.Count);
	}

	private static void GenerateHousingItems(List<HousingItemJsonData> items)
	{
		foreach (var item in items)
		{
			if (item.item_type != 0) continue;

			HousingItemData newItem = ScriptableObject.CreateInstance<HousingItemData>();

			newItem.ItemId = item.item_id;
			newItem.ItemName = item.item_name;
			newItem.ItemDescription = item.item_description;
			newItem.ItemType = (ItemTypes)item.item_type;
			newItem.ItemIcon = GetSpriteByItemId(item.item_id);
			newItem.ItemPrefab = GetPrefabByName(item.item_prefab);
			newItem.ItemGridSize = item.item_gridsize;

			HousingItemsList.Add(newItem);
		}
		Debug.Log("�Ͽ�¡ ������ ���� ���� : " + HousingItemsList.Count);
	}

	private static Sprite GetSpriteByItemId(int itemId)
	{
		LoadSpriteDataBase();
		return spriteDataBase?.GetSprite(itemId);
	}

	private static GameObject GetPrefabByName(string prefabName)
	{
		if (string.IsNullOrEmpty(prefabName)) return null;
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