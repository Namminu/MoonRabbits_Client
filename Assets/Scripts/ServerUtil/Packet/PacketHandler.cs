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

        List<Google.Protobuf.Protocol.OwnedCharacters> charsInfo = pkt.OwnedCharacters.ToList();
        Debug.Log("charsInfo : " + charsInfo);
        EventManager.Trigger("CheckHasChar", charsInfo);
    }

    public static void S2CCreateCharacterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CCreateCharacter pkt)
            return;
        Debug.Log($"S2CCreateCharacter 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region Town
    public static void S2CTownEnterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CTownEnter pkt)
            return;
        Debug.Log($"S2CTownEnter 패킷 무사 도착 : {pkt}");

        TownManager.Instance.Spawn(pkt.Player);
    }

    public static void S2CTownLeaveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CTownLeave pkt)
            return;
        Debug.Log($"S2CTownLeave 패킷 무사 도착 : {pkt}");
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

    #region Common - Chat & Spawn
    public static void S2CChatHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CChat pkt)
            return;
        Debug.Log($"S2CChat 패킷 무사 도착 : {pkt}");

        if (pkt.PlayerId > 0)
        {
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
            // 여기에서 localPlayer 설정이 작동하지 않는 버그가 발생 하여 해당 예외처리를 추가함.
            if (
                TownManager.Instance.MyPlayer != null
                && playerInfo.PlayerId == TownManager.Instance.MyPlayer.PlayerId
            )
                continue;

            Vector3 spawnPosition = new Vector3(
                playerInfo.Transform.PosX,
                playerInfo.Transform.PosY,
                playerInfo.Transform.PosZ
            );
            var player = TownManager.Instance.CreatePlayer(playerInfo, spawnPosition);
            player.SetIsMine(false);
        }
    }

    public static void S2CPlayerDespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CPlayerDespawn pkt)
            return;
        Debug.Log($"S2CPlayerDespawn 패킷 무사 도착 : {pkt}");

        foreach (int playerId in pkt.PlayerIds)
        {
            TownManager.Instance.ReleasePlayer(playerId);
        }
    }
    #endregion

    #region Common - Move(Player)
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

        var player = TownManager.Instance.GetPlayerAvatarById(pkt.PlayerId);
        player?.Move(position, rotation);
    }
    #endregion

    #region Common - Collision
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

    #region Party
    public static void S2CCreatePartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CCreateParty pkt)
            return;
        Debug.Log($"S2CCreateParty 패킷 무사 도착 : {pkt}");
    }

    public static void S2CInvitePartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CInviteParty pkt)
            return;
        Debug.Log($"S2CInviteParty 패킷 무사 도착 : {pkt}");
    }

    public static void S2CJoinPartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CJoinParty pkt)
            return;
        Debug.Log($"S2CJoinParty 패킷 무사 도착 : {pkt}");
    }

    public static void S2CLeavePartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CLeaveParty pkt)
            return;
        Debug.Log($"S2CLeaveParty 패킷 무사 도착 : {pkt}");
    }

    public static void S2CSetPartyLeaderHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CSetPartyLeader pkt)
            return;
        Debug.Log($"S2CSetPartyLeader 패킷 무사 도착 : {pkt}");
    }
    public static void S2CKickOutMemberHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CKickOutMember pkt)
            return;
        Debug.Log($"S2CKickOutMember 패킷 무사 도착 : {pkt}");
    }
    public static void S2CDisbandPartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CDisbandParty pkt)
            return;
        Debug.Log($"S2CDisbandParty 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region Sector
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
    #endregion

    #region Battle
    public static void S2CAttackHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CAttack pkt)
            return;
        Debug.Log($"S2CAttack 패킷 무사 도착 : {pkt}");
    }

    public static void S2CHitHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CHit pkt)
            return;
        Debug.Log($"S2CHit 패킷 무사 도착 : {pkt}");
    }

    public static void S2CDieHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CDie pkt)
            return;
        Debug.Log($"S2CDie 패킷 무사 도착 : {pkt}");
    }
    #endregion

    #region Dungeon - Move(Monster)
    public static void S2CMonsterLocationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S2CMonsterLocation pkt)
            return;
        Debug.Log($"S2CMonsterLocation 패킷 무사 도착 : {pkt}");
    }
    #endregion

    /* !!! 패킷 수정으로 보류된 핸들러 목록 !!! */
    // public static void S_ScreenTextHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_ScreenText pkt)
    //         return;

    //     BattleManager.Instance.UiScreen?.Display(pkt.ScreenText);
    // }

    // public static void S_ScreenDoneHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_ScreenDone pkt)
    //         return;

    //     var uiScreen = BattleManager.Instance.UiScreen;
    //     if (uiScreen != null)
    //     {
    //         uiScreen.gameObject.SetActive(false);
    //     }
    // }

    // public static void S_BattleLogHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_BattleLog pkt)
    //         return;

    //     BattleManager.Instance.UiBattleLog?.Initialize(pkt.BattleLog);
    // }

    // public static void S_SetPlayerHpHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_SetPlayerHp pkt)
    //         return;

    //     BattleManager.Instance.UiPlayerInformation?.UpdateHP(pkt.Hp);
    // }

    // public static void S_SetPlayerMpHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_SetPlayerMp pkt)
    //         return;

    //     BattleManager.Instance.UiPlayerInformation?.UpdateMP(pkt.Mp);
    // }

    // public static void S_SetMonsterHpHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_SetMonsterHp pkt)
    //         return;

    //     BattleManager.Instance.UpdateMonsterHp(pkt.MonsterIdx, pkt.Hp);
    // }

    // public static void S_PlayerActionHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_PlayerAction pkt)
    //         return;

    //     Monster monster = BattleManager.Instance.GetMonster(pkt.TargetMonsterIdx);
    //     monster?.Hit();

    //     BattleManager.Instance.TriggerPlayerAnimation(pkt.ActionSet.AnimCode);
    //     EffectManager.Instance.SetEffectToMonster(pkt.TargetMonsterIdx, pkt.ActionSet.EffectCode);
    // }

    // public static void S_MonsterActionHandler(PacketSession session, IMessage packet)
    // {
    //     if (packet is not S_MonsterAction pkt)
    //         return;

    //     Monster monster = BattleManager.Instance.GetMonster(pkt.ActionMonsterIdx);
    //     monster?.SetAnim(pkt.ActionSet.AnimCode);

    //     BattleManager.Instance.TriggerPlayerHitAnimation();
    //     EffectManager.Instance.SetEffectToPlayer(pkt.ActionSet.EffectCode);
    // }
}
