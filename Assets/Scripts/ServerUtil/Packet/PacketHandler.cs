using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
    #region Town

    public static void S_EnterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Enter enterPacket) return;
        TownManager.Instance.Spawn(enterPacket.Player);
    }

    public static void S_LeaveHandler(PacketSession session, IMessage packet) { }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Spawn spawnPacket) return;

        foreach (var playerInfo in spawnPacket.Players)
        {
            // 여기에서 localPlayer 설정이 작동하지 않는 버그가 발생 하여 해당 예외처리를 추가함.
            if (TownManager.Instance.MyPlayer != null &&
            playerInfo.PlayerId == TownManager.Instance.MyPlayer.PlayerId)
                continue;

            Vector3 spawnPosition = new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ);
            var player = TownManager.Instance.CreatePlayer(playerInfo, spawnPosition);
            player.SetIsMine(false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Despawn despawnPacket) return;

        foreach (int playerId in despawnPacket.PlayerIds)
        {
            TownManager.Instance.ReleasePlayer(playerId);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Move movePacket) return;

        TransformInfo transform = movePacket.Transform;
        Vector3 position = new Vector3(transform.PosX, transform.PosY, transform.PosZ);
        Quaternion rotation = Quaternion.Euler(0, transform.Rot, 0);

        var player = TownManager.Instance.GetPlayerAvatarById(movePacket.PlayerId);
        player?.Move(position, rotation);
    }

    public static void S_AnimationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Animation animationPacket) return;

        var player = TownManager.Instance.GetPlayerAvatarById(animationPacket.PlayerId);
        player?.PlayAnimation(animationPacket.AnimCode);
    }

    public static void S_ChangeCostumeHandler(PacketSession session, IMessage packet) { }

    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Chat chatPacket) return;

        if (chatPacket.PlayerId > 0)
        {
            var player = TownManager.Instance.GetPlayerAvatarById(chatPacket.PlayerId);
            player?.RecvMessage(chatPacket.ChatMsg);
        }
        else
        {
            TownManager.Instance.UiChat.PushMessage("System", chatPacket.ChatMsg, true);
        }
    }

    #endregion

    #region Battle

    public static void S_EnterDungeonHandler(PacketSession session, IMessage packet)
    {

        if (packet is not S_EnterDungeon pkt) return;

        Scene currentScene = SceneManager.GetActiveScene();


        if (currentScene.name == GameManager.BattleScene)
        {
            BattleManager.Instance.ConfigureGame(pkt);
        }
        else
        {
            GameManager.Instance.Pkt = pkt;
            SceneManager.LoadScene(GameManager.BattleScene);

        }
    }

    public static void S_LeaveDungeonHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_LeaveDungeon pkt) return;

        SceneManager.LoadScene(GameManager.TownScene);
    }

    public static void S_ScreenTextHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ScreenText pkt) return;

        BattleManager.Instance.UiScreen?.Display(pkt.ScreenText);
    }

    public static void S_ScreenDoneHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ScreenDone pkt) return;

        var uiScreen = BattleManager.Instance.UiScreen;
        if (uiScreen != null)
        {
            uiScreen.gameObject.SetActive(false);
        }
    }

    public static void S_BattleLogHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BattleLog pkt) return;

        BattleManager.Instance.UiBattleLog?.Initialize(pkt.BattleLog);
    }

    public static void S_SetPlayerHpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetPlayerHp pkt) return;

        BattleManager.Instance.UiPlayerInformation?.UpdateHP(pkt.Hp);
    }

    public static void S_SetPlayerMpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetPlayerMp pkt) return;

        BattleManager.Instance.UiPlayerInformation?.UpdateMP(pkt.Mp);
    }

    public static void S_SetMonsterHpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetMonsterHp pkt) return;

        BattleManager.Instance.UpdateMonsterHp(pkt.MonsterIdx, pkt.Hp);
    }

    public static void S_PlayerActionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_PlayerAction pkt) return;

        Monster monster = BattleManager.Instance.GetMonster(pkt.TargetMonsterIdx);
        monster?.Hit();

        BattleManager.Instance.TriggerPlayerAnimation(pkt.ActionSet.AnimCode);
        EffectManager.Instance.SetEffectToMonster(pkt.TargetMonsterIdx, pkt.ActionSet.EffectCode);
    }

    public static void S_MonsterActionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MonsterAction pkt) return;

        Monster monster = BattleManager.Instance.GetMonster(pkt.ActionMonsterIdx);
        monster?.SetAnim(pkt.ActionSet.AnimCode);

        BattleManager.Instance.TriggerPlayerHitAnimation();
        EffectManager.Instance.SetEffectToPlayer(pkt.ActionSet.EffectCode);
    }

    #endregion

    public static void S_RegisterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Register pkt) return;
        Debug.Log($"S_Register 패킷 무사히 도착{pkt}");
        // UILogIn.DisplayMessage(pkt.Msg);
    }

    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Login pkt) return;
        Debug.Log($"S_Login 패킷 무사히 도착 \n{pkt.IsSuccess}\n{pkt.Msg}\n{pkt.OwnedCharacters}");
        TownManager.Instance.GameStart(pkt.OwnedCharacters[0].Nickname,pkt.OwnedCharacters[0].Class-1001,"127.0.0.1");
    }
}