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

    private ServerSession _session = new ServerSession();

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

    #endregion
}
