using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item
{
    // 로컬 JSON 파일 경로 예시 (StreamingAssets 폴더)
    private string jsonFilePath => Path.Combine(Application.streamingAssetsPath, "./ItemAssets/itemData.json");
    private int itemCode;
    private Image itemImage;
    private ItemData itemData;


    public Image GetItemImage() { return itemImage; } 

    public ItemData GetItemData() { return itemData; }

}
