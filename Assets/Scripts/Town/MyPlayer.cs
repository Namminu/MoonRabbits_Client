using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MyPlayer : MonoBehaviour
{
    public Player P;

    [SerializeField]
    public static MyPlayer instance { get; private set; }
    private NavMeshAgent agent;
    public NavMeshAgent NavAgent
    {
        get => agent;
    }
    private RaycastHit rayHit;
    public EventSystem eSystem;
    private Animator anim;
    public Animator Anim
    {
        get => anim;
    }
    private Vector3 lastPos;
    private Vector3 targetPosition;
    public Vector3 TargetPos => targetPosition;
    private Vector3 lastTargetPosition;
    private readonly List<int> animHash = new List<int>();
    private int frameCount = 0;
    private const int targetFrames = 10; // 10 프레임마다 실행

    /* 감정표현 관련 */
    public bool isEmoting;
    private bool happyInput;
    private bool sadInput;
    private bool greetingInput;
    private EmoteManager emoteManager;

    /* 스킬 관련 */
    public GameObject grenade;
    public GameObject trap;
    private bool grenadeInput;
    private bool trapInput;
    private bool recallInput;

    private SkillManager skillManager;
    public SkillManager SkillManager => skillManager;

    /* 상호작용 관련 */
    public int currentEquip = 0;

    private bool equipChangeInput;
    private bool interactInput;
    private InteractManager interactManager;
    public InteractManager InteractManager => interactManager;

    void Awake()
    {
        instance = this;

        P = GetComponent<Player>();
        eSystem = TownManager.Instance.E_System;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        grenade = GetComponentInParent<Player>().grenade;
        trap = GetComponentInParent<Player>().trap;

        InitializeCamera();
        lastPos = transform.position;

        LoadAnimationHashes();

        skillManager = GetComponentInChildren<SkillManager>();
        interactManager = GetComponentInChildren<InteractManager>();
        emoteManager = GetComponent<EmoteManager>();
    }

    void Start()
    {
        StartCoroutine(ExecuteEvery10Frames());
    }

    void Update()
    {
        HandleInput();
        Emote();
        ThrowGrenade();
        SetTrap();
        Recall();
        EquipChange();
        Interact();
    }

    private void InitializeCamera()
    {
        Camera.main.gameObject.GetComponent<QuarterView>().target = transform;

        // var freeLook = TownManager.Instance.FreeLook;
        // freeLook.Follow = transform;
        // freeLook.LookAt = transform;
        // freeLook.gameObject.SetActive(true);
    }

    private void LoadAnimationHashes()
    {
        animHash.Add(Constants.TownPlayerAnim1);
        animHash.Add(Constants.TownPlayerAnim2);
        animHash.Add(Constants.TownPlayerAnim3);
    }

    // 왼쪽 마우스 버튼을 눌렀을 때 (Input.GetMouseButtonDown(0))
    // UI 요소를 클릭하지 않았을 경우만 실행 (!eSystem.IsPointerOverGameObject())
    // 마우스 위치에서 3D 광선(Ray)을 발사하여 충돌 지점을 찾음 (Physics.Raycast(...))
    // 충돌한 위치로 NavMeshAgent를 이동시킴 (agent.SetDestination(rayHit.point);
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !eSystem.IsPointerOverGameObject())
        {
            int layerMask = 1 << LayerMask.NameToLayer("Ground");

            if (
                Physics.Raycast(
                    Camera.main.ScreenPointToRay(Input.mousePosition),
                    out rayHit,
                    Mathf.Infinity,
                    layerMask
                )
            )
            {
                isEmoting = false;
                targetPosition = rayHit.point;
            }
        }

        happyInput = Input.GetKeyDown(KeyCode.Alpha1);
        sadInput = Input.GetKeyDown(KeyCode.Alpha2);
        greetingInput = Input.GetKeyDown(KeyCode.Alpha3);
        grenadeInput = Input.GetKeyDown(KeyCode.Q);
        trapInput = Input.GetKeyDown(KeyCode.E);
        recallInput = Input.GetKeyDown(KeyCode.T);
        interactInput = Input.GetKeyDown(KeyCode.F);
        equipChangeInput = Input.GetKeyDown(KeyCode.R);
    }

    IEnumerator ExecuteEvery10Frames()
    {
        while (true)
        {
            yield return null;
            frameCount++;

            CheckMove();

            // 마지막으로 전송했던 좌표(lastTargetPosition)와 달라졌을 때에만 실행
            if (frameCount >= targetFrames && targetPosition != lastTargetPosition)
            {
                frameCount = 0;
                MoveAndSendMovePacket();
            }
        }
    }

    private void MoveAndSendMovePacket()
    {
        if (P.IsStun)
            return;

        // 플레이어 이동시키기
        agent.SetDestination(targetPosition);

        // 마지막으로 전송했던 좌표 기억해두기
        lastTargetPosition = targetPosition;

        // 패킷 전송
        var movePacket = new C2SPlayerMove
        {
            StartPosX = transform.position.x,
            StartPosY = transform.position.y,
            StartPosZ = transform.position.z,
            TargetPosX = targetPosition.x,
            TargetPosY = targetPosition.y,
            TargetPosZ = targetPosition.z,
        };
        GameManager.Network.Send(movePacket);
    }

    private void SendLocationPacket()
    {
        var tr = new TransformInfo
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            Rot = transform.eulerAngles.y,
        };

        var locationPacket = new C2SPlayerLocation { Transform = tr };
        GameManager.Network.Send(locationPacket);
    }

    public void ExecuteAnimation(int animIdx)
    {
        if (animIdx < 0 || animIdx >= animHash.Count)
            return;

        int animKey = animHash[animIdx];
        agent.SetDestination(transform.position);

        var animationPacket = new C2SAnimation { AnimCode = animKey };
        Debug.Log($"감정표현?? : {animationPacket}");
        GameManager.Network.Send(animationPacket);
    }

    private void CheckMove()
    {
        float distanceMoved = Vector3.Distance(lastPos, transform.position);
        // animator.SetFloat(Constants.TownPlayerMove, distanceMoved * 100);

        if (distanceMoved > 0.01f)
        {
            SendLocationPacket();
            lastPos = transform.position;
        }
    }

    private void Emote()
    {
        if (!P.IsStun && !isEmoting)
        {
            if (happyInput)
            {
                isEmoting = true;
                emoteManager.event1.Invoke();
            }
            else if (sadInput)
            {
                isEmoting = true;
                emoteManager.event2.Invoke();
            }
            else if (greetingInput)
            {
                isEmoting = true;
                emoteManager.event3.Invoke();
            }
        }
    }

    private void ThrowGrenade()
    {
        if (grenadeInput && !P.IsStun)
        {
            skillManager.eventQ.Invoke();

            float cooltime = 5;

            if (SceneManager.GetActiveScene().name == "Sector1")
                S1Manager.Instance.UiPlayer.QSkillCool(cooltime);
            else if (SceneManager.GetActiveScene().name == "Sector2")
                S2Manager.Instance.UiPlayer.WSkillCool(cooltime);
        }
    }

    private void SetTrap()
    {
        if (trapInput && !P.IsStun)
            skillManager.eventE.Invoke();
    }

    private void Recall()
    {
        if (recallInput && !P.IsStun)
            skillManager.eventT.Invoke();
    }

    private void EquipChange()
    {
        if (equipChangeInput && !P.IsStun)
            interactManager.eventR.Invoke();
    }

    private void Interact()
    {
        if (interactInput && !P.IsStun)
            interactManager.eventF.Invoke();
    }

    public void Stun(float timer)
    {
        transform.Find("StunEffect").gameObject.SetActive(true);
        NavAgent.velocity = Vector3.zero;
        NavAgent.ResetPath();
        NavAgent.isStopped = true;
        Invoke(nameof(StunOut), timer);
    }

    private void StunOut()
    {
        transform.Find("StunEffect").gameObject.SetActive(false);
        NavAgent.isStopped = false;
    }
}
