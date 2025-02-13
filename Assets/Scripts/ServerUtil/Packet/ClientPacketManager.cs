using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using UnityEngine;

class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.SEnter, MakePacket<S_Enter>);
        _handler.Add((ushort)MsgId.SEnter, PacketHandler.S_EnterHandler);
        _onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
        _handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);
        // _onRecv.Add((ushort)MsgId.SLeave, MakePacket<S_Leave>);
        _handler.Add((ushort)MsgId.SLeave, PacketHandler.S_LeaveHandler);
        _onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
        _handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);
        _onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
        // _handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);
        _onRecv.Add((ushort)MsgId.SAnimation, MakePacket<S_Animation>);
        _handler.Add((ushort)MsgId.SAnimation, PacketHandler.S_AnimationHandler);
        // _onRecv.Add((ushort)MsgId.SChangeCostume, MakePacket<S_ChangeCostume>);
        _handler.Add((ushort)MsgId.SChangeCostume, PacketHandler.S_ChangeCostumeHandler);
        _onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
        _handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);
        _onRecv.Add((ushort)MsgId.SEnterDungeon, MakePacket<S_EnterDungeon>);
        _handler.Add((ushort)MsgId.SEnterDungeon, PacketHandler.S_EnterDungeonHandler);
        _onRecv.Add((ushort)MsgId.SLeaveDungeon, MakePacket<S_LeaveDungeon>);
        _handler.Add((ushort)MsgId.SLeaveDungeon, PacketHandler.S_LeaveDungeonHandler);
        _onRecv.Add((ushort)MsgId.SScreenText, MakePacket<S_ScreenText>);
        _handler.Add((ushort)MsgId.SScreenText, PacketHandler.S_ScreenTextHandler);
        _onRecv.Add((ushort)MsgId.SScreenDone, MakePacket<S_ScreenDone>);
        _handler.Add((ushort)MsgId.SScreenDone, PacketHandler.S_ScreenDoneHandler);
        _onRecv.Add((ushort)MsgId.SBattleLog, MakePacket<S_BattleLog>);
        _handler.Add((ushort)MsgId.SBattleLog, PacketHandler.S_BattleLogHandler);
        _onRecv.Add((ushort)MsgId.SSetPlayerHp, MakePacket<S_SetPlayerHp>);
        _handler.Add((ushort)MsgId.SSetPlayerHp, PacketHandler.S_SetPlayerHpHandler);
        _onRecv.Add((ushort)MsgId.SSetPlayerMp, MakePacket<S_SetPlayerMp>);
        _handler.Add((ushort)MsgId.SSetPlayerMp, PacketHandler.S_SetPlayerMpHandler);
        _onRecv.Add((ushort)MsgId.SSetMonsterHp, MakePacket<S_SetMonsterHp>);
        _handler.Add((ushort)MsgId.SSetMonsterHp, PacketHandler.S_SetMonsterHpHandler);
        _onRecv.Add((ushort)MsgId.SPlayerAction, MakePacket<S_PlayerAction>);
        _handler.Add((ushort)MsgId.SPlayerAction, PacketHandler.S_PlayerActionHandler);
        _onRecv.Add((ushort)MsgId.SMonsterAction, MakePacket<S_MonsterAction>);
        _handler.Add((ushort)MsgId.SMonsterAction, PacketHandler.S_MonsterActionHandler);
        _onRecv.Add((ushort)MsgId.SLocation, MakePacket<S_Location>);
        _handler.Add((ushort)MsgId.SLocation, PacketHandler.S_LocationHandler);


        Debug.Log("핸들러 등록 완료");
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        Debug.Log($"PacketManager.OnRecvPacket 호출: {BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count)}");

        ushort count = 0;

        // 크기를 4바이트로 읽음
        int size = BitConverter.ToInt32(buffer.Array, buffer.Offset);
        if (buffer.Count < size)
        {
            Debug.LogError("패킷 크기가 부족합니다. 데이터가 불완전합니다.");
            return;
        }

        Debug.Log($"패킷 크기: {size}");
        count += 4;

        // 아이디를 1바이트로 읽음
        byte id = buffer.Array[buffer.Offset + count];
        if (!_onRecv.ContainsKey(id))
        {
            Debug.LogError($"알 수 없는 패킷 ID: {id}");
            return;
        }

        Debug.Log($"패킷 ID: {id}");
        count += 1;

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;



        if (_onRecv.TryGetValue(id, out action))
        {
            Debug.Log($"패킷 핸들러 실행: {id}");
            action.Invoke(session, buffer, id);
        }
        else
        {
            Debug.LogError($"등록되지 않은 패킷 ID: {id}");
        }
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        try
        {
            T pkt = new T();
            pkt.MergeFrom(buffer.Array, buffer.Offset + 5, buffer.Count - 5);
            Debug.Log($"패킷 역직렬화 성공: {typeof(T).Name}");

            if (CustomHandler != null)
            {
                CustomHandler.Invoke(session, pkt, id);
            }
            else
            {
                Action<PacketSession, IMessage> action = null;
                if (_handler.TryGetValue(id, out action))
                {
                    Debug.Log($"핸들러 실행: {id}");
                    action.Invoke(session, pkt);
                }
                else
                {
                    Debug.LogError($"핸들러가 등록되지 않음: {id}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"패킷 역직렬화 실패: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
    {
        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            return action;
        return null;
    }
}