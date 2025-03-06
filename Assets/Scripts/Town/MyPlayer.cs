using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MyPlayer : MonoBehaviour
{
    private Player player;

    [SerializeField]
    public static MyPlayer instance { get; private set; }
    private NavMeshAgent agent;
    public NavMeshAgent NavAgent
    {
        get => agent;
    }
    private RaycastHit rayHit;
    public EventSystem eSystem;
    private bool isReadyESystem = false;
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

    private LineRenderer _lineRenderer;
    private Camera _cam;
    private float zoomSpeed = 20f;
    private float minFOV = 20f;
    private float maxFOV = 120f;

    void Awake()
    {
        instance = this;

        player = GetComponent<Player>();
        StartCoroutine(nameof(WaitForESystem));

        agent = GetComponent<NavMeshAgent>();
        _lineRenderer = GetComponent<LineRenderer>();
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

    private void Start()
    {
        StartCoroutine(ExecuteEvery0_1Seconds());
    }

    private void Update()
    {
        HandleInput();
        Emote();
        ThrowGrenade();
        SetTrap();
        Recall();
        EquipChange();
        Interact();
    }

    private void LateUpdate()
    {
        CheckMove();
        PathFinding();
        ScreenScrollZoom();
    }

    void PathFinding()
    {
        if (agent.pathPending)
            return;

        var corners = agent.path.corners;
        _lineRenderer.positionCount = corners.Length;

        _lineRenderer.SetPositions(corners);
    }

    void ScreenScrollZoom()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel"); // 마우스 휠 입력

        if (_cam == null)
            _cam = Camera.main;

        // 줌 조정
        // 줌 조정
        if (scrollData != 0f)
        {
            _cam.fieldOfView -= scrollData * zoomSpeed; // FOV 조정
            _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, minFOV, maxFOV); // 최소 및 최대 FOV 제한
        }
    }

    private void InitializeCamera()
    {
        Camera.main.gameObject.GetComponent<QuarterView>().target = transform;
    }

    IEnumerator WaitForESystem()
    {
        while (eSystem == null)
        {
            eSystem = GameManager.Instance.SManager.ESystem;
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitUntil(() => eSystem != null);
        isReadyESystem = true;
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
        if (player.IsStun || !isReadyESystem)
            return;

        if (Input.GetMouseButtonDown(0) && !eSystem.IsPointerOverGameObject())
        {
            if (
                Physics.Raycast(
                    Camera.main.ScreenPointToRay(Input.mousePosition),
                    out rayHit,
                    Mathf.Infinity,
                    LayerMask.GetMask("Ground")
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

    IEnumerator ExecuteEvery0_1Seconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 0.1초마다 실행

            // 마지막으로 전송했던 좌표(lastTargetPosition)와 달라졌을 때에만 실행
            if (targetPosition != lastTargetPosition)
            {
                MoveAndSendMovePacket();
            }
        }
    }

    private void MoveAndSendMovePacket()
    {
        if (player.IsStun)
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

    // public void ExecuteAnimation(int animIdx)
    // {
    //     if (animIdx < 0 || animIdx >= animHash.Count)
    //         return;

    //     int animKey = animHash[animIdx];
    //     agent.SetDestination(transform.position);

    //     var animationPacket = new C2SEmote { AnimCode = animKey };
    //     Debug.Log($"감정표현?? : {animationPacket}");
    //     GameManager.Network.Send(animationPacket);
    // }

    private void CheckMove()
    {
        float distanceMoved = Vector3.Distance(lastPos, transform.position);
        anim.SetFloat("Move", distanceMoved * 10);

        if (distanceMoved > 0.01f)
        {
            SendLocationPacket();
            lastPos = transform.position;
        }
    }

    private void Emote()
    {
        if (!isEmoting)
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
        if (grenadeInput && GameManager.Instance.CurrentSector != 100)
        {
            skillManager.eventQ.Invoke();

            float coolTime = 5;

            if (SceneManager.GetActiveScene().name == "Sector1")
                S1Manager.Instance.UiPlayer.QSkillCool(coolTime);
            else if (SceneManager.GetActiveScene().name == "Sector2")
                S2Manager.Instance.UiPlayer.WSkillCool(coolTime);
        }
    }

    private void SetTrap()
    {
        if (trapInput && GameManager.Instance.CurrentSector != 100)
            skillManager.eventE.Invoke();
    }

    private void Recall()
    {
        if (recallInput && GameManager.Instance.CurrentSector != 100)
            skillManager.eventT.Invoke();
    }

    private void EquipChange()
    {
        if (equipChangeInput && GameManager.Instance.CurrentSector != 100)
            interactManager.eventR.Invoke();
    }

    private void Interact()
    {
        if (interactInput)
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
