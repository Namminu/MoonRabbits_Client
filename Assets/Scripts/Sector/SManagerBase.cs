using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class SManagerBase : MonoBehaviour
{
    [SerializeField]
    private Transform spawnArea;

    [SerializeField]
    private Transform despawnArea;

    [SerializeField]
    private EventSystem eSystem;
    public EventSystem ESystem => eSystem;

    private UIChat uiChat;
    public UIChat UiChat => uiChat;
    private UIPlayer uiPlayer;
    public UIPlayer UiPlayer => uiPlayer;

    private Dictionary<int, Player> playerList = new();
    private Dictionary<int, string> prefabPaths = new();
    public Player myPlayer { get; private set; }

    private int planeCnt;
    private Dictionary<int, ResourceController> resources = new(); // key는 리소스인덱스
    private Dictionary<int, MonsterController> monsters = new(); // key는 몬스터인덱스

    void Start()
    {
        spawnArea = transform.Find("SpawnArea");
        despawnArea = transform.Find("DespawnArea");
    }

    protected void SetPrefabPath()
    {
        prefabPaths[1001] = "Player/Player1";
        prefabPaths[1002] = "Player/Player2";
        prefabPaths[1003] = "Player/Player3";
        prefabPaths[1004] = "Player/Player4";
        prefabPaths[1005] = "Player/Player5";
        Debug.Log(
            $"이거 작동해요?? {prefabPaths[1001]}, {prefabPaths[1002]}, {prefabPaths[1003]}, {prefabPaths[1004]}, {prefabPaths[1005]}"
        );
    }

    public void Enter(PlayerInfo playerInfo)
    {
        // [1] 프리펩 생성 및 정보 연동
        myPlayer = SpawnPlayer(playerInfo);
        // [2] "내" 프리펩인 걸 선언
        myPlayer.SetIsMine(true);
        // [3] 머리 위 닉네임 UI에 이름 박음
        // UiPlayer.SetNickname(playerInfo.Nickname);
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
        player.SetPlayerId(playerInfo.PlayerId);
        player.SetNickname(playerInfo.Nickname);
        player.SetLevel(playerInfo.Level);
        // [4] 이미 접속된 플레이어인지 확인
        if (playerList.TryGetValue(playerInfo.PlayerId, out var existingPlayer))
        {
            // [4 A] 중복 접속이면 기존 거 파괴하고 이번 꺼 덧씌움
            playerList[playerInfo.PlayerId] = player;
            Destroy(existingPlayer.gameObject);
        }
        else
        {
            // [4 B] 정상 접속이면 플레이어 리스트에 추가
            playerList.Add(playerInfo.PlayerId, player);
        }
        // [5] 생성된 플레이어 오브젝트 반환
        return player;
    }

    public void DespawnPlayer(int playerId)
    {
        // [1] 존재하는 플레이어인지 확인
        bool isExist = playerList.TryGetValue(playerId, out var player);
        if (!isExist)
            return;
        // [2] 실존하는 플레이어인지 확인
        if (player != null && player.gameObject != null)
            Destroy(player.gameObject);
        // [3] 플레이어 목록에서 제거
        playerList.Remove(playerId);
    }

    public Player GetPlayer(int playerId)
    {
        bool isExist = playerList.TryGetValue(playerId, out var player);
        return isExist ? player : null;
    }
}
