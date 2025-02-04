using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomParts
{
    Top,
    Bottom,
    Shoes,
    AccFace,
    AccBack
}

[ CreateAssetMenu( fileName = "CustomizeItemList", menuName = "Scriptable Object/CustomizeItemList" )]
public class CustomizeItemList : ScriptableObject
{
    [SerializeField] private List<CustomizeItem> faceList;
    [SerializeField] private List<CustomizeItem> hatList;
    [SerializeField] private List<CustomizeItem> shoesList;
    [SerializeField] private List<CustomizeItem> colorList;
    [SerializeField] private List<CustomizeItem> vehicleList;
    
    private List<List<CustomizeItem>> _itemList = null;

    public List<List<CustomizeItem>> ItemList
    {
        get
        {
            if (_itemList == null)
            {
                _itemList = new List<List<CustomizeItem>>();
                SetItemPartsAndRegisterToList(faceList, CustomParts.Top);
                SetItemPartsAndRegisterToList(hatList, CustomParts.Bottom);
                SetItemPartsAndRegisterToList(shoesList, CustomParts.Shoes);
                SetItemPartsAndRegisterToList(colorList, CustomParts.AccFace);
                SetItemPartsAndRegisterToList(vehicleList, CustomParts.AccBack);
            }

            return _itemList;
        }
    }

    private void SetItemPartsAndRegisterToList(List<CustomizeItem> items, CustomParts parts)
    {
        SetItemParts(items, parts);
        _itemList.Add(items);
    }
    
    private void SetItemParts(List<CustomizeItem> items, CustomParts parts)
    {
        for (var i = 0; i < items.Count; i++)
        {
            items[i].DataIdx = i;
            items[i].Parts = parts;
        }
    }
}

[Serializable]
public class CustomizeItem
{
    [SerializeField] private int idx;
    [SerializeField] private string name;
    [SerializeField] private float angle;
    [SerializeField] private Sprite sprite;
    
    [SerializeField] private int price;
    
    [SerializeField] private Material mat;

    private int _dataIdx;
    private CustomParts _parts;
    
    public int Idx => idx;
    public string Name => name;
    public float Angle => angle;
    public Sprite Sprite => sprite;
    
    public int Price => price;
    
    public Material Mat => mat;

    public int DataIdx { 
        get => _dataIdx;
        set => _dataIdx = value; 
    }
    
    public CustomParts Parts
    {
        get => _parts;
        set => _parts = value;
    }
}


