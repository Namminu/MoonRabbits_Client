using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 4;
        // 패킷 데이터 조립을 위한 내부 버퍼: [size(4)][packetId(1 또는 2)] 등으로 구성될 수 있음
        private List<byte> _pendingBuffer = new List<byte>();

        /// <summary>
        /// 수신된 원시 데이터 버퍼를 내부 버퍼에 추가하고, 
        /// 완전한 패킷 단위로 분리하여 OnRecvPacket()을 호출합니다.
        /// </summary>
        /// <param name="buffer">수신된 데이터 버퍼</param>
        /// <returns>처리한 총 바이트 수</returns>
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            // 들어온 데이터를 내부 _pendingBuffer에 추가
            _pendingBuffer.AddRange(buffer.Take(buffer.Count));
            int processLen = 0;

            // 헤더 길이(4바이트) 이상 데이터가 누적되어 있으면 패킷 처리 진행
            while (_pendingBuffer.Count >= HeaderSize)
            {
                // 버퍼의 첫 4바이트를 읽어 전체 패킷 크기를 구합니다.
                int dataSize = BitConverter.ToInt32(_pendingBuffer.ToArray(), 0);

                // 데이터 길이가 이상하거나 아직 완전한 패킷이 도착하지 않은 경우 대기
                if (dataSize <= 0 || _pendingBuffer.Count < dataSize)
                {
                    Debug.Log("패킷 크기가 부족하여 대기합니다.");
                    break;
                }

                // 완전한 패킷 데이터를 추출
                byte[] packet = _pendingBuffer.GetRange(0, dataSize).ToArray();

                // 완전한 패킷을 전달하여 패킷별 처리를 수행합니다.
                OnRecvPacket(new ArraySegment<byte>(packet, 0, dataSize));
                processLen += dataSize;

                // 처리한 데이터는 버퍼에서 제거
                _pendingBuffer.RemoveRange(0, dataSize);
            }
            return processLen;
        }

        /// <summary>
        /// 추출된 완전한 패킷 데이터를 인자로 받아 적절한 패킷 핸들러로 전달하는 추상 메서드
        /// (구현체에서는 PacketManager.Instance.OnRecvPacket(this, buffer) 등을 호출할 수 있음)
        /// </summary>
        /// <param name="buffer">완전한 패킷 데이터</param>
        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        public bool IsConnected { get; protected set; } = false;
        protected Socket _socket;
        int _disconnected = 0;
        RecvBuffer _recvBuffer = new RecvBuffer(65535);
        readonly object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        /// <summary>
        /// 내부 상태(버퍼, 큐 등)를 초기화합니다.
        /// </summary>
        void Clear()
        {
            lock (_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Start(Socket socket)
        {
            Debug.Log("Session Start 호출됨");
            _socket = socket;

            _recvArgs.Completed += OnRecvCompleted;
            _sendArgs.Completed += OnSendCompleted;
            RegisterRecv();
            Debug.Log("RegisterRecv 호출됨");
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            OnDisconnected(_socket.RemoteEndPoint);

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Socket shutdown 예외: " + ex.Message);
            }
            _socket.Close();
            Clear();

            // 이벤트 핸들러 해제하여 재접속 시 잔여 이벤트를 막음.
            _recvArgs.Completed -= OnRecvCompleted;
            _sendArgs.Completed -= OnSendCompleted;
        }

        #region 네트워크 통신 처리

        void RegisterSend()
        {
            if (_disconnected == 1)
                return;

            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                _pendingList.Add(buff);
            }
            _sendArgs.BufferList = _pendingList;

            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (!pending)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception e)
            {
                Debug.Log("RegisterSend Failed " + e);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();
                        OnSend(_sendArgs.BytesTransferred);
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("OnSendCompleted 예외: " + e);
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterRecv()
        {
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                Debug.Log("ReceiveAsync 호출");
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (!pending)
                    OnRecvCompleted(null, _recvArgs);
            }
            catch (Exception e)
            {
                Debug.LogError("RegisterRecv 예외: " + e.Message);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    if (_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Debug.Log("OnRecvCompleted 예외: " + e);
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}
