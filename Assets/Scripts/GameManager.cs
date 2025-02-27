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
    public int PlayerId;
    public int ClassCode;

    private async void Awake()
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

        //SoundManager.Instance.Play(19, Define.Sound.Bgm);
        await ItemDataLoader.GenerateAllItems();
    }

    void Start()
    {
        JsonFileLoader loader = new JsonFileLoader();

        // 단일 JSON 파일 로드
        string filePath = Path.Combine(Application.streamingAssetsPath, "material_item_data.json");

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

    public void WaitForSceneAwake(S2CSpawn pkt)
    {
        StartCoroutine(SpawnPlayer(pkt));
    }

    IEnumerator EnterScene(string sceneName, PlayerInfo playerInfo)
    {
        switch (sceneName)
        {
            case "Town":
                yield return new WaitUntil(() => TownManager.Instance != null);
                TownManager.Instance.Enter(playerInfo);
                break;
            case "Sector1":
                yield return new WaitUntil(() => S1Manager.Instance != null);
                S1Manager.Instance.Enter(playerInfo);
                MyPlayer.instance.eSystem = S1Manager.Instance.ESystem;
                break;
            case "Sector2":
                yield return new WaitUntil(() => S2Manager.Instance != null);
                S2Manager.Instance.Enter(playerInfo);
                MyPlayer.instance.eSystem = S2Manager.Instance.ESystem;
                break;
        }
    }

    IEnumerator SpawnPlayer(S2CSpawn pkt)
    {
        foreach (var playerInfo in pkt.Players)
        {
            switch (playerInfo.CurrentSector)
            {
                case 100:
                    yield return new WaitUntil(() => TownManager.Instance != null);
                    if (
                        TownManager.Instance.MyPlayer != null
                        && playerInfo.PlayerId == TownManager.Instance.MyPlayer.PlayerId
                    )
                        continue;

                    Vector3 spawnPosTown = new Vector3(
                        playerInfo.Transform.PosX,
                        playerInfo.Transform.PosY,
                        playerInfo.Transform.PosZ
                    );
                    var townPlayer = TownManager.Instance.SpawnPlayer(playerInfo, spawnPosTown);
                    townPlayer.SetIsMine(false, playerInfo.CurrentSector);
                    break;
                case 2:
                    if (
                        ASectorManager.Instance.MyPlayer != null
                        && playerInfo.PlayerId == ASectorManager.Instance.MyPlayer.PlayerId
                    )
                        continue;

                    Vector3 spawnPosSectorA = new Vector3(
                        playerInfo.Transform.PosX,
                        playerInfo.Transform.PosY,
                        playerInfo.Transform.PosZ
                    );
                    var sectorPlayer = ASectorManager.Instance.CreatePlayer(
                        playerInfo,
                        spawnPosSectorA
                    );
                    sectorPlayer.SetIsMine(false, playerInfo.CurrentSector);
                    break;
                case 101:
                    yield return new WaitUntil(() => S1Manager.Instance != null);
                    if (
                        S1Manager.Instance.MyPlayer != null
                        && playerInfo.PlayerId == S1Manager.Instance.MyPlayer.PlayerId
                    )
                        continue;
                    var s1Player = S1Manager.Instance.SpawnPlayer(playerInfo);
                    s1Player.SetIsMine(false, playerInfo.CurrentSector);
                    break;
                case 102:
                    yield return new WaitUntil(() => S2Manager.Instance != null);
                    if (
                        S2Manager.Instance.MyPlayer != null
                        && playerInfo.PlayerId == S2Manager.Instance.MyPlayer.PlayerId
                    )
                        continue;
                    var s2Player = S2Manager.Instance.SpawnPlayer(playerInfo);
                    s2Player.SetIsMine(false, playerInfo.CurrentSector);
                    break;
            }
        }
    }
}
