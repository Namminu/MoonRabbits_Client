using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Net;
using Google.Protobuf;
using ServerCore;


public class TestServer : MonoBehaviour
{
    private const string DefaultIP = "127.0.0.1";
    private const int DefaultPort = 3000;
    private const int MaxConnections = 1;

    private ServerSession _session;
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private Thread receiveThread;
    private bool isRunning = false;

    public bool IsConnected => _session?.IsConnected ?? false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        try
        {
            _session = new ServerSession();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(DefaultIP), DefaultPort);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => _session, MaxConnections);

            isRunning = true;
            Debug.Log("서버에 연결되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 연결 실패: {e.Message}");
        }
    }

    public void Send(IMessage packet)
    {
        _session?.Send(packet);
    }

    private void Update()
    {
        // 패킷 큐에서 모든 메시지를 가져와 처리
        var list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            handler?.Invoke(_session, packet.Message);
        }
    }

    private void OnDestroy()
    {
        isRunning = false;

        if (_session != null)
        {
            _session = null;
        }

        if (receiveThread != null)
        {
            receiveThread.Abort();
        }

        if (networkStream != null)
        {
            networkStream.Close();
        }

        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }

    // IP와 포트를 직접 지정할 경우 사용하는 초기화 메서드
    public void Init(string ipString, string portString)
    {
        string ip = !string.IsNullOrEmpty(ipString) ? ipString : DefaultIP;
        int port = int.TryParse(portString, out int p) ? p : DefaultPort;

        try
        {
            _session = new ServerSession();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => _session, MaxConnections);

            isRunning = true;
            Debug.Log($"서버에 연결되었습니다. (IP: {ip}, Port: {port})");
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 연결 실패: {e.Message}");
        }
    }
}
