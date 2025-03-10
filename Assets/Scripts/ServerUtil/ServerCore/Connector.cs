using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ServerCore
{
    public class Connector
    {
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            Debug.Log("Connector 연결 시도");
            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // sessionFactory는 PacketSession을 반환하는 팩토리 함수를 사용할 수 있습니다.
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                // 각 연결 요청마다 sessionFactory와 소켓 정보를 함께 보관하는 토큰을 생성
                args.UserToken = new ConnectorToken(socket, sessionFactory);
                RegisterConnect(args);
            }
        }

        private void RegisterConnect(SocketAsyncEventArgs args)
        {
            if (args.UserToken is ConnectorToken token)
            {
                Socket socket = token.Socket;
                if (socket == null)
                    return;
                bool pending = socket.ConnectAsync(args);
                if (!pending)
                    OnConnectCompleted(null, args);
            }
        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Debug.Log($"Connector 연결 성공: {args.RemoteEndPoint}");
                if (args.UserToken is ConnectorToken token)
                {
                    // sessionFactory를 호출하여 PacketSession(또는 Session) 객체를 생성합니다.
                    Session session = token.SessionFactory.Invoke();
                    session.Start(args.ConnectSocket);
                    session.OnConnected(args.RemoteEndPoint);
                }
            }
            else
            {
                Debug.LogError($"Connector 연결 실패: {args.SocketError}");
            }
        }

        // 소켓과 sessionFactory를 함께 저장하는 내부 토큰 클래스
        private class ConnectorToken
        {
            public Socket Socket { get; private set; }
            public Func<Session> SessionFactory { get; private set; }
            public ConnectorToken(Socket socket, Func<Session> sessionFactory)
            {
                Socket = socket;
                SessionFactory = sessionFactory;
            }
        }
    }
}
