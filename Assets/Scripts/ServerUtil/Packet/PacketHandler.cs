using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

        SoundManager.Instance.Play(0, Define.Sound.Bgm);

        if (!pkt.IsSuccess)
        {
            EventManager.Trigger("DisplayMessage", pkt.Msg);
            return;
        }

        List<OwnedCharacter> charsInfo = pkt.OwnedCharacters.ToList();
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

        string targetSceneName = GameManager.Instance.SceneName[pkt.TargetSector];

        if (targetSceneName != SceneManager.GetActiveScene().name)
            SceneManager.LoadScene(targetSceneName);

        GameManager.Instance.EnterAfterSceneAwake(pkt.TargetSector, pkt.Player);
    }

    public static void S2CMoveSectorHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CMoveSector pkt)
            return;
        Debug.Log($"S2CMoveSector 패킷 무사 도착 : {pkt}");
    }

    public static void S2CEmoteHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CEmote pkt)
            return;
        Debug.Log($"S2CEmote 패킷 무사 도착 : {pkt}");

        var player = GameManager.Instance.GetPlayer(pkt.PlayerId);
        if (player != null)
            player.Emote(pkt.AnimCode);
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
            var player = GameManager.Instance.GetPlayer(pkt.PlayerId);
            if (player != null)
                player.RecvMessage(pkt.ChatMsg, pkt.ChatType);
        }
        else
        {
            GameManager.Instance.SManager.UiChat.PushMessage(
                "System",
                pkt.ChatMsg,
                pkt.ChatType,
                true
            );
        }
    }

    public static void S2CPlayerSpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CSpawn pkt)
            return;
        Debug.Log($"S2CPlayerSpawn 패킷 무사 도착 : {pkt}");

        GameManager.Instance.SpawnAfterSceneAwake(pkt);
    }

    public static void S2CDespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CDespawn pkt)
            return;
        Debug.Log($"S2CDespawn 패킷 무사 도착 : {pkt}");

        foreach (int playerId in pkt.PlayerIds)
        {
            GameManager.Instance.SManager.DespawnPlayer(playerId);
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

        Vector3 position = new(pkt.Transform.PosX, pkt.Transform.PosY, pkt.Transform.PosZ);
        Quaternion rotation = Quaternion.Euler(0, pkt.Transform.Rot, 0);
        bool isValidTransform = pkt.IsValidTransform; // 이거 안 쓰나여?

        var player = GameManager.Instance.GetPlayer(pkt.PlayerId);
        if (player != null)
            player.Move(position, rotation);
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
    #endregion

    #region PlayerAction
    public static void S2CRecallHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CRecall pkt)
            return;
        Debug.Log($"S2CRecall 패킷 무사 도착 : {pkt}");

        var player = GameManager.Instance.GetPlayer(pkt.PlayerId);
        if (player != null)
            player.CastRecall(pkt.RecallTimer);
    }

    public static void S2CThrowGrenadeHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CThrowGrenade pkt)
            return;
        Debug.Log($"S2CThrowGrenade 패킷 무사 도착 : {pkt}");

        var player = GameManager.Instance.GetPlayer(pkt.PlayerId);
        if (player != null)
            player.CastGrenade(pkt.Velocity, pkt.CoolTime);
    }

    public static void S2CTrapsHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CTraps pkt)
            return;
        Debug.Log($"S2CTraps 패킷 무사 도착 : {pkt}");
    }

    public static void S2CSetTrapHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CSetTrap pkt)
            return;
        Debug.Log($"S2CSetTrap 패킷 무사 도착 : {pkt}");

        var player = GameManager.Instance.GetPlayer(pkt.TrapInfo.CasterId);
        if (player != null)
            player.CastTrap(pkt.TrapInfo.Pos, pkt.CoolTime);
    }

    public static void S2CRemoveTrapHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CRemoveTrap pkt)
            return;
        Debug.Log($"S2CRemoveTrap  패킷 무사 도착 : {pkt}");

        SkillObj[] traps = GameObject.FindObjectsOfType<SkillObj>();

        foreach (SkillObj obj in traps)
        {
            if (obj.type != SkillObj.SkillType.trap)
            {
                continue;
            }
            else
            {
                foreach (TrapInfo trap in pkt.TrapInfos)
                {
                    if (
                        obj.transform.position.x - trap.Pos.X / 10f <= 0.1
                        && obj.transform.position.z - trap.Pos.Z / 10f <= 0.1
                    )
                    {
                        obj.RemoveThis();
                    }
                }
            }
        }
    }

    public static void S2CStunHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CStun pkt)
            return;
        Debug.Log($"S2CStun 패킷 무사 도착 : {pkt}");
        // 몬스터 경우 처리도 추가해야함
        foreach (int playerId in pkt.PlayerIds)
        {
            var player = GameManager.Instance.GetPlayer(playerId);
            if (player != null)
                player.Stun(pkt.StunTimer);
        }
    }

    public static void S2CEquipChangeHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CEquipChange pkt)
            return;
        Debug.Log($"S2CEquipChange 패킷 무사 도착 : {pkt}");

        var player = GameManager.Instance.GetPlayer(pkt.PlayerId);
        if (player != null)
            player.ChangeEquip(pkt.NextEquip);
    }
    #endregion

    #region LevelUp
    public static void S2CAddExpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CAddExp pkt)
            return;
        Debug.Log($"S2CAddExp 패킷 무사 도착 : {pkt}");

        if (GameManager.Instance.MPlayer != null)
        {
            GameManager.Instance.MPlayer.SetExp(pkt.UpdatedExp);
        }

        // if (TownManager.Instance.MyPlayer != null)
        //     TownManager.Instance.MyPlayer.SetExp(pkt.UpdatedExp);
        // else if (S1Manager.Instance.MyPlayer != null)
        //     S1Manager.Instance.MyPlayer.SetExp(pkt.UpdatedExp);
        // else if (S2Manager.Instance.MyPlayer != null)
        //     S2Manager.Instance.MyPlayer.SetExp(pkt.UpdatedExp);
        // else if (S3Manager.Instance.MyPlayer != null)
        //     S3Manager.Instance.MyPlayer.SetExp(pkt.UpdatedExp);
        // Debug.Log("MyPlayer is in the S2Manager");
    }

    public static void S2CLevelUpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CLevelUp pkt)
            return;
        Debug.Log($"S2CLevelUp 패킷 무사 도착 : {pkt}");

        if (GameManager.Instance.MPlayer != null)
        {
            if (GameManager.Instance.MPlayer.PlayerId == pkt.PlayerId)
            {
                GameManager.Instance.MPlayer.LevelUp(
                    pkt.UpdatedLevel,
                    pkt.NewTargetExp,
                    pkt.UpdatedExp,
                    pkt.AbilityPoint
                );
            }
            else if (GameManager.Instance.MPlayer.PlayerId != pkt.PlayerId)
            {
                GameManager.Instance.GetPlayer(pkt.PlayerId).ActiveLevelUpEffect();
            }
        }
        // if (TownManager.Instance.MyPlayer != null)
        // {
        //     if (pkt.PlayerId == TownManager.Instance.MyPlayer.PlayerId)
        //     {
        //         TownManager.Instance.MyPlayer.LevelUp(
        //             pkt.UpdatedLevel,
        //             pkt.NewTargetExp,
        //             pkt.UpdatedExp,
        //             pkt.AbilityPoint
        //         );
        //     }
        //     else
        //     {
        //         TownManager.Instance.GetPlayer(pkt.PlayerId).LevelUpOther();
        //     }
        // }
        // else if (S1Manager.Instance.MyPlayer != null)
        // {
        //     if (pkt.PlayerId == S1Manager.Instance.MyPlayer.PlayerId)
        //     {
        //         S1Manager.Instance.MyPlayer.LevelUp(
        //             pkt.UpdatedLevel,
        //             pkt.NewTargetExp,
        //             pkt.UpdatedExp,
        //             pkt.AbilityPoint
        //         );
        //     }
        //     else
        //     {
        //         S1Manager.Instance.GetPlayer(pkt.PlayerId).LevelUpOther();
        //     }
        // }
        // else if (S2Manager.Instance.MyPlayer != null)
        // {
        //     if (pkt.PlayerId == S2Manager.Instance.MyPlayer.PlayerId)
        //     {
        //         S2Manager.Instance.MyPlayer.LevelUp(
        //             pkt.UpdatedLevel,
        //             pkt.NewTargetExp,
        //             pkt.UpdatedExp,
        //             pkt.AbilityPoint
        //         );
        //     }
        //     else
        //     {
        //         S2Manager.Instance.GetPlayer(pkt.PlayerId).LevelUpOther();
        //     }
        // }
    }

    public static void S2CInvestPointHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CInvestPoint pkt)
            return;
        Debug.Log($"S2CInvestPoint 패킷 무사 도착 : {pkt}");

        if (GameManager.Instance.MPlayer != null)
        {
            GameManager.Instance.MPlayer.InvestPoint(pkt.StatInfo);
        }

        // if (TownManager.Instance.MyPlayer != null)
        //     TownManager.Instance.MyPlayer.InvestPoint(pkt.StatInfo);
        // else if (S1Manager.Instance.MyPlayer != null)
        //     S1Manager.Instance.MyPlayer.InvestPoint(pkt.StatInfo);
        // else if (S2Manager.Instance.MyPlayer != null)
        //     S2Manager.Instance.MyPlayer.InvestPoint(pkt.StatInfo);
    }
    #endregion

    #region Item & Inventory

    public static void S2CInventoryUpdate(PacketSession session, IMessage packet)
    {
        if (packet is not S2CInventoryUpdate pkt)
            return;
        Debug.Log($"S2CInventoryUpdate 패킷 무사 도착 : {pkt}");
        InventoryManager.instance.UpdateInventoryData(pkt);
    }

    #endregion
}
