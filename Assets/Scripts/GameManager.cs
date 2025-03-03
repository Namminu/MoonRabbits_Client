using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    private NetworkManager network;
    public static NetworkManager Network => _instance.network;

    private SManagerBase sManager;
    public SManagerBase SManager => sManager;

    [Header("Me")]
    public string UserName;
    public Player MPlayer;
    public int ClassCode;
    public int CurrentSector;

    [Header("Players")]
    private Dictionary<int, Dictionary<int, Player>> playerList = new();
    public Dictionary<int, Dictionary<int, Player>> PlayerList => playerList;
    
    private Dictionary<int, Player> townPlayers = new();
    private Dictionary<int, Player> s1Players = new();
    private Dictionary<int, Player> s2Players = new();
    private Dictionary<int, Player> s3Players = new();
    private Dictionary<int, Player> s4Players = new();

    [Header("Utils")]
    public JsonContainer<Resource> resourceContainer;
    private readonly Dictionary<int, string> sceneName = new()
    {
        { 100, "Town" },
        { 101, "Sector1" },
        { 102, "Sector2" },
        { 103, "Sector3" },
        { 104, "Sector4" },
    };
    public Dictionary<int, string> SceneName => sceneName;

    private async void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            network = new NetworkManager();

            SoundManager.Instance.Play(4, Define.Sound.Bgm);

            SceneManagerEx.SetTransition();

            SetPlayerList();

            CurrentSector = 100;

            DontDestroyOnLoad(gameObject);

            EffectManager.Instance.CreatePersistentEffect("Confetti", new Vector3(-3, 14, 134), Quaternion.identity);

        }
        else
        {
            Destroy(gameObject);
        }

        //SoundManager.Instance.Play(19, Define.Sound.Bgm);
        await ItemDataLoader.GenerateAllItems();
    }

    void Start()
    {
        sManager = TownManager.Instance;
        LoadJson();
    }

    private void Update()
    {
        if (network != null)
            network.Update();
    }

    private void SetPlayerList()
    {
        playerList.Add(100, townPlayers);
        playerList.Add(101, s1Players);
        playerList.Add(102, s2Players);
        playerList.Add(103, s3Players);
        playerList.Add(104, s4Players);
    }

    IEnumerator SetSManager(int sectorCode)
    {
        if (sectorCode == CurrentSector)
            yield break;

        switch (sectorCode)
        {
            case 100:
                yield return new WaitUntil(() => TownManager.Instance != null);
                sManager = TownManager.Instance;
                break;
            case 101:
                yield return new WaitUntil(() => S1Manager.Instance != null);
                sManager = S1Manager.Instance;
                break;
            case 102:
                yield return new WaitUntil(() => S2Manager.Instance != null);
                sManager = S2Manager.Instance;
                break;
            case 103:
                yield return new WaitUntil(() => S3Manager.Instance != null);
                sManager = S3Manager.Instance;
                break;
            case 104:
                // yield return new WaitUntil(() => S4Manager.Instance != null);
                // sManager = S4Manager.Instance;
                break;
            default:
                Debug.Log($"유효하지 않은 섹터 코드입니다 : {sectorCode}");
                break;
        }
    }

    public void EnterAfterSceneAwake(int targetSector, PlayerInfo playerInfo)
    {
        // SceneManagerEx.SetScene(sceneName[playerInfo.CurrentSector]);
        StartCoroutine(EnterSector(targetSector, playerInfo));
    }

    public void SpawnAfterSceneAwake(S2CSpawn pkt)
    {
        StartCoroutine(SpawnOthers(pkt));
    }

    private void OnApplicationQuit()
    {
        network.Discconect();
        Debug.Log("애플리케이션이 종료됩니다.");
        // 애플리케이션 종료 시 처리할 작업을 여기에 추가하세요
    }

    public Player GetPlayer(int playerId)
    {
        foreach (Dictionary<int, Player> sector in playerList.Values)
        {
            if (sector.TryGetValue(playerId, out var player))
            {
                return player;
            }
        }

        return null;
    }

    IEnumerator EnterSector(int targetSector, PlayerInfo playerInfo)
    {
        // [1] 이전 섹터의 플레이어리스트 비움
        playerList[CurrentSector].Clear();
        // [2] 이동할 섹터의 매니저 찾고, 씬 로드 기다림
        StartCoroutine(SetSManager(targetSector));
        yield return new WaitUntil(() => sManager != null);
        // [3] 플레이어 오브젝트 생성 및 데이터 연동
        Player me = sManager.Enter(playerInfo);
        sManager.UiChat.Player = me;
        // [4] 현재 위치한 섹터 값 최신화
        CurrentSector = targetSector;
    }

    IEnumerator SpawnOthers(S2CSpawn pkt)
    {
        // [1] 스폰시킬 섹터의 매니저 찾고, 씬 로드 기다림
        yield return new WaitUntil(() => sManager != null);
        // [2] 플레이어 스폰 반복
        foreach (PlayerInfo playerInfo in pkt.Players)
        {
            // [2-1] 플레이어 정보 중 내 정보는 패스
            if (playerInfo.PlayerId == MPlayer.PlayerId)
                continue;
            // [2-2] 플레이어 오브젝트 생성 및 데이터 연동
            var player = sManager.SpawnPlayer(playerInfo);
            player.SetIsMine(false);
        }
    }

    private void LoadJson()
    {
        JsonFileLoader loader = new JsonFileLoader();

        // 단일 JSON 파일 로드
        string questFilePath = Path.Combine(Application.streamingAssetsPath, "Quest.json");
        string filePath = Path.Combine(Application.streamingAssetsPath, "material_item_data.json");

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

        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[0].quest_id}");
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[1].quest_id}");
        Debug.Log($"퀘스트 데이터 로드 완료: {questContainer.data[2].quest_id}");

        // 단일 JSON 파일 로드
        string resourceFilePath = Path.Combine(Application.streamingAssetsPath, "Resource.json");

        if (!File.Exists(resourceFilePath))
        {
            Debug.LogError($"resouce JSON 파일을 찾을 수 없습니다: {resourceFilePath}");
            return;
        }

        resourceContainer = loader.ReadJsonFile<JsonContainer<Resource>>(resourceFilePath);
        if (
            resourceContainer == null
            || resourceContainer.data == null
            || resourceContainer.data.Count == 0
        )
        {
            Debug.LogError("resouce JSON 파싱 실패: 데이터가 없습니다.");
            return;
        }
    }
}
