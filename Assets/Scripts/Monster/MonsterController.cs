using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

// 한 칸에 Extents (34.52, 0, 34.52)

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private int id;

    [SerializeField]
    private int sectorCode;

    public int ID
    {
        get { return id; }
    }

    [SerializeField]
    private Transform monsterArea;
    private const float maxDistance = 34f;
    private CapsuleCollider _collider;

    [SerializeField]
    private Transform target;
    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }

    private Vector3 _targetPosition;

    private Rigidbody rigid;

    private Animator anim;

    private NavMeshAgent agent;
    public NavMeshAgent NavAgent => agent;

    private Coroutine coDefaultMove;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
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
        if (other.CompareTag("Player") == false)
            return;
        if (_isAttack)
            return;
        StartCoroutine(CoMonsterAttackCoolTime());
        Debug.Log("플레이어가 몬스터와 충돌하였다.");
        CapsuleCollider playerCollider = other.GetComponent<CapsuleCollider>();
        C2SCollision collisionPacket = new C2SCollision();
        var collisionInfo = new CollisionInfo();
        var myPos = transform.position;
        var targetPos = playerCollider.transform.position;
        var targetId = playerCollider.GetComponent<Player>().PlayerId;
        collisionInfo.SectorCode = sectorCode;
        collisionInfo.MyType = 2;
        collisionInfo.MyId = id;
        collisionInfo.MyPosition = new Vec3()
        {
            X = myPos.x,
            Y = myPos.y,
            Z = myPos.z,
        };
        collisionInfo.MyHeight = _collider.height;
        collisionInfo.MyRadius = _collider.radius;
        collisionInfo.TargetType = 1;
        collisionInfo.TargetId = targetId;
        collisionInfo.TargetPosition = new Vec3()
        {
            X = targetPos.x,
            Y = targetPos.y,
            Z = targetPos.z,
        };
        collisionInfo.TargetHeight = playerCollider.height;
        collisionInfo.TargetRadius = playerCollider.radius;
        collisionPacket.CollisionInfo = collisionInfo;

        GameManager.Network.Send(collisionPacket);
    }

    public void SetPosition(Vector3 position)
    {
        _targetPosition = position;
    }

    public void Stun(float timer)
    {
        Debug.Log($"걸린 녀석 : {ID}");
        NavAgent.velocity = Vector3.zero;
        NavAgent.ResetPath();
        NavAgent.isStopped = true;
        Invoke(nameof(StunOut), timer);
    }

    private void StunOut()
    {
        NavAgent.isStopped = false;
    }

    public void SetCollision(CollisionPushInfo info)
    {
        var type = info.TargetType;
        Debug.Log($"타겟 타입??! : {type}");
        switch (type)
        {
            //충돌한 자가 플레이어면
            case 1:
                anim.SetTrigger("Attack");
                break;
            default:
                break;
        }
    }
}
