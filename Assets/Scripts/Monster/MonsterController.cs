using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

// 한 칸에 Extents (34.52, 0, 34.52)

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private int id;

    [SerializeField]
    private int sectorCode;

    private bool _isAttack = false;

    public int ID
    {
        get { return id; }
    }
    private CapsuleCollider _collider;

    private Vector3 _targetPosition;

    private Animator anim;
    private NavMeshAgent _agent;

    private void Start()
    {
        anim = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _agent = GetComponent<NavMeshAgent>();
        MonsterManager.Instance.AddMonster(this);
    }

    private void Update()
    {
        // transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 5f);
        // Vector3 direction = _targetPosition - transform.position;
        // direction.y = 0;
        // Quaternion targetRotation = Quaternion.LookRotation(direction);
        // transform.rotation = Quaternion.Slerp(
        //     transform.rotation,
        //     targetRotation,
        //     Time.deltaTime * 150
        // );
    }

    IEnumerator CoMonsterAttackCoolTime()
    {
        _isAttack = true;
        yield return new WaitForSeconds(2f);
        _isAttack = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") == false)
            return;
        if (_isAttack)
            return;
        var player = other.GetComponent<Player>();
        if (player.GetIsImotal)
            return;
        StartCoroutine(CoMonsterAttackCoolTime());
        Debug.Log("플레이어가 몬스터와 충돌하였다.");
        CapsuleCollider playerCollider = other.GetComponent<CapsuleCollider>();
        C2SCollision collisionPacket = new C2SCollision();
        var collisionInfo = new CollisionInfo();
        var myPos = transform.position;
        var targetPos = playerCollider.transform.position;
        var targetId = player.PlayerId;
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
        //_targetPosition = position;
        _agent.SetDestination(position);
    }

    public void SetCollision(CollisionPushInfo info)
    {
        var type = info.TargetType;
        if (info.HasCollision == false)
            return;
        switch (type)
        {
            //충돌한 자가 플레이어면
            case 1:
                var player = GameManager.Instance.GetPlayer(info.TargetId);
                if (player != null && player.CurHp > 0)
                {
                    anim.SetTrigger("Attack");
                    player.Damaged(1);
                }
                break;
            default:
                break;
        }
    }
}
