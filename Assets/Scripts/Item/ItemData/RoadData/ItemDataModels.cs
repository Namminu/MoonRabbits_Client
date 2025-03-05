using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemJsonData
{
	public int item_id;
	public string item_name;
	public int item_type;
	public string item_description;
}

[Serializable]
public class MaterialItemJsonData : ItemJsonData
{
	public int item_max_stack;
}

[Serializable]
public class HousingItemJsonData : ItemJsonData
{
	public string item_prefab;
	public int[] item_gridSize;
}

[Serializable]
public class MaterialItemJsonWrapper
{
	public string name;
	public string version;
	public List<MaterialItemJsonData> data;
}

[Serializable]
public class HousingItemJsonWrapper
{
	public string name;
	public string version;
	public List<HousingItemJsonData> data;
}