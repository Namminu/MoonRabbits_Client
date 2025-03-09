using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField]
    private UINameChat uiNameChat;
    private UIPlayer uiPlayer;

    [Header("Movement Settings")]
    public float SmoothMoveSpeed = 15f; // 위치 보간 속도
    public float SmoothRotateSpeed = 15f; // 회전 보간 속도
    public float TeleportDistanceThreshold = 1f; // 순간 이동 거리 임계값

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
    private Transform throwPoint;

    private Dictionary<int, string> emotions = new();
    public bool IsStun = false;
    private Dictionary<int, GameObject> equips = new();
    public GameObject ActiveEquipObj = null;

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

    //불멸의 시간이 다가왔다.
    private float startImotalTime = 5f;
    private float ImotalTime = 1f;
    private bool isImotal = false;
    public bool GetIsImotal
    {
        get { return isImotal; }
    }

    private void Start()
    {
        Avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
        throwPoint = transform.Find("ThrowPoint");

        SetAnimTrigger();
        SetEquipObj();

        StartCoroutine(CoImotalTime(startImotalTime));
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        if (!IsMine)
        {
            SmoothMoveAndRotate();
        }
    }

    private void LateUpdate()
    {
        if (!IsMine)
        {
            CheckMove();
        }
    }

    private void SetAnimTrigger()
    {
        emotions[111] = "Happy";
        emotions[222] = "Sad";
        emotions[333] = "Greeting";
        emotions[10] = "Exit";
        emotions[11] = "PickAxe";
        emotions[12] = "Axe";
    }

    private void SetEquipObj()
    {
        equips[1] = axe;
        equips[2] = pickAxe;
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

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public int GetHp()
    {
        return this.curHp;
    }

    public int SetHp(int num)
    {
        return this.curHp = num;
    }

    public void SetIsMine(bool isMine)
    {
        IsMine = isMine;

        if (IsMine)
        {
            MPlayer = gameObject.AddComponent<MyPlayer>();
            GameManager.Instance.MPlayer = this;
        }
        else
        {
            Destroy(GetComponent<NavMeshAgent>());
        }

        uiChat = GameManager.Instance.SManager.UiChat;
        uiPlayer = GameManager.Instance.SManager.UiPlayer;
        isInitialized = true;
    }

    public void SetCollision(CollisionPushInfo info)
    {
        var type = info.MyType;
        switch (type)
        {
            case 1:
                SetMyCollision(info);
                break; //플레이어
            case 2:
                SetMonsterCollision(info);
                break; // 몬스터
        }
    }

    private void SetMyCollision(CollisionPushInfo info) // 내가 인식 했을경우
    {
        var type = info.TargetType;
        switch (type)
        {
            case 1: //플레이어와의 충돌
                break;
            case 2: // 몬스터와의 충돌
                break;
            case 3: //덫?
                break;
            case 4: // 폭탄?
                break;
        }
    }

    private void SetMonsterCollision(CollisionPushInfo info) // 몬스터가 나에게 알려줄경우
    {
        //체력을 깎는다.
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
            float t = Mathf.Clamp(Time.deltaTime * SmoothRotateSpeed, 0, 0.5f); // 보간 범위 조정
            // transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
            transform.rotation = goalRot;
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
            ChatType = chatType,
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

    public void Emote(int animCode)
    {
        Debug.Log(emotions[animCode]);
        if (animCode < 100)
        {
            if (animCode == 10)
            {
                animator?.SetBool(emotions[12], false);
                animator?.SetBool(emotions[11], false);
            }
            else
            {
                animator?.SetBool(emotions[animCode], true);
            }
        }
        else
        {
            animator?.SetTrigger(emotions[animCode]);
        }
    }

    public void StartOpenChest(int openTimer)
    {
        StartCoroutine(TryOpen(openTimer));
    }

    IEnumerator TryOpen(int openTimer)
    {
        animator.SetBool("OpenChest", true);
        // 진행도 유아이 액티브?
        Vector3 startPos = transform.position;

        int openWaiting = 0;

        while (openWaiting < openTimer)
        {
            if (CancelOpen(startPos))
                yield break;

            yield return new WaitForSeconds(0.5f);

            if (CancelOpen(startPos))
                yield break;

            yield return new WaitForSeconds(0.5f);
            Debug.Log($"상자 오픈까지 남은 시간 : {openTimer - openWaiting}초");
            openWaiting += 1;
        }

        yield return new WaitForSeconds(0.5f);
        animator.SetBool("OpenChest", false);

        GameObject chest = GameManager.Instance.SManager.Chest.gameObject;
        if (chest != null && chest.activeSelf)
        {
            chest.SetActive(false);
        }

        if (IsMine)
        {
            MPlayer.SkillManager.IsCasting = false;
            MPlayer.InteractManager.IsInteracting = false;

            var pkt = new C2SGetTreasure { };
            GameManager.Network.Send(pkt);
        }
    }

    private bool CancelOpen(Vector3 startPos)
    {
        if (Vector3.Distance(startPos, transform.position) > 0.5f || IsStun)
        {
            animator.SetBool("OpenChest", false);

            if (IsMine)
            {
                MPlayer.SkillManager.IsCasting = false;
                MPlayer.InteractManager.IsInteracting = false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void CastRecall(int recallTimer)
    {
        StartCoroutine(TryRecall(recallTimer));
    }

    IEnumerator TryRecall(int recallTimer)
    {
        GameObject effect = transform.Find("RecallEffect").gameObject;
        effect.SetActive(true);

        Vector3 startPos = transform.position;

        int castingTime = 0;

        while (castingTime < recallTimer)
        {
            if (CancelRecall(startPos, effect))
                yield break;

            yield return new WaitForSeconds(0.5f);

            if (CancelRecall(startPos, effect))
                yield break;

            yield return new WaitForSeconds(0.5f);
            Debug.Log($"귀환까지 남은 시간 : {recallTimer - castingTime}초");
            castingTime += 1;
        }

        yield return new WaitForSeconds(0.5f);
        effect.SetActive(false);

        if (IsMine)
        {
            MPlayer.SkillManager.IsCasting = false;

            var pkt = new C2SMoveSector { TargetSector = 100 };
            GameManager.Network.Send(pkt);
        }
    }

    private bool CancelRecall(Vector3 startPos, GameObject effect)
    {
        if (Vector3.Distance(startPos, transform.position) > 0.5f || IsStun)
        {
            effect.SetActive(false);

            if (IsMine)
            {
                MPlayer.SkillManager.IsCasting = false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void CastGrenade(Vec3 vel, float coolTime)
    {
        GameObject grenadeObj = Instantiate(grenade, throwPoint.position, Quaternion.identity);
        SkillObj skillObj = grenadeObj.GetComponent<SkillObj>();
        skillObj.CasterId = PlayerId;

        Rigidbody rigid = grenadeObj.GetComponent<Rigidbody>();
        rigid.velocity = new Vector3(vel.X, vel.Y, vel.Z);
        rigid.AddTorque(Vector3.back, ForceMode.Impulse);

        if (IsMine)
        {
            StartCoroutine(RunCoolTime(coolTime, (int)skillObj.type));
        }
    }

    public void CastTrap(Vec3 pos, float coolTime)
    {
        animator.SetTrigger("SetTrap");

        GameObject trapObj = Instantiate(
            trap,
            new Vector3(pos.X / 10f, 0, pos.Z / 10f),
            transform.rotation
        );

        SkillObj skillObj = trapObj.GetComponent<SkillObj>();
        skillObj.CasterId = PlayerId;

        if (IsMine)
        {
            MPlayer.NavAgent.SetDestination(transform.position);
            MPlayer.NavAgent.velocity = Vector3.zero;
            StartCoroutine(RunCoolTime(coolTime, (int)skillObj.type));
        }
    }

    IEnumerator RunCoolTime(float coolTime, int skillType)
    {
        yield return new WaitForSeconds(1f);
        MPlayer.SkillManager.IsCasting = false;

        yield return new WaitForSeconds(coolTime);

        switch (skillType)
        {
            case 1:
                MPlayer.SkillManager.IsGrenadeReady = true;
                break;
            case 2:
                MPlayer.SkillManager.IsTrapReady = true;
                break;
        }
    }

    public void Stun(float timer)
    {
        if (IsStun)
            return;

        transform.Find("StunEffect").gameObject.SetActive(true);
        IsStun = true;

        // PlayerManager.playerSaveData[PlayerId].IsStun = true;

        if (IsMine)
        {
            MPlayer.NavAgent.ResetPath();
            MPlayer.NavAgent.velocity = Vector3.zero;
        }

        Invoke(nameof(StunOut), timer);
    }

    private void StunOut()
    {
        transform.Find("StunEffect").gameObject.SetActive(false);
        IsStun = false;
        // PlayerManager.playerSaveData[PlayerId].IsStun = false;
    }

    public void ChangeEquip(int nextEquip)
    {
        if (ActiveEquipObj != null && ActiveEquipObj.activeSelf)
        {
            ActiveEquipObj.SetActive(false);
        }

        ActiveEquipObj = equips[nextEquip];
        ActiveEquipObj.SetActive(true);

        if (IsMine)
        {
            MPlayer.currentEquip = nextEquip;
            PlayerManager.playerSaveData[PlayerId].CurrentEquip = nextEquip;
            MPlayer.InteractManager.isEquipChanging = false;
        }

        PartyMemberUI.instance.UpdateUI();
    }

    private void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat("Move", dist * 10);
        lastPos = transform.position;
    }

    // STAT, UI
    public void SetStatInfo(StatInfo statInfo)
    {
        maxHp = 3;
        curHp = statInfo.Hp;
        level = statInfo.Level;
        exp = statInfo.Exp;
        targetExp = statInfo.TargetExp;
        cur_stamina = statInfo.CurStamina;
        stamina = statInfo.Stamina;
        pickSpeed = statInfo.PickSpeed;
        moveSpeed = statInfo.MoveSpeed;
        abilityPoint = statInfo.AbilityPoint;

        SetStamina(stamina);
        SetPickSpeed(pickSpeed);
        SetMoveSpeed(moveSpeed);

        if (IsMine)
        {
            if (uiPlayer == null)
                Debug.LogError("uiPlayer is null. 먼저 세팅돼야함");
            uiPlayer.SetStatInfo(statInfo);
            uiPlayer.SetNickname(nickname);
            uiPlayer.InitHp(curHp);
        }
    }

    public void SetExp(int updatedExp)
    {
        exp = updatedExp;
        if (exp > targetExp)
        {
            Debug.LogError($"exp({exp}) > targetExp({targetExp})");
            return;
        }
        if (IsMine)
            uiPlayer.SetExp(updatedExp, targetExp);
    }

    public void InvestPoint(StatInfo statInfo)
    {
        abilityPoint = statInfo.AbilityPoint;
        if (IsMine)
            uiPlayer.SetAbilityPoint(abilityPoint);

        if (statInfo.Stamina > stamina)
        {
            SetStamina(statInfo.Stamina);
        }
        if (statInfo.PickSpeed > pickSpeed)
        {
            SetPickSpeed(statInfo.PickSpeed);
        }
        if (statInfo.MoveSpeed > moveSpeed)
        {
            SetMoveSpeed(statInfo.MoveSpeed);
        }

        if (IsMine && abilityPoint <= 0)
            uiPlayer.DeActiveAP();
    }

    private void SetStamina(int stamina)
    {
        if (stamina > this.stamina)
            this.cur_stamina += (stamina - this.stamina);
        this.stamina = stamina;
        if (IsMine)
            uiPlayer.SetStamina(cur_stamina, stamina, abilityPoint > 0);
    }

    private void SetPickSpeed(int pickSpeed)
    {
        this.pickSpeed = pickSpeed;
        if (IsMine)
            uiPlayer.SetPickSpeed(pickSpeed, abilityPoint > 0);
    }

    public int GetPickSpeed()
    {
        return this.pickSpeed;
    }

    private void SetMoveSpeed(float moveSpeed)
    {
        // 플레이어 오브젝트 속도 변경
        GetComponent<NavMeshAgent>().speed = 3.0f + (moveSpeed * 0.1f);

        /*this.moveSpeed = moveSpeed;*/
        if (IsMine)
            uiPlayer.SetMoveSpeed(moveSpeed, abilityPoint > 0);
    }

    public void LevelUp(int updatedLevel, int newTargetExp, int updatedExp, int updatedAbilityPoint)
    {
        level = updatedLevel;
        targetExp = newTargetExp;
        exp = updatedExp;

        PlayerManager.playerSaveData[PlayerId].Level = level;
        PlayerManager.playerSaveData[PlayerId].TargetExp = targetExp;
        PlayerManager.playerSaveData[PlayerId].Exp = exp;

        Debug.Log($"레벨업 응답 실행 {level}/ap{abilityPoint}/{exp}/{targetExp} isMine?{IsMine}");
        if (IsMine)
            uiPlayer.LevelUp(
                updatedLevel,
                newTargetExp,
                updatedExp,
                abilityPoint,
                updatedAbilityPoint
            );

        abilityPoint = updatedAbilityPoint;
        ActiveLevelUpEffect();
    }

    public void ActiveLevelUpEffect()
    {
        // 다른 플레이어 레벨 표시 변경
        // TownManager.Instance.SetPlayerLevel(playerId, level);

        // 레벨업 이펙트
        GameObject effect = transform.Find("LevelUpEffect").gameObject;
        if (effect.activeSelf)
        {
            effect.SetActive(false);
        }
        effect.SetActive(true); // 얘는 이펙트가 다 재생되면 알아서 꺼지는 넘입니다!
    }

    public void SetUI(UIPlayer uiPlayer)
    {
        this.uiPlayer = uiPlayer;
        this.uiPlayer.gameObject.SetActive(true);
    }

    public void Damaged(int damage)
    {
        curHp -= damage;
        ResourceManager.Instance.Instantiate("Effects", "FX_Shoot", transform.position);
        // PlayerManager.playerSaveData[PlayerId].CurHp -= damage;
        isImotal = true;
        // PlayerManager.playerSaveData[PlayerId].IsImotal = true;
        StartCoroutine(CoImotalTime(ImotalTime));

        if (IsMine)
        {
            uiPlayer.InitHp(curHp);
        }

        PartyMemberUI.instance.UpdateUI();
    }

    IEnumerator CoImotalTime(float time)
    {
        yield return new WaitForSeconds(time);
        isImotal = false;
        // PlayerManager.playerSaveData[PlayerId].IsImotal = false;
    }
}
