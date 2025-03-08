using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class NetworkManager
{
    private Socket _socket;
    private NetworkStream _networkStream;
    private bool _isConnected = false;

    private const string DefaultIP = "127.0.0.1";
    private const int DefaultPort = 3000;
    private const int MaxConnections = 1;
    public IPEndPoint serverIP;
    public string _ipString;

    private ServerSession _session = new ServerSession();

    public void Discconect()
    {
        _session.Disconnect();
    }

    public void Reconnect()
    {
        _networkStream?.Close();
        _networkStream = null;

        if (_socket != null)
        {
            _socket.Shutdown(SocketShutdown.Both); // 읽기/쓰기 모두 중지
            _socket.Close();
            _socket = null;
        }

        // 상태 초기화
        _session.Disconnect();
        _isConnected = false;
        // 여기서 플레이어 오브젝트 싹 날리고 새로 불러오기
        SceneManager.LoadScene("Town");
    }

    public bool IsConnected => _session?.IsConnected ?? false;

    public void Send(IMessage packet)
    {
        _session?.Send(packet);
    }

    public void Init()
    {
        IPEndPoint endPoint = GetDefaultEndPoint();
        InitializeConnection(endPoint);
    }

    public void Init(string ipString, string portString)
    {
        IPAddress ipAddr = ParseIPAddress(ipString, DefaultIP);
        int port = ParsePort(portString, DefaultPort);

        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);
        InitializeConnection(endPoint);
        serverIP = endPoint;
        _ipString = ipString;

        // 3초 후 Discconect => InitializeConnection

    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            handler?.Invoke(_session, packet.Message);
        }
    }

    #region Private Methods

    public void InitializeConnection(IPEndPoint endPoint)
    {
        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session, MaxConnections);
        _isConnected = true;
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

    #endregion
}
