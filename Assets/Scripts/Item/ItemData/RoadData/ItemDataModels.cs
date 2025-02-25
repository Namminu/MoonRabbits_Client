using JetBrains.Annotations;
using System;
using System.Collections.Generic;

[Serializable]
public class ItemJsonData
{
	public int item_id;
	public string item_name;
	public string item_type;
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
	public int item_max_stack;
}

[Serializable]
public class ItemJsonWrapper
{
	public string name;
	public string version;
	public List<ItemJsonData> data;
}