using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public abstract class SManagerBase : MonoBehaviour
{
    private int sectorCode;
    public int SectorCode
    {
        get { return sectorCode; }
        set { sectorCode = value; }
    }

    [SerializeField]
    private Transform spawnArea;

    [SerializeField]
    private Transform despawnArea;

    [SerializeField]
    private EventSystem eSystem;
    public EventSystem ESystem => eSystem;

    [SerializeField]
    private UIChat uiChat;
    public UIChat UiChat => uiChat;

    [SerializeField]
    private UIPlayer uiPlayer;
    public UIPlayer UiPlayer => uiPlayer;

    [SerializeField]
    private SkillObj trap;

    [SerializeField]
    private Chest chest;
    public Chest Chest => chest;

    private readonly Dictionary<int, string> prefabPaths = new();
    public Player MPlayer { get; private set; }

    public HashSet<int> spawnWaitings = new(); // spawnPlayer 메서드를 거치고 있는 플레이어 아이디들을 보관

    protected virtual void Awake()
    {
        uiChat = CanvasManager.Instance.uIChat;
        uiPlayer = CanvasManager.Instance.uIPlayer;
        trap = Resources.Load<SkillObj>("Prefabs/Weapon/ThrowObj/Trap");
    }

    protected void SetPrefabPath()
    {
        prefabPaths[1001] = "Player/Player1";
        prefabPaths[1002] = "Player/Player2";
        prefabPaths[1003] = "Player/Player3";
        prefabPaths[1004] = "Player/Player4";
        prefabPaths[1005] = "Player/Player5";
    }

    protected virtual void ActivateUI()
    {
        CanvasManager.Instance.ActivateUI();
    }

    public void Enter(List<PlayerInfo> players)
    {
        // [1] UI 활성화
        ActivateUI();
        // [2] 받은 플레이어 정보들 순회하며 내꺼는 마킹
        foreach (PlayerInfo playerInfo in players)
        {
            if (playerInfo == null)
                continue;

            var player = SpawnPlayer(playerInfo);

            if (player == null)
                continue;

            // 플레이어 매니저에서 정보 업데이트
            PlayerManager.RegisterPlayer(player);

            if (playerInfo.Nickname == GameManager.Instance.NickName)
            {
                player.SetIsMine(true);
                MPlayer = player;
                MPlayer.SetStatInfo(playerInfo.StatInfo);
                uiChat.Player = MPlayer;
            }
            else
            {
                player.SetIsMine(false);
            }
        }

        PartyMemberUI.instance.UpdateUI();
    }

    public Player SpawnPlayer(PlayerInfo playerInfo)
    {
        // [0] 스폰 대기 리스트에 추가
        if (spawnWaitings.Contains(playerInfo.PlayerId))
        {
            return null;
        }
        else
        {
            spawnWaitings.Add(playerInfo.PlayerId);
        }
        // [1] 플레이어 프리펩 가져올 경로 찾기
        bool hasPrefab = prefabPaths.TryGetValue(playerInfo.ClassCode, out string prefabPath);
        if (!hasPrefab)
        {
            Debug.Log($"플레이어 프리펩을 찾지 못 했습니다 : {prefabPath}");
            return null;
        }

        // [2] Resources 폴더에서 플레이어 프리펩 로드
        Player playerPrefab = Resources.Load<Player>(prefabPath);

        // [3] 스폰 위치 설정 (최초 입장이면 스폰 위치, 아니라면 전달받은 위치 정보)
        Vector3 spawnPos =
            playerInfo.Transform.PosX == 0
            && playerInfo.Transform.PosY == 0
            && playerInfo.Transform.PosZ == 0
                ? spawnArea.position
                : new Vector3(playerInfo.Transform.PosX, 0, playerInfo.Transform.PosZ);

        // [4] 이미 생성된 플레이어 오브젝트인지 확인
        var players = GameManager.Instance.PlayerList[SectorCode];
        if (players.TryGetValue(playerInfo.PlayerId, out var existingPlayer))
        {
            players.Remove(playerInfo.PlayerId);
            if (existingPlayer != null)
            {
                Destroy(existingPlayer.gameObject);
            }
        }
        // [4-2] 현재 내 씬에서도 혹시 생성된 오브젝트가 있는지 확인
        foreach (Player p in GameObject.FindObjectsOfType<Player>())
        {
            if (p.PlayerId == playerInfo.PlayerId)
            {
                Destroy(p.gameObject);
            }
        }
        // [5] 프리펩 생성
        var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // [6] 플레이어 리스트에 추가
        players.Add(playerInfo.PlayerId, player);

        // [7] 서버 정보 연동
        player.Move(spawnPos, Quaternion.identity);
        player.SetPlayerId(playerInfo.PlayerId);
        player.SetNickname(playerInfo.Nickname);
        player.SetLevel(playerInfo.Level);
        player.SetHp(playerInfo.StatInfo.Hp);

        PartyMemberUI.instance.UpdateUI();

        // [8] 스폰 대기 리스트에서 지우고, 생성된 플레이어 오브젝트 반환
        spawnWaitings.Remove(player.PlayerId);
        return player;
    }

    public void SetTraps(List<TrapInfo> traps)
    {
        if (trap == null)
        {
            Debug.Log("Trap Prefab을 찾지 못했습니다");
            return;
        }

        foreach (TrapInfo trapInfo in traps)
        {
            var trapObj = Instantiate(
                trap,
                new Vector3(trapInfo.Pos.X / 10f, 0, trapInfo.Pos.Z / 10f),
                Quaternion.identity
            );

            SkillObj skillObj = trapObj.GetComponent<SkillObj>();
            skillObj.CasterId = trapInfo.CasterId;

            skillObj.SetTrapColor();
        }
    }
}
