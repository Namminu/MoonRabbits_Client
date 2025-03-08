using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    private bool _isLowSpecMode;
    public bool IsLowSpecMode
    {
        get { return _isLowSpecMode; }
        set
        {
            _isLowSpecMode = value;
            var isLowSpec = GameManager.Instance.IsLowSpecMode;
            var volum = FindObjectOfType<Volume>();
            if (volum)
                volum.gameObject.SetActive(isLowSpec);
            UniversalAdditionalCameraData uac =
                Camera.main.GetComponent<UniversalAdditionalCameraData>();
            uac.renderPostProcessing = isLowSpec;
            if (isLowSpec)
            {
                QualitySettings.SetQualityLevel(0, true); // 0은 Low, 1은 Medium, 2는 High, 3은 Very High
            }
            else
            {
                QualitySettings.SetQualityLevel(3, true); // 0은 Low, 1은 Medium, 2는 High, 3은 Very High
            }
        }
    }

    private NetworkManager network;
    public static NetworkManager Network => _instance.network;

    private SManagerBase sManager;
    public SManagerBase SManager => sManager;

    [Header("Me")]
    public string NickName;
    public int ClassCode;
    public int CurrentSector;
    public Player MPlayer;

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
    public JsonContainer<Recipe> recipeContainer;
    public JsonContainer<ItemJson> materialItemContainer;
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

            if (!IsLowSpecMode)
                EffectManager.Instance.CreatePersistentEffect(
                    "Confetti",
                    new Vector3(-3, 14, 134),
                    Quaternion.identity
                );
        }
        else
        {
            Destroy(gameObject);
        }

        LoadJson();
        //SoundManager.Instance.Play(19, Define.Sound.Bgm);
        await ItemDataLoader.GenerateAllItems();
    }

    void Start()
    {
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
        {
            yield return new WaitUntil(() => TownManager.Instance != null);
            sManager = TownManager.Instance;
            yield break;
        }

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

    public void EnterAfterSceneAwake(List<PlayerInfo> playerInfos)
    {
        StartCoroutine(EnterTown(playerInfos));
    }

    public void EnterAfterSceneAwake(
        int targetSector,
        List<PlayerInfo> playerInfos,
        List<TrapInfo> trapInfos
    )
    {
        StartCoroutine(EnterSector(targetSector, playerInfos, trapInfos));
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
            if (sector.TryGetValue(playerId, out Player player))
            {
                return player;
            }
        }
        return null;
    }

    public Player GetPlayer(string nickname)
    {
        foreach (Dictionary<int, Player> sector in playerList.Values)
        {
            foreach (Player player in sector.Values)
            {
                if (player.nickname == nickname)
                {
                    return player;
                }
            }
        }

        return null;
    }

    IEnumerator EnterTown(List<PlayerInfo> playerInfos)
    {
        // [1] 마을 섹터의 매니저 찾고, Awake 기다림
        StartCoroutine(SetSManager(100));
        yield return new WaitUntil(() => sManager != null);
        // [2] 마을에 있는 플레이어들 생성하고, 닉네임을 통해 내 플레이어면 마킹
        sManager.Enter(playerInfos);

        Invoke("Wait3Sec", 10f);
    }

    void Wait3Sec()
    {
        network.Reconnect(); // 스트림, 소켓 닫고, 씬 로드 다시한다...

        // ConfirmServer
        TownManager.Instance.TryConnectToServer(network._ipString, "3000");
        // TownManager.Instance.uiStart.gameObject.SetActive(false);
        // TownManager.Instance.uiStart.UILogin.SetActive(true);

        // ConfirmNickname
        string nickname = NickName;

        var dataPacket = new C2SCreateCharacter
        {
            Nickname = nickname,
            ClassCode = PlayerManager.classCode,
        };
        network.Send(dataPacket);
        TownManager.Instance.uiStart.gameObject.SetActive(false);
        TownManager.Instance.uiStart.UILogin.SetActive(true);
        TownManager.Instance.uiStart.gameObject.SetActive(false);


        // 엔터를 다시 하기
        var enterPacket = new C2SEnterTown { Nickname = NickName, ClassCode = PlayerManager.classCode };
        network.Send(enterPacket);
    }

    IEnumerator EnterSector(
        int targetSector,
        List<PlayerInfo> playerInfos,
        List<TrapInfo> trapInfos
    )
    {
        // [1] 이동할 섹터의 매니저 찾고, 씬 로드 기다림
        StartCoroutine(SetSManager(targetSector));
        yield return new WaitUntil(() => sManager != null);
        // [2] 해당 섹터에 있는 플레이어들 생성하고, 닉네임을 통해 내 플레이어면 마킹
        sManager.Enter(playerInfos);
        // [3] 설치된 덫이 하나라도 있으면 동기화
        if (trapInfos.Count >= 1)
        {
            sManager.SetTraps(trapInfos);
        }
        // [3] 현재 위치한 섹터 값 최신화
        CurrentSector = targetSector;
    }

    public void DespawnPlayer(int playerId)
    {
        // [1] 전체 플레이어 리스트 순회
        foreach (Dictionary<int, Player> sector in playerList.Values)
        {
            if (sector.TryGetValue(playerId, out Player player))
            {
                // [2] 플레이어 목록에서 제거
                sector.Remove(playerId);
                // [3] 해당 플레이어 오브젝트 확인후 파괴
                if (player != null && !player.IsMine)
                {
                    Destroy(player.gameObject);
                }
            }
        }
    }

    private void LoadJson()
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

        // recipe.json, materialItem.json 파일 로드
        string[] jsonFiles = { "recipe.json", "material_item_data.json" };

        foreach (string jsonFile in jsonFiles)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, jsonFile);

            if (!File.Exists(filePath))
            {
                Debug.LogError($"{jsonFile} 파일을 찾을 수 없습니다: {filePath}");
                continue;
            }

            if (jsonFile == "recipe.json")
            {
                recipeContainer = loader.ReadJsonFile<JsonContainer<Recipe>>(filePath);
                if (recipeContainer == null || recipeContainer.data == null || recipeContainer.data.Count == 0)
                {
                    Debug.LogError($"{jsonFile} 파싱 실패: 데이터가 없습니다.");
                    continue;
                }
            }
            if (jsonFile == "material_item_data.json")
            {
                materialItemContainer = loader.ReadJsonFile<JsonContainer<ItemJson>>(filePath);
                if (materialItemContainer == null || materialItemContainer.data == null || materialItemContainer.data.Count == 0)
                {
                    Debug.LogError($"{jsonFile} 파싱 실패: 데이터가 없습니다.");
                    continue;
                }
            }
        }
    }
}
