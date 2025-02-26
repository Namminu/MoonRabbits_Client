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

    public JsonContainer<Resource> resourceContainer;

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
        string questFilePath = Path.Combine(Application.streamingAssetsPath, "Quest.json");

        if (!File.Exists(questFilePath))
        {
            Debug.LogError($"quest JSON 파일을 찾을 수 없습니다: {questFilePath}");
            return;
        }

        var questContainer = loader.ReadJsonFile<JsonContainer<Quest>>(questFilePath);
        if (questContainer == null || questContainer.data == null || questContainer.data.Count == 0)
        {
            Debug.LogError("quest JSON 파싱 실패: 데이터가 없습니다.");
            return;
        }
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[0].quest_name}");
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[1].quest_name}");
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[2].quest_name}");

        // 단일 JSON 파일 로드
        string resourceFilePath = Path.Combine(Application.streamingAssetsPath, "Resource.json");

        if (!File.Exists(resourceFilePath))
        {
            Debug.LogError($"resouce JSON 파일을 찾을 수 없습니다: {resourceFilePath}");
            return;
        }

        resourceContainer = loader.ReadJsonFile<JsonContainer<Resource>>(resourceFilePath);
        if (resourceContainer == null || resourceContainer.data == null || resourceContainer.data.Count == 0)
        {
            Debug.LogError("resouce JSON 파싱 실패: 데이터가 없습니다.");
            return;
        }
    }

    private void Update()
    {
        if (network != null)
            network.Update();
    }
}
