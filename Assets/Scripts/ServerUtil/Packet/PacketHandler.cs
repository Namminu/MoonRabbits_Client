using System;
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

        switch (pkt.Player.CurrentSector)
        {
            case 100:
                if (SceneManager.GetActiveScene().name != "Town")
                {
                    SceneManager.LoadScene("Town");
                    GameManager.Instance.WaitForSceneAwake("Town", pkt.Player);
                }
                else
                {
                    TownManager.Instance.Enter(pkt.Player);
                }
                break;
            case 2:
                ASectorManager.Instance.Spawn(pkt.Player);
                break;
            case 101:
                SceneManager.LoadScene("Sector1");
                GameManager.Instance.WaitForSceneAwake("Sector1", pkt.Player);
                break;
            case 102:
                SceneManager.LoadScene("Sector2");
                GameManager.Instance.WaitForSceneAwake("Sector2", pkt.Player);
                break;
        }
    }

    public static void S2CMoveSectorHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CMoveSector pkt)
            return;
        Debug.Log($"S2CMoveSector 패킷 무사 도착 : {pkt}");
    }

    public static void S2CAnimationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CAnimation pkt)
            return;
        Debug.Log($"S2CAnimation 패킷 무사 도착 : {pkt}");

        switch (pkt.CurrentSector)
        {
            case 100:
                var townPlayer = TownManager.Instance.GetPlayer(pkt.PlayerId);
                townPlayer?.Emote(pkt.AnimCode);
                break;
            case 101:
                var s1Player = S1Manager.Instance.GetPlayer(pkt.PlayerId);
                s1Player?.Emote(pkt.AnimCode);
                break;
            case 102:
                var s2Player = S2Manager.Instance.GetPlayer(pkt.PlayerId);
                s2Player?.Emote(pkt.AnimCode);
                break;
        }
    }
    #endregion

    #region Chat & Spawn
    public static void S2CChatHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CChat pkt)
            return;
        Debug.Log($"S2CChat 패킷 무사 도착 : {pkt}");

        switch (pkt.CurrentSector)
        {
            case 100:
                if (pkt.PlayerId > 0)
                {
                    var player = TownManager.Instance.GetPlayer(pkt.PlayerId);
                    player?.RecvMessage(pkt.ChatMsg, pkt.ChatType);
                }
                else
                {
                    TownManager.Instance.UiChat.PushMessage(
                        "System",
                        pkt.ChatMsg,
                        pkt.ChatType,
                        true
                    );
                }
                break;
            case 101:
                if (pkt.PlayerId > 0)
                {
                    var player = S1Manager.Instance.GetPlayer(pkt.PlayerId);
                    player?.RecvMessage(pkt.ChatMsg, pkt.ChatType);
                }
                else
                {
                    S1Manager.Instance.UiChat.PushMessage(
                        "System",
                        pkt.ChatMsg,
                        pkt.ChatType,
                        true
                    );
                }
                break;
            case 102:
                if (pkt.PlayerId > 0)
                {
                    var player = S2Manager.Instance.GetPlayer(pkt.PlayerId);
                    player?.RecvMessage(pkt.ChatMsg, pkt.ChatType);
                }
                else
                {
                    S2Manager.Instance.UiChat.PushMessage(
                        "System",
                        pkt.ChatMsg,
                        pkt.ChatType,
                        true
                    );
                }
                break;
        }
    }

    public static void S2CPlayerSpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CSpawn pkt)
            return;
        Debug.Log($"S2CPlayerSpawn 패킷 무사 도착 : {pkt}");

        // @@@ 섹터 매니저 awake까지 기다려야해.... @@@
        GameManager.Instance.WaitForSceneAwake(pkt);
    }

    public static void S2CDespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CDespawn pkt)
            return;
        Debug.Log($"S2CDespawn 패킷 무사 도착 : {pkt}");

        switch (pkt.CurrentSector)
        {
            case 100:
                foreach (int playerId in pkt.PlayerIds)
                {
                    TownManager.Instance.DespawnPlayer(playerId);
                }
                break;
            case 2:
                foreach (int playerId in pkt.PlayerIds)
                {
                    ASectorManager.Instance.ReleasePlayer(playerId);
                }
                break;
            case 101:
                foreach (int playerId in pkt.PlayerIds)
                {
                    S1Manager.Instance.DespawnPlayer(playerId);
                }
                break;
            case 102:
                foreach (int playerId in pkt.PlayerIds)
                {
                    S2Manager.Instance.DespawnPlayer(playerId);
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
        // Debug.Log($"S2CPlayerLocation 패킷 무사 도착 : {pkt}");

        TransformInfo transform = pkt.Transform;
        Vector3 position = new Vector3(transform.PosX, transform.PosY, transform.PosZ);
        Quaternion rotation = Quaternion.Euler(0, transform.Rot, 0);
        bool isValidTransform = pkt.IsValidTransform;

        switch (pkt.CurrentSector)
        {
            case 100:
                var townPlayer = TownManager.Instance.GetPlayer(pkt.PlayerId);
                townPlayer?.Move(position, rotation);
                break;
            case 2:
                var sectorPlayer = ASectorManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
                sectorPlayer?.Move(position, rotation);
                break;
            case 101:
                var s1Player = S1Manager.Instance.GetPlayer(pkt.PlayerId);
                s1Player.Move(position, rotation);
                break;
            case 102:
                var s2Player = S2Manager.Instance.GetPlayer(pkt.PlayerId);
                s2Player.Move(position, rotation);
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
    public static void S2CCollisionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CCollision pkt)
            return;
        //Debug.Log($"S2CPlayerCollision 패킷 무사 도착 : {pkt}");
        //1: 플레이어
        //2: 몬스터
        S2CCollision collisionPushInfo = (S2CCollision)packet;
        var info = collisionPushInfo.CollisionPushInfo;

        if (info.HasCollision == false)
        {
            Debug.LogError("해당 충돌은 거짓 판명이 나왔다.");
            return;
        }
        switch (info.MyType)
        {
            case 1:
                var players = GameObject.FindObjectsOfType<Player>(); // 모든 플레이어를 배열로 가져옴
                var player = Array.Find(players, x => x.PlayerId == info.MyId); // 특정 ID를 가진 플레이어 찾기
                player.SetCollision(info);
                break;
            case 2:
                var monster = MonsterManager.Instance.GetMonster(info.MyId); // 몬스터 찾기
                monster?.SetCollision(info);
                break;
        }
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

        Vector3 position = new Vector3(
            monsterPosition.PosX,
            monsterPosition.PosY,
            monsterPosition.PosZ
        );
        MonsterManager.Instance.SendPositionPacket(monsterId, position);

        //Debug.Log($"S2CMonsterLocation 패킷 무사 도착 : {pkt}");
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
        if (packet is not S2CResourcesList pkt)
            return;
        Debug.Log($"S2CResourceList 패킷 무사 도착 : {pkt}");
        ResourcesManager.Instance.ResourcesInit(pkt);
    }

    public static void S2CUpdateDurabilityHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CUpdateDurability pkt)
            return;
        Debug.Log($"S2CUpdateDurability 패킷 무사 도착 : {pkt}");
        ResourcesManager.Instance.ResourcesUpdateDurability(pkt);
    }

    public static void S2CGatheringStartHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CGatheringStart pkt)
            return;
        Debug.Log($"S2CGatheringStart 패킷 무사 도착 : {pkt}");
        ResourcesManager.Instance.ResourcesGatheringStart(pkt);
    }

    public static void S2CGatheringSkillCheckHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CGatheringSkillCheck pkt)
            return;
        Debug.Log($"S2CGatheringSkillCheck 패킷 무사 도착 : {pkt}");
        ResourcesManager.Instance.ResourcesGatheringSkillCheck(pkt);
    }

    public static void S2CGatheringDoneHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CGatheringDone pkt)
            return;
        Debug.Log($"S2CGatheringDone 패킷 무사 도착 : {pkt}");
    }

    public static void S2CRecallHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CRecall pkt)
            return;
        Debug.Log($"S2CRecall 패킷 무사 도착 : {pkt}");

        switch (pkt.CurrentSector)
        {
            case 100:
                var townPlayer = TownManager.Instance.GetPlayer(pkt.PlayerId);
                townPlayer.CastRecall(pkt.RecallTimer);
                break;
            case 101:
                var s1Player = S1Manager.Instance.GetPlayer(pkt.PlayerId);
                s1Player.CastRecall(pkt.RecallTimer);
                break;
            case 102:
                var s2Player = S2Manager.Instance.GetPlayer(pkt.PlayerId);
                s2Player.CastRecall(pkt.RecallTimer);
                break;
        }
    }

    public static void S2CThrowGrenadeHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CThrowGrenade pkt)
            return;
        Debug.Log($"S2CThrowGrenade 패킷 무사 도착 : {pkt}");

        switch (pkt.CurrentSector)
        {
            case 100:
                var townPlayer = TownManager.Instance.GetPlayer(pkt.PlayerId);
                break;
            case 101:
                var s1Player = S1Manager.Instance.GetPlayer(pkt.PlayerId);
                break;
            case 102:
                var s2Player = S2Manager.Instance.GetPlayer(pkt.PlayerId);
                break;
        }
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
        Debug.Log($"S2CSectorMove 패킷 무사 도착 : {pkt}");

         //Scene currentScene = SceneManager.GetActiveScene();

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

        TownManager.Instance.MyPlayer.SetExp(pkt.UpdatedExp);
    }

    public static void S2CLevelUpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CLevelUp pkt)
            return;
        Debug.Log($"S2CLevelUp 패킷 무사 도착 : {pkt}");

        if (
            TownManager.Instance.MyPlayer != null
            && pkt.PlayerId == TownManager.Instance.MyPlayer.PlayerId
        )
        {
            Debug.Log(
                $"1. 패킷플레이어ID:{pkt.PlayerId}, 내플레이어ID:{TownManager.Instance.MyPlayer.PlayerId}"
            );
            TownManager.Instance.MyPlayer.LevelUp(
                pkt.UpdatedLevel,
                pkt.NewTargetExp,
                pkt.UpdatedExp,
                pkt.AbilityPoint
            );
        }
        else
        {
            Debug.Log(
                $"2. 패킷플레이어ID:{pkt.PlayerId}, 내플레이어ID:{TownManager.Instance.MyPlayer.PlayerId}"
            );
            TownManager.Instance.GetPlayer(pkt.PlayerId).LevelUpOther();
        }
    }

    public static void S2CInvestPointHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CInvestPoint pkt)
            return;
        Debug.Log($"S2CInvestPoint 패킷 무사 도착 : {pkt}");

        TownManager.Instance.MyPlayer.InvestPoint(pkt.StatInfo);
    }
    #endregion
}
