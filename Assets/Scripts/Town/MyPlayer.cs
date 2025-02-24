using System;
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

    /* 섬광탄 관련 변수 */
    private Transform throwPoint;
    private int throwPower = 15;
    private bool grenadeInput;
    private bool trapInput;
    private bool isThrow = false;
    private GameObject grenade;
    private GameObject trap;

    // PlayerInfo
    // private UIPlayer uiPlayer;
    // private int exp;
    // private int targetExp;
    // private int stamina;
    // private int cur_stamina;
    // private int pickSpeed;
    // private int moveSpeed;
    // private int abilityPoint;
    // private int cur_hp;
    // private int hp;

    void Awake()
    {
        eSystem = TownManager.Instance.E_System;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        throwPoint = transform.Find("ThrowPoint").transform;
        grenade = GetComponentInParent<Player>().grenade;
        trap = GetComponentInParent<Player>().trap;

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
        Throw();
        // CheckMove();
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
                targetPosition = rayHit.point;
            }
        }

        grenadeInput = Input.GetButtonDown("Grenade");
        trapInput = Input.GetButtonDown("Trap");
    }

    IEnumerator ExecuteEvery10Frames()
    {
        while (true)
        {
            yield return null; // 1 프레임 대기
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
        // animator.SetFloat(Constants.TownPlayerMove, distanceMoved * 100);

        if (distanceMoved > 0.01f)
        {
            SendLocationPacket();
        }

        lastPos = transform.position;
    }

    private void Throw()
    {
        if (!isThrow && (grenadeInput || trapInput))
        {
            isThrow = true;

            if (grenadeInput)
            {
                GameObject currentObj = Instantiate(
                    grenade,
                    throwPoint.position,
                    throwPoint.rotation
                );

                Rigidbody rigid = currentObj.GetComponent<Rigidbody>();

                Vector3 forceVec = throwPoint.forward * throwPower + throwPoint.up * throwPower / 2;

                rigid.AddForce(forceVec, ForceMode.VelocityChange);
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
        isThrow = false;
    }
    
    // public void SetStatInfo(StatInfo statInfo)
    // {
    //     level = statInfo.Level;
    //     exp = statInfo.Exp;
    //     targetExp = statInfo.TargetExp;
    //     cur_stamina = statInfo.CurStamina;
    //     stamina = statInfo.Stamina;
    //     pickSpeed = statInfo.PickSpeed;
    //     moveSpeed = statInfo.MoveSpeed;
    //     abilityPoint = statInfo.AbilityPoint;

    //     uiPlayer.SetStatInfo(statInfo);
    // }

    // public void SetExp(int updatedExp)
    // {
    //     Debug.Log("경험치 응답 실행");
    //     exp = updatedExp;
    //     if (exp > targetExp)
    //     {
    //         Debug.Log($"WTF! exp too much {exp}, {targetExp}");
    //         return;
    //     }
    //     Debug.Log($"updatedExp : {updatedExp}");

    //     uiPlayer.SetExp(updatedExp, targetExp);
    // }

    // public void InvestPoint(StatInfo statInfo)
    // {
    //     Debug.Log("능력치 투자자 응답 실행");
    //     abilityPoint = statInfo.AbilityPoint;
    //     uiPlayer.SetAbilityPoint(abilityPoint);
        
    //     if(statInfo.Stamina > stamina){
    //         StaminaUp();
    //     }
    //     if(statInfo.PickSpeed > pickSpeed){
    //         PickSpeedUp();
    //     }
    //     if(statInfo.MoveSpeed > moveSpeed){
    //         MoveSpeedUp();
    //     }

    //     if(abilityPoint <= 0) uiPlayer.DeActiveAP();
    // }

    // private void StaminaUp()
    // {
    //     stamina++;
    //     TownManager.Instance.UiPlayer.SetStamina(cur_stamina, stamina);
    // }

    // private void PickSpeedUp()
    // {
    //     pickSpeed++;
    //     TownManager.Instance.UiPlayer.SetPickSpeed(pickSpeed);
    // }

    // private void MoveSpeedUp()
    // {
    //     moveSpeed++;
    //     TownManager.Instance.UiPlayer.SetMoveSpeed(moveSpeed);
    // }

    // public void LevelUp(int updatedLevel, int newTargetExp, int updatedExp, int updatedAbilityPoint)
    // {
    //     level = updatedLevel;
    //     targetExp = newTargetExp;
    //     exp = updatedExp;

    //     Debug.Log($"레벨업 응답 실행 {level}/ap{abilityPoint}/{exp}/{targetExp}");
    //     TownManager.Instance.UiPlayer.LevelUp(updatedLevel, newTargetExp, updatedExp, abilityPoint, updatedAbilityPoint);
        
    //     abilityPoint = updatedAbilityPoint;
    // }
}
