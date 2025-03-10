using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

public class ServerSession : PacketSession
{
    // 연결 종료 시 외부에서 UI를 호출할 수 있도록 이벤트 선언
    public event Action<EndPoint> OnDisconnectedEvent;
    public void Send(IMessage packet)
    {
        string msgName = packet.Descriptor.Name.Replace("_", String.Empty);
        MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

        ushort size = (ushort)packet.CalculateSize();
        // byte[] sendBuff = new byte[size + 4];
        // Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuff, 0, sizeof(ushort)); // 어느정도 크기의 데이터인지
        // Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuff, 2, sizeof(ushort)); // 프로토콜의 아이디
        // Array.Copy(packet.ToByteArray(), 0, sendBuff, 4, size); // 전달하려는 데이터

        byte[] sendBuff = new byte[size + 5]; // 크기(4바이트) + 아이디(1바이트) + 데이터 크기
        Array.Copy(BitConverter.GetBytes(size + 5), 0, sendBuff, 0, sizeof(int)); // 데이터 크기 (4바이트)
        sendBuff[4] = (byte)msgId; // 프로토콜의 아이디 (1바이트)
        Array.Copy(packet.ToByteArray(), 0, sendBuff, 5, size); // 전달하려는 데이터

        Send(new ArraySegment<byte>(sendBuff));
        if (msgName == "C2SLogin")
        {
            Debug.Log($"Login==> : {BitConverter.ToString(sendBuff)}");
        }
        else if (msgName == "C2SEnter")
        {
            Debug.Log($"Enter==> : {BitConverter.ToString(sendBuff)}");
        }
        else if (msgName == "C2SPlayerMove")
        {
            Debug.Log($"PlayerMove==> : {BitConverter.ToString(sendBuff)}");
        }

    }

    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"OnConnected : {endPoint}");
        IsConnected = true;

        // TownManager.Instance.Connected();

        PacketManager.Instance.CustomHandler = (s, m, i) =>
        {
            PacketQueue.Instance.Push(i, m);
#if !UNITY_EDITOR
            Debug.Log($"Packet id : [{i}] {(MsgId)i}  -  msg : {m}");
#endif
        };

        Debug.Log($"클라이언트 연결 상태: {IsConnected}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Debug.Log($"OnDisconnected : {endPoint}");
        IsConnected = false;
        // 이벤트를 발생시켜 NetworkManager 등 외부에서 UI 호출 등의 처리를 진행할 수 있도록 함
        OnDisconnectedEvent?.Invoke(endPoint);
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        Debug.Log($"패킷 수신 크기: {buffer.Count}");
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        //Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}
