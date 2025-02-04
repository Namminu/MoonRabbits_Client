
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

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !eSystem.IsPointerOverGameObject())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit))
            {
                agent.SetDestination(rayHit.point);
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
            SendMovePacket();
        }

        lastPos = transform.position;
    }

    private void SendMovePacket()
    {
        var tr = new TransformInfo
        {
            PosX = transform.position.x,
            PosY = transform.position.y,
            PosZ = transform.position.z,
            Rot = transform.eulerAngles.y
        };

        var movePacket = new C_Move { Transform = tr };
        GameManager.Network.Send(movePacket);
    }
}