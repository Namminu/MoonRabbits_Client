using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;

[Serializable]
public class Recipe
{
  public int recipe_id;
  public List<MaterialItem> material_items;
  public int craft_item_id;
  public string craft_item_name = null;

  public class MaterialItem
  {
    public int item_id;
    public int count;
    public string name = null;
  }
}

