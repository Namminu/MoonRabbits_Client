using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class PlayerManager
{
    // 플레이어 컴포넌트와 저장 데이터를 관리하는 정적 딕셔너리
    public static Dictionary<int, Player> players = new Dictionary<int, Player>();
    public static Dictionary<int, SavePlayerData> playerSaveData = new Dictionary<int, SavePlayerData>();

    // 플레이어 등록 및 데이터 저장
    public static void RegisterPlayer(Player player)
    {
        if (player == null)
            return;

        int playerId = player.PlayerId;

        // Player 컴포넌트 참조 저장
        players[playerId] = player;

        // 플레이어 데이터 저장
        SavePlayerData(player);
    }

    // Player 컴포넌트에서 기본 데이터 추출하여 SavePlayerData에 저장
    public static void SavePlayerData(Player player)
    {
        if (player == null)
            return;

        int playerId = player.PlayerId;

        // 해당 플레이어의 SavePlayerData가 없으면 생성
        playerSaveData[playerId] = new SavePlayerData();

        SavePlayerData saveData = playerSaveData[playerId];

        // Player 컴포넌트에서 기본 데이터 저장 (파괴되지 않는 데이터)
        playerSaveData[playerId].PlayerId = playerId;
        saveData.Nickname = player.nickname;
        saveData.Level = player.level;
        saveData.CurHp = player.GetHp();
        saveData.IsMine = player.IsMine;
        saveData.IsStun = player.IsStun;
        saveData.IsImotal = player.GetIsImotal;

        // 위치와 회전 정보 저장
        if (player.transform != null)
        {
            saveData.Position = player.transform.position;
            saveData.Rotation = player.transform.rotation;
        }

        // 장비 정보 저장
        if (player.ActiveEquipObj == player.axe)
            saveData.CurrentEquip = 1;
        else if (player.ActiveEquipObj == player.pickAxe)
            saveData.CurrentEquip = 2;
        else
            saveData.CurrentEquip = 0;
    }

    // 플레이어 데이터 가져오기
    public static SavePlayerData GetPlayerSaveData(int playerId)
    {
        if (playerSaveData.TryGetValue(playerId, out SavePlayerData data))
            return data;
        return null;
    }

    // 플레이어 제거
    public static void RemovePlayer(int playerId)
    {
        players.Remove(playerId);
        // playerSaveData.Remove(playerId); 데이터는 유지
    }

    // 모든 데이터 초기화
    public static void ClearAll()
    {
        players.Clear();
        playerSaveData.Clear();
    }
}

// 파괴되지 않는 플레이어 데이터를 저장하는 클래스
public class SavePlayerData
{
    // 기본 정보
    public int PlayerId;
    public string Nickname;
    public int Level;
    public bool IsMine;
    public bool IsStun;
    public bool IsImotal;

    // 변환 정보
    public Vector3 Position;
    public Quaternion Rotation;

    // 장비 정보
    public int CurrentEquip;

    // 스탯 정보
    public int MaxHp;
    public int CurHp;
    public int Exp;
    public int TargetExp;
    public int Stamina;
    public int CurStamina;
    public int PickSpeed;
    public int MoveSpeed;
    public int AbilityPoint;

    // 네트워크 관련 정보
    public Vector3 GoalPos;
    public Quaternion GoalRot;

    // 감정표현 및 기타 정보
    public Dictionary<int, string> Emotions = new Dictionary<int, string>();
}