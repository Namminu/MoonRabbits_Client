using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private UINameChat uiNameChat;

    [Header("Movement Settings")]
    public float SmoothMoveSpeed = 10f; // 위치 보간 속도
    public float SmoothRotateSpeed = 10f; // 회전 보간 속도
    public float TeleportDistanceThreshold = 0.5f; // 순간 이동 거리 임계값

    public Avatar Avatar { get; private set; }
    public MyPlayer MPlayer { get; private set; }

    private string nickname;
    private UIChat uiChat;

    private Vector3 goalPos;
    private Quaternion goalRot;

    private Animator animator;

    public int PlayerId { get; private set; }
    public bool IsMine { get; private set; }
    private bool isInitialized = false;

    private Vector3 lastPos;
    private Coroutine locationCoroutine;

    private void Start()
    {
        Avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
        // 코루틴 시작
        locationCoroutine = StartCoroutine(SendLocationRoutine());
    }
    IEnumerator SendLocationRoutine()
    {
        while (true) // 무한 루프 (게임이 실행되는 동안 계속 실행)
        {
            SendLocationPacket(); // 위치 패킷 전송
            yield return new WaitForSeconds(0.5f); // 0.5초 대기
        }
    }

    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
    }

    public void SetNickname(string nickname)
    {
        this.nickname = nickname;
        uiNameChat.SetName(nickname);
    }

    public void SetIsMine(bool isMine)
    {
        IsMine = isMine;

        if (IsMine)
        {
            MPlayer = gameObject.AddComponent<MyPlayer>();
        }
        else
        {
            Destroy(GetComponent<NavMeshAgent>());
        }

        uiChat = TownManager.Instance.UiChat;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (!IsMine)
        {
            SmoothMoveAndRotate();
        }

        CheckMove();
    }

    private void SmoothMoveAndRotate()
    {
        MoveSmoothly();
        RotateSmoothly();
    }

    private void MoveSmoothly()
    {
        float distance = Vector3.Distance(transform.position, goalPos);

        if (distance > TeleportDistanceThreshold)
        {
            transform.position = goalPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, goalPos, Time.deltaTime * SmoothMoveSpeed);
        }

        // var movePacket = new C_Move
        // {
        //     StartPosX = transform.position.x,
        //     StartPosY = transform.position.y,
        //     TargetPosX = goalPos.x,
        //     TargetPosY = goalPos.y,
        //     TargetPosZ = goalPos.z
        // };

        // GameManager.Network.Send(movePacket);
    }

    private void SendLocationPacket()
    {
        var tr = new TransformInfo
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            Rot = transform.eulerAngles.y
        };

        var locationPacket = new C_Location { Transform = tr };
        GameManager.Network.Send(locationPacket);
    }

    private void RotateSmoothly()
    {
        if (goalRot != Quaternion.identity)
        {
            float t = Mathf.Clamp(Time.deltaTime * SmoothRotateSpeed, 0, 0.99f);
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
        }
    }

    public void SendMessage(string msg)
    {
        if (!IsMine)
            return;

        var chatPacket = new C_Chat
        {
            PlayerId = PlayerId,
            SenderName = nickname,
            ChatMsg = msg
        };

        GameManager.Network.Send(chatPacket);
    }

    public void RecvMessage(string msg)
    {
        uiNameChat.PushText(msg);
        uiChat.PushMessage(nickname, msg, IsMine);
    }

    public void Move(Vector3 move, Quaternion rot)
    {
        goalPos = move;
        goalRot = rot;
    }

    public void PlayAnimation(int animCode)
    {
        animator?.SetTrigger(animCode);
    }

    private void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, dist * 100);
        lastPos = transform.position;
    }
}
