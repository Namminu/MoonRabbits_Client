
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MyPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private RaycastHit rayHit;
    private EventSystem eSystem;
    private Animator animator;
    private Vector3 lastPos;

    private readonly List<int> animHash = new List<int>();

    void Awake()
    {
        eSystem = TownManager.Instance.E_System;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        InitializeCamera();
        lastPos = transform.position;

        LoadAnimationHashes();
    }

    void Update()
    {
        HandleInput();
        CheckMove();
    }

    private void InitializeCamera()
    {
        var freeLook = TownManager.Instance.FreeLook;
        freeLook.Follow = transform;
        freeLook.LookAt = transform;
        freeLook.gameObject.SetActive(true);
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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit))
            {
                agent.SetDestination(rayHit.point);
                var movePacket = new C_Move
                {
                    StartPosX = transform.position.x,
                    StartPosY = transform.position.y,
                    StartPosZ = transform.position.z,
                    TargetPosX = rayHit.point.x,
                    TargetPosY = rayHit.point.y,
                    TargetPosZ = rayHit.point.z
                };

                GameManager.Network.Send(movePacket);
            }
        }
    }

    public void ExecuteAnimation(int animIdx)
    {
        if (animIdx < 0 || animIdx >= animHash.Count)
            return;

        int animKey = animHash[animIdx];
        agent.SetDestination(transform.position);

        var animationPacket = new C_Animation { AnimCode = animKey };
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
}