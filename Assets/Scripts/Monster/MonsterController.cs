using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;

// 한 칸에 Extents (34.52, 0, 34.52)

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private int id;

    public int ID
    {
        get { return id; }
    }

    [SerializeField]
    private Transform monsterArea;
    private const float maxDistance = 34f;

    [SerializeField]
    private Transform target;
    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }

    private Vector3 _targetPosition;

    // private Rigidbody rigid;

    private Animator anim;

    private NavMeshAgent agent;

    private Coroutine coDefaultMove;

    private void Start()
    {
        // rigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //coDefaultMove = StartCoroutine(DefaultMove());
        agent.speed = 10f;
        agent.acceleration = 0;
        agent.angularSpeed = float.MaxValue;
        agent.isStopped = false;
        agent.stoppingDistance = 0;
        MonsterManager.Instance.AddMonster(this);
    }

    private void Update()
    {
        //Chase();
        transform.position = Vector3.Lerp(
            transform.position,
            _targetPosition,
            Time.deltaTime * 10f
        );
        Vector3 direction = _targetPosition - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * 150
        );
        //agent.destination = _targetPosition;
    }

    private void FixedUpdate()
    {
        //ControlLocation();
    }

    private void Chase() // 타겟 추적하는 함수
    {
        if (target != null)
        {
            agent.destination = target.position;
        }
        else
        {
            if (coDefaultMove == null && target == null)
            {
                coDefaultMove = StartCoroutine(DefaultMove());
            }
        }
    }

    private void ControlLocation() // 담당 구역 이탈 시 복귀하는 함수
    {
        float distance = Vector3.Distance(transform.position, monsterArea.position);
        if (distance >= maxDistance)
        {
            target = null;
            agent.destination = monsterArea.position;
        }
    }

    IEnumerator DefaultMove() // 평상 시 이동
    {
        while (true)
        {
            if (target != null)
            {
                coDefaultMove = null;
                yield break;
            }
            yield return new WaitForSeconds(3f);
            float x = Random.Range(-34, 34) + monsterArea.position.x;
            float z = Random.Range(-34, 34) + monsterArea.position.z;
            agent.destination = new Vector3(x, 0, z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<TempPlayer>();

            if (player && player.IsAlive)
            {
                anim.SetTrigger("Attack");
                player.Anim.SetTrigger("Attacked");
                player.IsAlive = false;
            }

            target = null;
        }
    }

    public void SetPosition(Vector3 position)
    {
        _targetPosition = position;
    }
}
