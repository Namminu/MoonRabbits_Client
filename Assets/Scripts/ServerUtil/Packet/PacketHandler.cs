using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
    #region Account & Character
    public static void S2CRegisterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CRegister pkt)
            return;
        Debug.Log($"S2CRegister 패킷 무사 도착 : {pkt}");
        EventManager.Trigger("DisplayMessage", pkt.Msg);
    }

    public static void S2CLoginHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CLogin pkt)
            return;
        Debug.Log($"S2CLogin 패킷 무사 도착 : {pkt}");

        if (!pkt.IsSuccess)
        {
            EventManager.Trigger("DisplayMessage", pkt.Msg);
            return;
        }

        List<Google.Protobuf.Protocol.OwnedCharacter> charsInfo = pkt.OwnedCharacters.ToList();
        EventManager.Trigger("CheckHasChar", charsInfo);
    }

    public static void S2CCreateCharacterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CCreateCharacter pkt)
            return;
        Debug.Log($"S2CCreateCharacter 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region Enter & Leave
    public static void S2CEnterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CEnter pkt)
            return;
        Debug.Log($"S2CEnter 패킷 무사 도착 : {pkt}");

        switch (pkt.Player.CurrentScene)
        {
            case 1:
                TownManager.Instance.Spawn(pkt.Player);
                break;
            case 2:
                ASectorManager.Instance.Spawn(pkt.Player);
                break;
        }
    }

    public static void S2CLeaveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CLeave pkt)
            return;
        Debug.Log($"S2CLeave 패킷 무사 도착 : {pkt}");
    }

    public static void S2CAnimationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CAnimation pkt)
            return;
        Debug.Log($"S2CAnimation 패킷 무사 도착 : {pkt}");

        var player = TownManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
        player?.PlayAnimation(pkt.AnimCode);
    }
    #endregion

    #region Chat & Spawn
    public static void S2CChatHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CChat pkt)
            return;
        Debug.Log($"S2CChat 패킷 무사 도착 : {pkt}");

        if (pkt.PlayerId > 0)
        {
            Debug.Log("pkt.PlayerId : " + pkt.PlayerId);
            var player = TownManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
            player?.RecvMessage(pkt.ChatMsg);
        }
        else
        {
            TownManager.Instance.UiChat.PushMessage("System", pkt.ChatMsg, true);
        }
    }

    public static void S2CPlayerSpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CPlayerSpawn pkt)
            return;
        Debug.Log($"S2CPlayerSpawn 패킷 무사 도착 : {pkt}");

        foreach (var playerInfo in pkt.Players)
        {
            switch (playerInfo.CurrentScene)
            {
                case 1:
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
                    var townPlayer = TownManager.Instance.CreatePlayer(playerInfo, spawnPosTown);
                    townPlayer.SetIsMine(false);
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
                    sectorPlayer.SetIsMine(false);
                    break;
            }

            // 여기에서 localPlayer 설정이 작동하지 않는 버그가 발생 하여 해당 예외처리를 추가함.
        }
    }

    public static void S2CPlayerDespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CPlayerDespawn pkt)
            return;
        Debug.Log($"S2CPlayerDespawn 패킷 무사 도착 : {pkt}");

        switch (pkt.CurrentScene)
        {
            case 1:
                foreach (int playerId in pkt.PlayerIds)
                {
                    TownManager.Instance.ReleasePlayer(playerId);
                }
                break;
            case 2:
                foreach (int playerId in pkt.PlayerIds)
                {
                    ASectorManager.Instance.ReleasePlayer(playerId);
                }
                break;
        }
    }
    #endregion

    #region Move(Player)
    public static void S2CPlayerMoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CPlayerMove pkt)
            return;
        Debug.Log($"S2CPlayerMove 패킷 무사 도착 : {pkt}");
    }

    public static void S2CPlayerLocationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CPlayerLocation pkt)
            return;
        Debug.Log($"S2CPlayerLocation 패킷 무사 도착 : {pkt}");

        TransformInfo transform = pkt.Transform;
        Vector3 position = new Vector3(transform.PosX, transform.PosY, transform.PosZ);
        Quaternion rotation = Quaternion.Euler(0, transform.Rot, 0);
        bool isValidTransform = pkt.IsValidTransform;

        switch (pkt.CurrentScene)
        {
            case 1:
                var townPlayer = TownManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
                townPlayer?.Move(position, rotation);
                break;
            case 2:
                var sectorPlayer = ASectorManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
                sectorPlayer?.Move(position, rotation);
                break;
        }
    }

    public static void S2CPlayerRunningHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CPlayerRunning pkt)
            return;
        Debug.Log($"S2CPlayerRunning 패킷 무사 도착 : {pkt}");
    }
    #endregion

    public static void S2CUpdateRankingHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CUpdateRanking pkt)
            return;
        Debug.Log($"S2CUpdateRanking 패킷 무사 도착 : {pkt}");
    }

    #region Collision
    public static void S2CPlayerCollisionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CPlayerCollision pkt)
            return;
        Debug.Log($"S2CPlayerCollision 패킷 무사 도착 : {pkt}");
    }

    public static void S2CMonsterCollisionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CMonsterCollision pkt)
            return;
        Debug.Log($"S2CMonsterCollision 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region Store
    public static void S2CSelectStoreHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CSelectStore pkt)
            return;
        Debug.Log($"S2CSelectStore 패킷 무사 도착 : {pkt}");
    }

    public static void S2CBuyItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CBuyItem pkt)
            return;
        Debug.Log($"S2CBuyItem 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region Party
    public static void S2CCreatePartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CCreateParty pkt)
            return;
        Debug.Log($"S2CCreateParty 패킷 무사 도착 : {pkt}");
        Party.instance.CreatePartyData(pkt);
    }

    public static void S2CInvitePartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CInviteParty pkt)
            return;
        Debug.Log($"S2CInviteParty 패킷 무사 도착 : {pkt}");

        string leaderNickname = pkt.LeaderNickname;
        string partyId = pkt.PartyId;
        int memberId = pkt.MemberId;

        // 수락 팝업창 띄우기
        PartyUI.instance.OpenAllowInvitePopUp(partyId, leaderNickname, memberId);
    }

    public static void S2CAllowInviteHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CAllowInvite pkt)
            return;
        Debug.Log($"S2CAllowInvite 패킷 무사 도착 : {pkt}");
        Party.instance.AllowInviteData(pkt);
    }

    public static void S2CJoinPartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CJoinParty pkt)
            return;
        Debug.Log($"S2CJoinParty 패킷 무사 도착 : {pkt}");
        Party.instance.JoinPartyData(pkt);
    }

    public static void S2CLeavePartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CLeaveParty pkt)
            return;
        Debug.Log($"S2CLeaveParty 패킷 무사 도착 : {pkt}");
        Party.instance.LeavePartyData(pkt);
    }

    public static void S2CCheckPartyListHandler(PacketSession session, IMessage packet)
    {
        Debug.Log($"!!!! 패킷 !! : {packet}");
        if (packet is not S2CCheckPartyList pkt)
        {
            Debug.Log("S2CCheckPartyList 패킷의 상태가 이상하다.");

            return;
        }

        Debug.Log($"S2CCheckPartyList 패킷 무사 도착 : {pkt}");
        PartyUI.instance.createPartyCard(pkt);
    }

    public static void S2CKickOutMemberHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CKickOutMember pkt)
            return;
        Debug.Log($"S2CKickOutMember 패킷 무사 도착 : {pkt}");
        Party.instance.KickedOutData(pkt);
    }

    public static void S2CDisbandPartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CDisbandParty pkt)
            return;
        Debug.Log($"S2CDisbandParty 패킷 무사 도착 : {pkt}");
        PartyUI.instance.KickedOut(pkt.Msg);
    }
    public static void S2CRejectInviteHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CRejectInvite pkt)
            return;
        Debug.Log($"S2CRejectInvite 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region Sector
    public static void S2CMonsterLocationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CMonsterLocation pkt)
            return;
        var monsterId = pkt.MonsterId;
        var monsterPosition = pkt.TransformInfo;

        Vector3 position = new Vector3(monsterPosition.PosX, monsterPosition.PosY, monsterPosition.PosZ);
        MonsterManager.Instance.SendPositionPacket(monsterId, position);

        Debug.Log($"S2CMonsterLocation 패킷 무사 도착 : {pkt}");
    }

    public static void S2CDetectedPlayerHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CDetectedPlayer pkt)
            return;
        Debug.Log($"S2CDetectedPlayer 패킷 무사 도착 : {pkt}");
    }

    public static void S2CMissingPlayerHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CMissingPlayer pkt)
            return;
        Debug.Log($"S2CMissingPlayer 패킷 무사 도착 : {pkt}");
    }

    public static void S2CResourceListHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CResourceList pkt)
            return;
        Debug.Log($"S2CResourceList 패킷 무사 도착 : {pkt}");
    }

    public static void S2CUpdateDurabilityHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CUpdateDurability pkt)
            return;
        Debug.Log($"S2CUpdateDurability 패킷 무사 도착 : {pkt}");
    }

    public static void S2CGatheringStartHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CGatheringStart pkt)
            return;
        Debug.Log($"S2CGatheringStart 패킷 무사 도착 : {pkt}");
    }

    public static void S2CGatheringSkillCheckHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CGatheringSkillCheck pkt)
            return;
        Debug.Log($"S2CGatheringSkillCheck 패킷 무사 도착 : {pkt}");
    }

    public static void S2CGatheringDoneHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CGatheringDone pkt)
            return;
        Debug.Log($"S2CGatheringDone 패킷 무사 도착 : {pkt}");
    }

    public static void S2CSectorEnterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CSectorEnter pkt)
            return;
        Debug.Log($"S2CSectorEnter 패킷 무사 도착 : {pkt}");

        // Scene currentScene = SceneManager.GetActiveScene();

        // if (currentScene.name == GameManager.BattleScene)
        // {
        //     BattleManager.Instance.ConfigureGame(pkt);
        // }
        // else
        // {
        //     GameManager.Instance.Pkt = pkt;
        //     SceneManager.LoadScene(GameManager.BattleScene);
        // }
    }

    public static void S2CSectorLeaveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CSectorLeave pkt)
            return;
        Debug.Log($"S2CSectorLeave 패킷 무사 도착 : {pkt}");

        // SceneManager.LoadScene(GameManager.TownScene);
    }

    public static void S2CInPortalHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CInPortal pkt)
            return;
        Debug.Log($"S2CInPortal 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region LevelUp
    public static void S2CAddExpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CAddExp pkt)
            return;
        Debug.Log($"S2CAddExp 패킷 무사 도착 : {pkt}");

        TownManager.Instance.UiPlayer.SetExp(pkt.UpdatedExp);
    }

    public static void S2CLevelUpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CLevelUp pkt)
            return;
        Debug.Log($"S2CLevelUp 패킷 무사 도착 : {pkt}");

        TownManager.Instance.UiPlayer.LevelUp(pkt.UpdatedLevel, pkt.NewTargetExp, pkt.UpdatedExp, pkt.AbilityPoint);
    }

    public static void S2CInvestPointHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CInvestPoint pkt)
            return;
        Debug.Log($"S2CInvestPoint 패킷 무사 도착 : {pkt}");

        TownManager.Instance.UiPlayer.InvestPoint(pkt.StatInfo);
    }
    #endregion
}
