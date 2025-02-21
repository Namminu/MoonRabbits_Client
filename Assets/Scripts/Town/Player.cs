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

    public string nickname;
    public int level;
    private UIChat uiChat;

    private Vector3 goalPos;
    private Quaternion goalRot;

    private Animator animator;

    public int PlayerId { get; private set; }
    public bool IsMine { get; private set; }
    private bool isInitialized = false;

    private Vector3 lastPos;

    [Header("Throw Item")]
    public GameObject grenade;
    public GameObject trap;

    // PlayerInfo
    private int exp;
    private int targetExp;
    private int stamina;
    private int cur_stamina;
    private int pickSpeed;
    private int moveSpeed;
    private int abilityPoint;
    private int cur_hp;
    private int hp;

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
        TownManager.Instance.UiPlayer.SetNickname(nickname);
    }

    public void SetLevel(int level)
    {
        this.level = level;
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
        if (!isInitialized)
            return;

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

    public void SetStatInfo(StatInfo statInfo)
    {
        level = statInfo.Level;
        exp = statInfo.Exp;
        targetExp = statInfo.TargetExp;
        cur_stamina = statInfo.CurStamina;
        stamina = statInfo.Stamina;
        pickSpeed = statInfo.PickSpeed;
        moveSpeed = statInfo.MoveSpeed;
        abilityPoint = statInfo.AbilityPoint;

        TownManager.Instance.UiPlayer.SetStatInfo(statInfo);
    }

    public void SetExp(int updatedExp)
    {
        Debug.Log("경험치 응답 실행");
        exp = updatedExp;
        if (exp > targetExp)
        {
            Debug.Log($"WTF! exp too much {exp}, {targetExp}");
            return;
        }
        Debug.Log($"updatedExp : {updatedExp}");

        TownManager.Instance.UiPlayer.SetExp(updatedExp, targetExp);
    }

    public void InvestPoint(StatInfo statInfo)
    {
        Debug.Log("능력치 투자자 응답 실행");
        abilityPoint = statInfo.AbilityPoint;
        TownManager.Instance.UiPlayer.SetAbilityPoint(abilityPoint);
        
        if(statInfo.Stamina > stamina){
            StaminaUp();
        }
        if(statInfo.PickSpeed > pickSpeed){
            PickSpeedUp();
        }
        if(statInfo.MoveSpeed > moveSpeed){
            MoveSpeedUp();
        }

        if(abilityPoint <= 0) TownManager.Instance.UiPlayer.DeActiveAP();
    }

    private void StaminaUp()
    {
        stamina++;
        TownManager.Instance.UiPlayer.SetStamina(cur_stamina, stamina);
    }

    private void PickSpeedUp()
    {
        pickSpeed++;
        TownManager.Instance.UiPlayer.SetPickSpeed(pickSpeed);
    }

    private void MoveSpeedUp()
    {
        moveSpeed++;
        TownManager.Instance.UiPlayer.SetMoveSpeed(moveSpeed);
    }

    public void LevelUp(int updatedLevel, int newTargetExp, int updatedExp, int updatedAbilityPoint)
    {
        Debug.Log("레벨업 응답 실행");
        level = updatedLevel;
        abilityPoint = updatedAbilityPoint;
        targetExp = newTargetExp;
        exp = updatedExp;

        TownManager.Instance.UiPlayer.LevelUp(updatedLevel, newTargetExp, updatedExp, abilityPoint);
    }
}
