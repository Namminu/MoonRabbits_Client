using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
    private const string DefaultIP = "127.0.0.1";
    private const int DefaultPort = 3000;
    private const int MaxConnections = 1;

    /*private ServerSession _session = new ServerSession();*/
    // 기존 연결 재사용 대신, 재접속 시 새로운 Session을 생성하도록 함
    private ServerSession _session;

    public void Disconnect()
    {
        if (_session != null)
        {
            _session.Disconnect();
        }
    }

    public bool IsConnected => _session?.IsConnected ?? false;

    public void Send(IMessage packet)
    {
        _session?.Send(packet);
    }

    /*   public void Init()
       {
           IPEndPoint endPoint = GetDefaultEndPoint();
           InitializeConnection(endPoint);
       }*/
    // 기본 IP, Port를 사용한 초기 연결 시도
    public void Init()
    {
        if (_session != null)
        {
            _session.Disconnect();
        }
        _session = new ServerSession();
        _session.OnDisconnectedEvent += OnSessionDisconnected;
        IPEndPoint endPoint = GetDefaultEndPoint();
        InitializeConnection(endPoint);
    }

    // 입력된 ipString, portString을 사용한 초기 연결 시도
    public void Init(string ipString, string portString)
    {
        if (_session != null)
        {
            _session.Disconnect();
        }
        _session = new ServerSession();
        _session.OnDisconnectedEvent += OnSessionDisconnected;
        IPAddress ipAddr = ParseIPAddress(ipString, DefaultIP);
        int port = ParsePort(portString, DefaultPort);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
        InitializeConnection(endPoint);
    }

    // 잘못된 IP/Port 입력 후 재연결 시 호출하는 메서드
    public void RetryConnection(string ipString, string portString)
    {
        Debug.Log("재접속 시도 시작");
        Disconnect();
        _session = new ServerSession();
        _session.OnDisconnectedEvent += OnSessionDisconnected;
        IPAddress ipAddr = ParseIPAddress(ipString, DefaultIP);
        int port = ParsePort(portString, DefaultPort);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
        InitializeConnection(endPoint);
    }

    // PacketQueue에서 수신된 패킷들을 꺼내어 PacketManager에 등록된 핸들러를 호출함
    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
            {
                handler.Invoke(_session, packet.Message);
            }
        }
    }

    #region Private Methods

    private void InitializeConnection(IPEndPoint endPoint)
    {
        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session, MaxConnections);
    }

    private IPEndPoint GetDefaultEndPoint()
    {
        try
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            return new IPEndPoint(ipAddr, DefaultPort);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get default endpoint: {ex.Message}");
            return new IPEndPoint(IPAddress.Parse(DefaultIP), DefaultPort);
        }
    }

    private IPAddress ParseIPAddress(string ipString, string defaultIP)
    {
        return IPAddress.TryParse(ipString, out IPAddress ipAddr) ? ipAddr : IPAddress.Parse(defaultIP);
    }

    private int ParsePort(string portString, int defaultPort)
    {
        return int.TryParse(portString, out int port) ? port : defaultPort;
    }

    // 세션 연결 종료 시 호출되는 이벤트 핸들러.
    // UIManager.Instance.ShowDisconnectPopup()는 미리 준비된 팝업을 호출하여,
    // 사용자가 확인 버튼을 누르면 Application.Quit()를 실행하도록 구현되어야 합니다.
    private void OnSessionDisconnected(EndPoint endPoint)
    {
        Debug.Log($"세션 연결 종료됨: {endPoint}");
        UIDisconnect.Instance.ShowDisconnectPopup();
    }

    #endregion
}
