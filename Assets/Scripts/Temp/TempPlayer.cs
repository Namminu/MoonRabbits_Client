using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class TempPlayer : MonoBehaviour
{
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

    /* 섬광탄 관련 변수 */
    private Transform throwPoint;
    private int throwPower = 15;
    private bool grenadeInput;
    private bool trapInput;
    private bool isThrow = false;
    private GameObject grenade;
    private GameObject trap;

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
    public bool InteractInput
    {
        get => interactInput;
    }
    private TempInteractionManager interactManager;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        throwPoint = transform.Find("ThrowPoint").transform;
        grenade = GetComponentInParent<Player>().grenade;
        trap = GetComponentInParent<Player>().trap;
        axe = GetComponentInParent<Player>().axe;
        pickAxe = GetComponentInParent<Player>().pickAxe;

        // 장애물 회피 설정 낮추기(서버와 경로를 최대한 비슷하게 만들기 위함)
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.avoidancePriority = 0; // 회피 우선순위 낮게 설정

        InitializeCamera();

        interactManager = GetComponentInChildren<TempInteractionManager>();
    }

    void Update()
    {
        HandleInput();
        Throw();
        EquipChange();
        Interact();
    }

    private void InitializeCamera()
    {
        Camera.main.gameObject.GetComponent<TempCamera>().target = transform;
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

        grenadeInput = Input.GetButtonDown("Grenade");
        trapInput = Input.GetButtonDown("Trap");
        interactInput = Input.GetButtonDown("Interact");
        equipChangeInput = Input.GetButtonDown("EquipChange");
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

    private void EquipChange()
    {
        if (equipChangeInput)
            interactManager.eventR.Invoke();
    }

    private void Interact()
    {
        if (interactInput)
            interactManager.eventF.Invoke();
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
