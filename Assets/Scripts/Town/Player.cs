using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField]
    private UINameChat uiNameChat;

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
    private NavMeshAgent agent;
    private Vector3 lastTargetPosition;
    private Vector3 targetPosition; // 목표 위치 저장
    private float moveSpeed = 10f; // 이동 속도

    [Header("Skill Settings")]
    /* 섬광탄 관련 변수 */
    public Transform throwPoint;
    public int throwPower;
    private bool grenadeInput;
    private bool trapInput;
    private bool isThrow = false;
    public GameObject grenade;
    public GameObject trap;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
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

    public void MoveToTargetPosition(Vector3 position)
    {
        goalPos = position; // 목표 위치 업데이트
    }

    private void Update()
    {
        // if (!isInitialized)
        //     return;

        // 인풋 키 받는 함수 (스킬이나 조작 추가시 여기에 추가)
        GetInput();

        // if (!IsMine)
        // {
        //     SmoothMoveAndRotate();
        // }

        CheckMove();

        Throw();
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
            transform.position = Vector3.Lerp(
                transform.position,
                goalPos,
                Time.deltaTime * SmoothMoveSpeed
            );
        }
    }

    private void RotateSmoothly()
    {
        if (goalRot != Quaternion.identity)
        {
            float t = Mathf.Clamp(Time.deltaTime * SmoothRotateSpeed, 0, 0.99f);
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
        }
    }

    public void SendChatMessage(string msg)
    {
        if (!IsMine)
            return;

        var chatPacket = new C2SChat
        {
            PlayerId = PlayerId,
            SenderName = nickname,
            ChatMsg = msg,
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

    private void GetInput()
    {
        grenadeInput = Input.GetButtonDown("Grenade");
        trapInput = Input.GetButtonDown("Trap");
    }

    private void Throw()
    {
        if (!isThrow && (grenadeInput || trapInput))
        {
            Debug.Log("Throw 진입");
            isThrow = true;

            if (grenadeInput)
            {
                Debug.Log("Grenade 진입");
                GameObject currentObj = Instantiate(
                    grenade,
                    throwPoint.position,
                    throwPoint.rotation
                );

                Rigidbody rigid = currentObj.GetComponent<Rigidbody>();

                Vector3 forceVec = throwPoint.forward * throwPower + throwPoint.up * throwPower / 2;

                rigid.AddForce(forceVec, ForceMode.Impulse);
                rigid.AddTorque(Vector3.right, ForceMode.Impulse);
            }
            else if (trapInput)
            {
                // currentObj = Instantiate(trap, throwPoint.position, throwPoint.rotation);
            }

            Invoke(nameof(ThrowEnd), 3f);
        }
    }

    private void ThrowEnd()
    {
        Debug.Log("ThrowEnd 진입");
        isThrow = false;
    }
}
