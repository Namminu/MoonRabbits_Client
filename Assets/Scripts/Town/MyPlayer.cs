using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MyPlayer : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    private RaycastHit rayHit;
    private EventSystem eSystem;
    private Animator animator;
    private Vector3 lastPos;
    private Vector3 targetPosition;
    private Vector3 lastTargetPosition;
    private readonly List<int> animHash = new List<int>();
    private int frameCount = 0;
    private const int targetFrames = 10; // 10 프레임마다 실행

    void Awake()
    {
        eSystem = TownManager.Instance.E_System;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 장애물 회피 설정 낮추기(서버와 경로를 최대한 비슷하게 만들기 위함)
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.avoidancePriority = 0; // 회피 우선순위 낮게 설정

        InitializeCamera();
        lastPos = transform.position;

        LoadAnimationHashes();
    }

    void Start()
    {
        StartCoroutine(ExecuteEvery10Frames());
    }

    void Update()
    {
        HandleInput();
        CheckMove();
    }

    private void InitializeCamera()
    {
        Camera.main.gameObject.GetComponent<TempCamera>().target = transform;

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
        Debug.Log($"마우스 눌림? : {Input.GetMouseButtonDown(0)}");
        Debug.Log($"이벤트시스템? : {!eSystem.IsPointerOverGameObject()}");
        if (Input.GetMouseButtonDown(0) && !eSystem.IsPointerOverGameObject())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit))
            {
                targetPosition = rayHit.point;
            }
        }
    }

    IEnumerator ExecuteEvery10Frames()
    {
        while (true)
        {
            yield return null; // 1 프레임 대기
            frameCount++;

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
        GameManager.Network.Send(animationPacket);
    }

    private void CheckMove()
    {
        float distanceMoved = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, distanceMoved * 100);

        if (distanceMoved > 0.01f)
        {
            SendLocationPacket();
        }

        lastPos = transform.position;
    }

    public void SendMonsterLocation()
    {
        var tr = new TransformInfo
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            Rot = transform.eulerAngles.y,
        };

        var locationPacket = new C2SMonsterLocation { Transform = tr };
        GameManager.Network.Send(locationPacket);
    }
}
