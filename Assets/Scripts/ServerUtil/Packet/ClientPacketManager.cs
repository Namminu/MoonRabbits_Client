using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance
    {
        get { return _instance; }
    }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv =
        new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler =
        new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.S2CRegister, MakePacket<S2CRegister>);
        _handler.Add((ushort)MsgId.S2CRegister, PacketHandler.S2CRegisterHandler);
        _onRecv.Add((ushort)MsgId.S2CLogin, MakePacket<S2CLogin>);
        _handler.Add((ushort)MsgId.S2CLogin, PacketHandler.S2CLoginHandler);
        _onRecv.Add((ushort)MsgId.S2CCreateCharacter, MakePacket<S2CCreateCharacter>);
        _handler.Add((ushort)MsgId.S2CCreateCharacter, PacketHandler.S2CCreateCharacterHandler);
        _onRecv.Add((ushort)MsgId.S2CTownEnter, MakePacket<S2CTownEnter>);
        _handler.Add((ushort)MsgId.S2CTownEnter, PacketHandler.S2CTownEnterHandler);
        _onRecv.Add((ushort)MsgId.S2CTownLeave, MakePacket<S2CTownLeave>);
        _handler.Add((ushort)MsgId.S2CTownLeave, PacketHandler.S2CTownLeaveHandler);
        _onRecv.Add((ushort)MsgId.S2CAnimation, MakePacket<S2CAnimation>);
        _handler.Add((ushort)MsgId.S2CAnimation, PacketHandler.S2CAnimationHandler);
        _onRecv.Add((ushort)MsgId.S2CChat, MakePacket<S2CChat>);
        _handler.Add((ushort)MsgId.S2CChat, PacketHandler.S2CChatHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerSpawn, MakePacket<S2CPlayerSpawn>);
        _handler.Add((ushort)MsgId.S2CPlayerSpawn, PacketHandler.S2CPlayerSpawnHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerDespawn, MakePacket<S2CPlayerDespawn>);
        _handler.Add((ushort)MsgId.S2CPlayerDespawn, PacketHandler.S2CPlayerDespawnHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerMove, MakePacket<S2CPlayerMove>);
        _handler.Add((ushort)MsgId.S2CPlayerMove, PacketHandler.S2CPlayerMoveHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerLocation, MakePacket<S2CPlayerLocation>);
        _handler.Add((ushort)MsgId.S2CPlayerLocation, PacketHandler.S2CPlayerLocationHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerCollision, MakePacket<S2CPlayerCollision>);
        _handler.Add((ushort)MsgId.S2CPlayerCollision, PacketHandler.S2CPlayerCollisionHandler);
        _onRecv.Add((ushort)MsgId.S2CMonsterCollision, MakePacket<S2CMonsterCollision>);
        _handler.Add((ushort)MsgId.S2CMonsterCollision, PacketHandler.S2CMonsterCollisionHandler);
        _onRecv.Add((ushort)MsgId.S2CCreateParty, MakePacket<S2CCreateParty>);
        _handler.Add((ushort)MsgId.S2CCreateParty, PacketHandler.S2CCreatePartyHandler);
        _onRecv.Add((ushort)MsgId.S2CInviteParty, MakePacket<S2CInviteParty>);
        _handler.Add((ushort)MsgId.S2CInviteParty, PacketHandler.S2CInvitePartyHandler);
        _onRecv.Add((ushort)MsgId.S2CJoinParty, MakePacket<S2CJoinParty>);
        _handler.Add((ushort)MsgId.S2CJoinParty, PacketHandler.S2CJoinPartyHandler);
        _onRecv.Add((ushort)MsgId.S2CLeaveParty, MakePacket<S2CLeaveParty>);
        _handler.Add((ushort)MsgId.S2CLeaveParty, PacketHandler.S2CLeavePartyHandler);
        _onRecv.Add((ushort)MsgId.S2CSetPartyLeader, MakePacket<S2CSetPartyLeader>);
        _handler.Add((ushort)MsgId.S2CSetPartyLeader, PacketHandler.S2CSetPartyLeaderHandler);
        _onRecv.Add((ushort)MsgId.S2CBuff, MakePacket<S2CBuff>);
        _handler.Add((ushort)MsgId.S2CBuff, PacketHandler.S2CBuffHandler);
        _onRecv.Add((ushort)MsgId.S2CDungeonEnter, MakePacket<S2CDungeonEnter>);
        _handler.Add((ushort)MsgId.S2CDungeonEnter, PacketHandler.S2CDungeonEnterHandler);
        _onRecv.Add((ushort)MsgId.S2CDungeonLeave, MakePacket<S2CDungeonLeave>);
        _handler.Add((ushort)MsgId.S2CDungeonLeave, PacketHandler.S2CDungeonLeaveHandler);
        _onRecv.Add((ushort)MsgId.S2CAttack, MakePacket<S2CAttack>);
        _handler.Add((ushort)MsgId.S2CAttack, PacketHandler.S2CAttackHandler);
        _onRecv.Add((ushort)MsgId.S2CHit, MakePacket<S2CHit>);
        _handler.Add((ushort)MsgId.S2CHit, PacketHandler.S2CHitHandler);
        _onRecv.Add((ushort)MsgId.S2CDie, MakePacket<S2CDie>);
        _handler.Add((ushort)MsgId.S2CDie, PacketHandler.S2CDieHandler);
        _onRecv.Add((ushort)MsgId.S2CMonsterLocation, MakePacket<S2CMonsterLocation>);
        _handler.Add((ushort)MsgId.S2CMonsterLocation, PacketHandler.S2CMonsterLocationHandler);

        Debug.Log("핸들러 등록 완료");
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        Debug.Log(
            $"PacketManager.OnRecvPacket 호출: {BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count)}"
        );

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

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id)
        where T : IMessage, new()
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
