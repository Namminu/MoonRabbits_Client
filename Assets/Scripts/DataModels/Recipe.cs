using System;
using System.Collections.Generic;

[Serializable]
public class Recipe
{
  public int recipe_id;
  public List<MaterialItem> material_items;
  public int craft_item_id;

  public class MaterialItem
  {
    public int item_id;
    public int count;
  }
}

