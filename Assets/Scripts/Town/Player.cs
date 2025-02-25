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

    [Header("Equipments")]
    public GameObject grenade;
    public GameObject trap;
    public GameObject axe;
    public GameObject pickAxe;
    
    // PlayerInfo
    private int maxHp;
    private int curHp;
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
        if(IsMine) {
            TownManager.Instance.UiPlayer.SetNickname(nickname);
            TownManager.Instance.UiPlayer.InitHp(3);
        }
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
            float t = Mathf.Clamp(Time.deltaTime * SmoothRotateSpeed, 0, 0.3f);
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
        }
    }

    public void SendChatMessage(string msg, string chatType)
    {
        if (!IsMine)
            return;

        var chatPacket = new C2SChat
        {
            PlayerId = PlayerId,
            SenderName = nickname,
            ChatMsg = msg,
            ChatType = chatType
        };

        GameManager.Network.Send(chatPacket);
    }

    public void RecvMessage(string msg, string chatType)
    {
        uiNameChat.PushText(msg);
        uiChat.PushMessage(nickname, msg, chatType, IsMine);
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
    
    // STAT, UI
    public void SetStatInfo(StatInfo statInfo)
    {
        maxHp = 3;
        curHp = 3;
        level = statInfo.Level;
        exp = statInfo.Exp;
        targetExp = statInfo.TargetExp;
        cur_stamina = statInfo.CurStamina;
        stamina = statInfo.Stamina;
        pickSpeed = statInfo.PickSpeed;
        moveSpeed = statInfo.MoveSpeed;
        abilityPoint = statInfo.AbilityPoint;

        if(IsMine) TownManager.Instance.UiPlayer.SetStatInfo(statInfo);
    }

    public void SetExp(int updatedExp)
    {
        Debug.Log("경험치 응답 실행");
        exp = updatedExp;
        if (exp > targetExp)
        {
            Debug.LogError($"exp({exp}) > targetExp({targetExp})");
            return;
        }
        Debug.Log($"updatedExp : {updatedExp}");

        if(IsMine) TownManager.Instance.UiPlayer.SetExp(updatedExp, targetExp);
    }

    public void InvestPoint(StatInfo statInfo)
    {
        abilityPoint = statInfo.AbilityPoint;
        if(IsMine) TownManager.Instance.UiPlayer.SetAbilityPoint(abilityPoint);
        
        if(statInfo.Stamina > stamina){
            SetStamina(statInfo.Stamina);
        }
        if(statInfo.PickSpeed > pickSpeed){
            SetPickSpeed(statInfo.PickSpeed);
        }
        if(statInfo.MoveSpeed > moveSpeed){
            SetMoveSpeed(statInfo.MoveSpeed);
        }

        if(IsMine && abilityPoint <= 0) TownManager.Instance.UiPlayer.DeActiveAP();
    }

    private void SetStamina(int stamina)
    {
        if(stamina > this.stamina) this.cur_stamina += (stamina - this.stamina);
        this.stamina = stamina;
        if(IsMine) TownManager.Instance.UiPlayer.SetStamina(cur_stamina, stamina, abilityPoint>0);
    }

    private void SetPickSpeed(int pickSpeed)
    {
        this.pickSpeed = pickSpeed;
        if(IsMine) TownManager.Instance.UiPlayer.SetPickSpeed(pickSpeed, abilityPoint>0);
    }

    private void SetMoveSpeed(int moveSpeed)
    {
        // 플레이어 오브젝트 속도 변경
        GetComponent<NavMeshAgent>().speed = moveSpeed * 1 + 5;
        GetComponent<NavMeshAgent>().angularSpeed = 300 + moveSpeed * 100;
        GetComponent<NavMeshAgent>().acceleration = moveSpeed * 2 + 3;

        this.moveSpeed = moveSpeed;
        if(IsMine) TownManager.Instance.UiPlayer.SetMoveSpeed(moveSpeed, abilityPoint>0);
    }

    public void LevelUp(int updatedLevel, int newTargetExp, int updatedExp, int updatedAbilityPoint)
    {
        level = updatedLevel;
        targetExp = newTargetExp;
        exp = updatedExp;

        Debug.Log($"레벨업 응답 실행 {level}/ap{abilityPoint}/{exp}/{targetExp} isMine?{IsMine}");
        if(IsMine) TownManager.Instance.UiPlayer.LevelUp(updatedLevel, newTargetExp, updatedExp, abilityPoint, updatedAbilityPoint);
        
        abilityPoint = updatedAbilityPoint;
    }

    public void LevelUpOther()
    {
        // 다른 플레이어 레벨 표시 변경
        // TownManager.Instance.SetPlayerLevel(playerId, level);

        // 레벨업 이펙트
        // TownManager.Instance.GetPlayerAvatarById(playerId).이펙트함수;
    }
}
