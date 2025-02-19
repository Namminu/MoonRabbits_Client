using System.Collections;
using System.Net;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

public class TempPlayer : MonoBehaviour
{
    public Avatar Avatar { get; private set; }
    private Animator animator;
    public Animator Anim
    {
        get => animator;
    }
    Vector3 moveVec;
    float hAxis;
    float vAxis;
    public float moveSpeed; // 이동 속도

    private bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
        set { isAlive = value; }
    }

    [Header("Skill Settings")]
    /* 섬광탄 관련 변수 */
    public Transform throwPoint;
    public int throwPower;
    private bool grenadeInput;
    private bool trapInput;
    private bool isThrow = false;
    public GameObject grenade;
    public GameObject trap;

    private void Start()
    {
        Avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 인풋 키 받는 함수 (스킬이나 조작 추가시 여기에 추가)
        GetInput();
        Turn();
        Move();
        Throw();
    }

    public void PlayAnimation(int animCode)
    {
        animator?.SetTrigger(animCode);
    }

    private void GetInput()
    {
        if (!isAlive)
        {
            hAxis = 0;
            vAxis = 0;
            return;
        }
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        grenadeInput = Input.GetButtonDown("Grenade");
        trapInput = Input.GetButtonDown("Trap");
    }

    public void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveSpeed * Time.deltaTime * moveVec;

        animator.SetFloat("Move", 1);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    private void Throw()
    {
        if (!isThrow && (grenadeInput || trapInput))
        {
            Debug.Log("Throw 진입");
            isThrow = true;

            if (grenadeInput)
            {
                Debug.Log("Grenade 진입");
                GameObject currentObj = Instantiate(
                    grenade,
                    throwPoint.position,
                    throwPoint.rotation
                );

                Rigidbody rigid = currentObj.GetComponent<Rigidbody>();

                Vector3 forceVec = throwPoint.forward * throwPower + throwPoint.up * throwPower / 2;

                rigid.AddForce(forceVec, ForceMode.Impulse);
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
        Debug.Log("ThrowEnd 진입");
        isThrow = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            var bounds = collision.gameObject.GetComponent<MeshCollider>().bounds;
            Debug.Log($"땅 크기 ??! {bounds}");
        }
    }
}
