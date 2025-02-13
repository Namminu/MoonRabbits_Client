using System.Collections.Generic;
using Cinemachine;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class TownManager : MonoBehaviour
{
    private static TownManager _instance;
    public static TownManager Instance => _instance;

    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private EventSystem eSystem;
    [SerializeField] private UIStart uiStart;
    [SerializeField] private UIAnimation uiAnimation;
    [SerializeField] private UIChat uiChat;
    [SerializeField] private TMP_Text txtServer;

    private const string DefaultPlayerPath = "Player/Player1";

    public CinemachineFreeLook FreeLook => freeLook;
    public EventSystem E_System => eSystem;
    public UIChat UiChat => uiChat;

    private Dictionary<int, Player> playerList = new();
    private Dictionary<int, string> playerDb = new();

    public Player MyPlayer { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePlayerDatabase();
    }

    private void Start()
    {
        if (!GameManager.Network.IsConnected)
        {
            uiStart.gameObject.SetActive(true);
        }
        else
        {
            Connected();
        }
    }

    private void InitializePlayerDatabase()
    {
        playerDb[1001] = "Player/Player1";
        playerDb[1002] = "Player/Player2";
        playerDb[1003] = "Player/Player3";
        playerDb[1004] = "Player/Player4";
        playerDb[1005] = "Player/Player5";
    }

    public void GameStart(string gameServer, string port, string userName, int classIdx)
    {
        GameManager.Network.Init(gameServer, port);
        GameManager.Instance.UserName = userName;
        GameManager.Instance.ClassIdx = classIdx + 1001;
        txtServer.text = gameServer;
    }

    public void Connected()
    {
        var enterPacket = new C_Enter
        {
            Nickname = GameManager.Instance.UserName,
            Class = GameManager.Instance.ClassIdx
        };

        GameManager.Network.Send(enterPacket);
    }

    public void Spawn(PlayerInfo playerInfo)
    {
        Vector3 spawnPos = CalculateSpawnPosition(playerInfo.Transform);

        MyPlayer = CreatePlayer(playerInfo, spawnPos);
        MyPlayer.SetIsMine(true);

        ActivateGameUI();
    }

    private Vector3 CalculateSpawnPosition(TransformInfo transformInfo)
    {
        Vector3 spawnPos = spawnArea.position;
        spawnPos.x += transformInfo.PosX;
        spawnPos.z += transformInfo.PosZ;
        return spawnPos;
    }

    public Player CreatePlayer(PlayerInfo playerInfo, Vector3 spawnPos)
    {
        string playerResPath = playerDb.GetValueOrDefault(playerInfo.Class, DefaultPlayerPath);
        Player playerPrefab = Resources.Load<Player>(playerResPath);

        // NavMesh 위의 가장 가까운 위치 찾기
        Vector3 validatedSpawnPos = GetNearestNavMeshPosition(spawnPos, 3.0f);

        var player = Instantiate(playerPrefab, validatedSpawnPos, Quaternion.identity);
        player.Move(validatedSpawnPos, Quaternion.identity);
        player.SetPlayerId(playerInfo.PlayerId);
        player.SetNickname(playerInfo.Nickname);

        if (playerList.TryGetValue(playerInfo.PlayerId, out var existingPlayer))
        {
            playerList[playerInfo.PlayerId] = player;
            Destroy(existingPlayer.gameObject);
        }
        else
        {
            playerList.Add(playerInfo.PlayerId, player);
        }

        return player;
    }

    // NavMesh 위에서 가장 가까운 위치를 찾는 함수
    private Vector3 GetNearestNavMeshPosition(Vector3 position, float maxSearchDistance)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, maxSearchDistance, NavMesh.AllAreas))
        {
            return hit.position; // NavMesh 위의 가장 가까운 위치 반환
        }
        else
        {
            Debug.LogWarning("NavMesh 위의 위치를 찾을 수 없음, 기본 안전한 위치 반환");
            return new Vector3(-5, 1, 137); // NavMesh가 있는 기본 안전한 위치로 변경
        }
    }


    public void ReleasePlayer(int playerId)
    {
        if (!playerList.TryGetValue(playerId, out var player)) return;

        playerList.Remove(playerId);
        Destroy(player.gameObject);
    }

    private void ActivateGameUI()
    {
        uiStart.gameObject.SetActive(false);
        uiChat.gameObject.SetActive(true);
        uiAnimation.gameObject.SetActive(true);
    }

    public Player GetPlayerAvatarById(int playerId)
    {
        return playerList.TryGetValue(playerId, out var player) ? player : null;
    }
}