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
        _onRecv.Add((ushort)MsgId.S2CEnter, MakePacket<S2CEnter>);
        _handler.Add((ushort)MsgId.S2CEnter, PacketHandler.S2CEnterHandler);
        _onRecv.Add((ushort)MsgId.S2CMoveSector, MakePacket<S2CMoveSector>);
        _handler.Add((ushort)MsgId.S2CMoveSector, PacketHandler.S2CMoveSectorHandler);
        _onRecv.Add((ushort)MsgId.S2CEmote, MakePacket<S2CEmote>);
        _handler.Add((ushort)MsgId.S2CEmote, PacketHandler.S2CEmoteHandler);
        _onRecv.Add((ushort)MsgId.S2CChat, MakePacket<S2CChat>);
        _handler.Add((ushort)MsgId.S2CChat, PacketHandler.S2CChatHandler);
        _onRecv.Add((ushort)MsgId.S2CSpawn, MakePacket<S2CSpawn>);
        _handler.Add((ushort)MsgId.S2CSpawn, PacketHandler.S2CPlayerSpawnHandler);
        _onRecv.Add((ushort)MsgId.S2CDespawn, MakePacket<S2CDespawn>);
        _handler.Add((ushort)MsgId.S2CDespawn, PacketHandler.S2CDespawnHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerMove, MakePacket<S2CPlayerMove>);
        _handler.Add((ushort)MsgId.S2CPlayerMove, PacketHandler.S2CPlayerMoveHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerLocation, MakePacket<S2CPlayerLocation>);
        _handler.Add((ushort)MsgId.S2CPlayerLocation, PacketHandler.S2CPlayerLocationHandler);
        _onRecv.Add((ushort)MsgId.S2CPlayerRunning, MakePacket<S2CPlayerRunning>);
        _handler.Add((ushort)MsgId.S2CPlayerRunning, PacketHandler.S2CPlayerRunningHandler);
        _onRecv.Add((ushort)MsgId.S2CPortal, MakePacket<S2CPortal>);
        _handler.Add((ushort)MsgId.S2CPortal, PacketHandler.S2CPortalHandler);
        _onRecv.Add((ushort)MsgId.S2CUpdateRanking, MakePacket<S2CUpdateRanking>);
        _handler.Add((ushort)MsgId.S2CUpdateRanking, PacketHandler.S2CUpdateRankingHandler);
        _onRecv.Add((ushort)MsgId.S2CCollision, MakePacket<S2CCollision>);
        _handler.Add((ushort)MsgId.S2CCollision, PacketHandler.S2CCollisionHandler);

        _onRecv.Add((ushort)MsgId.S2CInventoryUpdate, MakePacket<S2CInventoryUpdate>);
        _handler.Add((ushort)MsgId.S2CInventoryUpdate, PacketHandler.S2CInventoryUpdateHandler);

        _onRecv.Add((ushort)MsgId.S2CCreateParty, MakePacket<S2CCreateParty>);
        _handler.Add((ushort)MsgId.S2CCreateParty, PacketHandler.S2CCreatePartyHandler);
        _onRecv.Add((ushort)MsgId.S2CInviteParty, MakePacket<S2CInviteParty>);
        _handler.Add((ushort)MsgId.S2CInviteParty, PacketHandler.S2CInvitePartyHandler);
        _onRecv.Add((ushort)MsgId.S2CAllowInvite, MakePacket<S2CAllowInvite>);
        _handler.Add((ushort)MsgId.S2CAllowInvite, PacketHandler.S2CAllowInviteHandler);
        _onRecv.Add((ushort)MsgId.S2CJoinParty, MakePacket<S2CJoinParty>);
        _handler.Add((ushort)MsgId.S2CJoinParty, PacketHandler.S2CJoinPartyHandler);
        _onRecv.Add((ushort)MsgId.S2CLeaveParty, MakePacket<S2CLeaveParty>);
        _handler.Add((ushort)MsgId.S2CLeaveParty, PacketHandler.S2CLeavePartyHandler);
        _onRecv.Add((ushort)MsgId.S2CCheckPartyList, MakePacket<S2CCheckPartyList>);
        _handler.Add((ushort)MsgId.S2CCheckPartyList, PacketHandler.S2CCheckPartyListHandler);
        _onRecv.Add((ushort)MsgId.S2CKickOutMember, MakePacket<S2CKickOutMember>);
        _handler.Add((ushort)MsgId.S2CKickOutMember, PacketHandler.S2CKickOutMemberHandler);
        _onRecv.Add((ushort)MsgId.S2CDisbandParty, MakePacket<S2CDisbandParty>);
        _handler.Add((ushort)MsgId.S2CDisbandParty, PacketHandler.S2CDisbandPartyHandler);
        _onRecv.Add((ushort)MsgId.C2SRejectInvite, MakePacket<C2SRejectInvite>);
        _handler.Add((ushort)MsgId.S2CRejectInvite, PacketHandler.S2CRejectInviteHandler);
        _onRecv.Add((ushort)MsgId.S2CMonsterLocation, MakePacket<S2CMonsterLocation>);
        _handler.Add((ushort)MsgId.S2CMonsterLocation, PacketHandler.S2CMonsterLocationHandler);
        _onRecv.Add((ushort)MsgId.S2CDetectedPlayer, MakePacket<S2CDetectedPlayer>);
        _handler.Add((ushort)MsgId.S2CDetectedPlayer, PacketHandler.S2CDetectedPlayerHandler);
        _onRecv.Add((ushort)MsgId.S2CMissingPlayer, MakePacket<S2CMissingPlayer>);
        _handler.Add((ushort)MsgId.S2CMissingPlayer, PacketHandler.S2CMissingPlayerHandler);

        _onRecv.Add((ushort)MsgId.S2CResourcesList, MakePacket<S2CResourcesList>);
        _handler.Add((ushort)MsgId.S2CResourcesList, PacketHandler.S2CResourceListHandler);
        _onRecv.Add((ushort)MsgId.S2CUpdateDurability, MakePacket<S2CUpdateDurability>);
        _handler.Add((ushort)MsgId.S2CUpdateDurability, PacketHandler.S2CUpdateDurabilityHandler);
        _onRecv.Add((ushort)MsgId.S2CGatheringStart, MakePacket<S2CGatheringStart>);
        _handler.Add((ushort)MsgId.S2CGatheringStart, PacketHandler.S2CGatheringStartHandler);
        _onRecv.Add((ushort)MsgId.S2CGatheringSkillCheck, MakePacket<S2CGatheringSkillCheck>);
        _handler.Add(
            (ushort)MsgId.S2CGatheringSkillCheck,
            PacketHandler.S2CGatheringSkillCheckHandler
        );
        _onRecv.Add((ushort)MsgId.S2CGatheringDone, MakePacket<S2CGatheringDone>);
        _handler.Add((ushort)MsgId.S2CGatheringDone, PacketHandler.S2CGatheringDoneHandler);
        _onRecv.Add((ushort)MsgId.S2CRecall, MakePacket<S2CRecall>);
        _handler.Add((ushort)MsgId.S2CRecall, PacketHandler.S2CRecallHandler);
        _onRecv.Add((ushort)MsgId.S2CThrowGrenade, MakePacket<S2CThrowGrenade>);
        _handler.Add((ushort)MsgId.S2CThrowGrenade, PacketHandler.S2CThrowGrenadeHandler);
        _onRecv.Add((ushort)MsgId.S2CTraps, MakePacket<S2CTraps>);
        _handler.Add((ushort)MsgId.S2CTraps, PacketHandler.S2CTrapsHandler);
        _onRecv.Add((ushort)MsgId.S2CSetTrap, MakePacket<S2CSetTrap>);
        _handler.Add((ushort)MsgId.S2CSetTrap, PacketHandler.S2CSetTrapHandler);
        _onRecv.Add((ushort)MsgId.S2CRemoveTrap, MakePacket<S2CRemoveTrap>);
        _handler.Add((ushort)MsgId.S2CRemoveTrap, PacketHandler.S2CRemoveTrapHandler);
        _onRecv.Add((ushort)MsgId.S2CStun, MakePacket<S2CStun>);
        _handler.Add((ushort)MsgId.S2CStun, PacketHandler.S2CStunHandler);
        _onRecv.Add((ushort)MsgId.S2CEquipChange, MakePacket<S2CEquipChange>);
        _handler.Add((ushort)MsgId.S2CEquipChange, PacketHandler.S2CEquipChangeHandler);
        _onRecv.Add((ushort)MsgId.S2CAddExp, MakePacket<S2CAddExp>);
        _handler.Add((ushort)MsgId.S2CAddExp, PacketHandler.S2CAddExpHandler);
        _onRecv.Add((ushort)MsgId.S2CLevelUp, MakePacket<S2CLevelUp>);
        _handler.Add((ushort)MsgId.S2CLevelUp, PacketHandler.S2CLevelUpHandler);
        _onRecv.Add((ushort)MsgId.S2CInvestPoint, MakePacket<S2CInvestPoint>);
        _handler.Add((ushort)MsgId.S2CInvestPoint, PacketHandler.S2CInvestPointHandler);

        _onRecv.Add((ushort)MsgId.S2CCraft, MakePacket<S2CCraft>);
        _handler.Add((ushort)MsgId.S2CCraft, PacketHandler.S2CCraftHandler);
        _onRecv.Add((ushort)MsgId.S2CPing, MakePacket<S2CPing>);
        _handler.Add((ushort)MsgId.S2CPing, PacketHandler.S2CPingHandler);
        _onRecv.Add((ushort)MsgId.S2CGetInventorySlotByItemId, MakePacket<S2CGetInventorySlotByItemId>);
        _handler.Add((ushort)MsgId.S2CGetInventorySlotByItemId, PacketHandler.S2CGetInventorySlotByItemIdHandler);

        Debug.Log("핸들러 등록 완료");
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        // Debug.Log(
        //     $"PacketManager.OnRecvPacket 호출: {BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count)}"
        // );

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
