using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<int, Player> playerList = new();
    private readonly Dictionary<int, string> prefabPaths = new();
    public Player MyPlayer { get; private set; }

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
        if (!UiChat.gameObject.activeSelf)
            UiChat.gameObject.SetActive(true);
    }

    public Player Enter(PlayerInfo playerInfo)
    {
        // [1] UI 활성화
        ActivateUI();
        // [2] 플레이어 프리펩 생성 및 정보 연동
        MyPlayer = SpawnPlayer(playerInfo);
        // [3] "내" 프리펩임 선언
        MyPlayer.SetIsMine(true);
        // [4] 머리 위 닉네임 UI에 이름 박음
        MyPlayer.SetUI(UiPlayer);
        MyPlayer.SetNickname(playerInfo.Nickname);
        MyPlayer.SetStatInfo(playerInfo.StatInfo);
        PartyMemberUI.instance.UpdateUI();
        // [5] "내" 플레이어 오브젝트 반환
        return MyPlayer;
    }

    public Player SpawnPlayer(PlayerInfo playerInfo)
    {
        // [1] 플레이어 프리펩 가져올 경로 찾기
        bool hasPrefab = prefabPaths.TryGetValue(playerInfo.ClassCode, out string prefabPath);
        if (!hasPrefab)
        {
            Debug.Log($"플레이어 프리펩을 찾지 못 했습니다 : {prefabPath}");
            // 서버로 실패 패킷?
            return null;
        }
        // [2] Resources 폴더에서 플레이어 프리펩 로드
        Player playerPrefab = Resources.Load<Player>(prefabPath);

        // [3] 프리펩 생성 및 정보 연동
        var player = Instantiate(playerPrefab, spawnArea.position, Quaternion.identity);
        player.Move(spawnArea.position, Quaternion.identity);
        player.SetPlayerId(playerInfo.PlayerId);
        player.SetNickname(playerInfo.Nickname);
        player.SetLevel(playerInfo.Level);
        // [4] 이미 접속된 플레이어인지 확인
        var players = GameManager.Instance.PlayerList[SectorCode];
        if (playerList.TryGetValue(playerInfo.PlayerId, out var existingPlayer))
        {
            // [4 A] 중복 접속이면 기존 거 파괴하고 이번 꺼 덧씌움
            playerList[playerInfo.PlayerId] = player;
            Destroy(existingPlayer.gameObject);
        }
        else
        {
            // [4 B] 정상 접속이면 플레이어 리스트에 추가
            players.Add(playerInfo.PlayerId, player);
        }
        // [5] 생성된 플레이어 오브젝트 반환
        return player;
    }

    public void DespawnPlayer(int playerId)
    {
        // [1] 존재하는 플레이어인지 확인
        var players = GameManager.Instance.PlayerList[SectorCode];
        if (!players.TryGetValue(playerId, out var player))
        {
            return;
        }
        // [2] 해당 플레이어 오브젝트 확인후 파괴
        if (player != null && player.gameObject != null)
        {
            Destroy(player.gameObject);
        }
        // [3] 플레이어 목록에서 제거
        players.Remove(playerId);
    }

    public Player GetPlayer(int playerId)
    {
        bool isExist = playerList.TryGetValue(playerId, out var player);
        return isExist ? player : null;
    }
}
