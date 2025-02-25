using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        string filePath = Path.Combine(Application.streamingAssetsPath, "Quest.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        var questContainer = loader.ReadJsonFile<JsonContainer<Quest>>(filePath);
        if (questContainer == null || questContainer.data == null || questContainer.data.Count == 0)
        {
            Debug.LogError("JSON 파싱 실패: 데이터가 없습니다.");
            return;
        }
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[0].quest_id}");
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[1].quest_id}");
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[2].quest_id}");
    }

    private void Update()
    {
        if (network != null)
            network.Update();
    }

    public void WaitForSceneAwake(string sceneName, PlayerInfo playerInfo)
    {
        StartCoroutine(EnterScene(sceneName, playerInfo));
    }

    IEnumerator EnterScene(string sceneName, PlayerInfo playerInfo)
    {
        switch (sceneName)
        {
            case "Town":
                yield return new WaitUntil(() => TownManager.Instance != null);
                TownManager.Instance.Spawn(playerInfo);
                break;
            case "Sector1":
                yield return new WaitUntil(() => S1Manager.Instance != null);
                S1Manager.Instance.Enter(playerInfo);
                break;
            case "Sector2":
                yield return new WaitUntil(() => S2Manager.Instance != null);
                S2Manager.Instance.Enter(playerInfo);
                break;
        }
    }
}
