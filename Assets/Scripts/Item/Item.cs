using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // 로컬 JSON 파일 경로 예시 (StreamingAssets 폴더)
    private string jsonFilePath => Path.Combine(Application.streamingAssetsPath, "./ItemAssets/itemData.json");
    private int itemCode;
    private Image itemImage;
    private ItemData itemData;


    public Image GetItemImage() { return itemImage; } 

    public ItemData GetItemData() { return itemData; }

    public IEnumerator SendItemData()
    {
        // JSON 파일 읽기
        string jsonData;
        try {
            jsonData = File.ReadAllText(jsonFilePath);
        } catch(Exception e) {
            Debug.LogError("JSON 파일 읽기 에러: " + e.Message);
            yield break;
        }

        // protobuf 메시지 생성
        // ItemPacket itemPacket = new ItemPacket
        // {
        //     JsonData = jsonData
        // };
    }
}
