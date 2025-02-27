using System;
using System.Collections.Generic;

[Serializable]
public class Resource
{
    public int resource_id;
    public string resource_name;
    public string resource_type;
    public string resource_description;
    public int resource_difficulty;
    public int resource_durability;
    public int resource_respawn;
    public List<Drop_Item> drop_item;
}

public class Drop_Item
{
    public int chance;
    public int item_id;
}
