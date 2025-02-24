using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf.Protocol;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    private NetworkManager network;
    public static NetworkManager Network => _instance.network;

    public const string BattleScene = "Battle";
    public const string TownScene = "Town";

    public S2CSectorEnter Pkt;

    public string UserName;
    public int ClassCode;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            network = new NetworkManager();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SoundManager.Instance.Play(19, Define.Sound.Bgm);

    }

    void Start()
    {
        JsonFileLoader loader = new JsonFileLoader();

        // 단일 JSON 파일 로드
        string filePath = Path.Combine(Application.streamingAssetsPath, "consumable_item.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        var itemContainer = loader.ReadJsonFile<JsonContainer<ConsumableItem>>(filePath);
        if (itemContainer == null || itemContainer.data == null || itemContainer.data.Count == 0)
        {
            Debug.LogError("JSON 파싱 실패: 데이터가 없습니다.");
            return;
        }
        Debug.Log($"소모품 아이템 로드 완료: {itemContainer.data[0].item_name}");
        Debug.Log($"소모품 아이템 로드 완료: {itemContainer.data[1].item_name}");
        Debug.Log($"소모품 아이템 로드 완료: {itemContainer.data[2].item_name}");

        // 디렉토리 내 모든 JSON 파일 로드
        string dirPath = Application.streamingAssetsPath;
        var allItems = loader.ReadAllJsonFiles<ConsumableItem>(dirPath);
        Debug.Log($"총 {allItems.Count}개의 JSON 파일을 로드함!");
    }

    private void Update()
    {
        if (network != null)
            network.Update();
    }
}
