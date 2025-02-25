using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class TempPlayer : MonoBehaviour
{
    private int id = 555;
    public int ID => id;

    [SerializeField]
    private NavMeshAgent agent;
    public NavMeshAgent NavAgent
    {
        get => agent;
    }
    private RaycastHit rayHit;
    private Animator anim;
    public Animator Anim
    {
        get => anim;
    }
    private Vector3 targetPosition;

    public bool IsAlive = false;

    /* 스킬 관련 변수 */
    public GameObject grenade;
    public GameObject trap;
    private bool grenadeInput;
    private bool trapInput;
    private bool recallInput;
    private TempSkillManager skillManager;

    /* 상호작용 관련 변수 */
    public GameObject axe;
    public GameObject pickAxe;
    public int currentEquip = (int)EquipState.none;

    public enum EquipState
    {
        none = 0,
        axe = 1,
        pickAxe = 2,
    }

    private bool equipChangeInput;
    private bool interactInput;
    private TempInteractionManager interactManager;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        grenade = GetComponentInParent<Player>().grenade;
        trap = GetComponentInParent<Player>().trap;
        axe = GetComponentInParent<Player>().axe;
        pickAxe = GetComponentInParent<Player>().pickAxe;

        // 장애물 회피 설정 낮추기(서버와 경로를 최대한 비슷하게 만들기 위함)
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.avoidancePriority = 0; // 회피 우선순위 낮게 설정

        InitializeCamera();

        skillManager = GetComponentInChildren<TempSkillManager>();
        interactManager = GetComponentInChildren<TempInteractionManager>();
    }

    void Update()
    {
        HandleInput();
        ThrowGrenade();
        SetTrap();
        Recall();
        EquipChange();
        Interact();
    }

    private void InitializeCamera()
    {
        Camera.main.gameObject.GetComponent<QuarterView>().target = transform;
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
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
                agent.SetDestination(targetPosition);
            }
        }

        grenadeInput = Input.GetKeyDown(KeyCode.Q);
        trapInput = Input.GetKeyDown(KeyCode.E);
        recallInput = Input.GetKeyDown(KeyCode.T);
        interactInput = Input.GetKeyDown(KeyCode.F);
        equipChangeInput = Input.GetKeyDown(KeyCode.R);
    }

    private void ThrowGrenade()
    {
        if (grenadeInput)
            skillManager.eventQ.Invoke();
    }

    private void SetTrap()
    {
        if (trapInput)
            skillManager.eventE.Invoke();
    }

    private void Recall()
    {
        if (recallInput)
            skillManager.eventT.Invoke();
    }

    private void Interact()
    {
        if (interactInput)
            interactManager.eventF.Invoke();
    }

    private void EquipChange()
    {
        if (equipChangeInput)
            interactManager.eventR.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            var bounds = collision.gameObject.GetComponent<MeshCollider>().bounds;
            Debug.Log($"땅 크기 ??! {bounds}");
        }
        if (collision.gameObject.CompareTag("Resource")) { }
    }
}
