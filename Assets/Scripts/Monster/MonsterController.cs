using Google.Protobuf.Protocol;
using UnityEngine;

// 한 칸에 Extents (34.52, 0, 34.52)

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private int id;

    [SerializeField] private int sectorCode = 2;

    public int ID { get { return id; } }

    private Vector3 _targetPosition;

    private Rigidbody rigid;

    private Animator anim;

    [SerializeField] private CapsuleCollider _collider;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        MonsterManager.Instance.AddMonster(this);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10f);
        Vector3 direction = _targetPosition - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 150);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거");
        //Debug.Log($"감지되었던것이 {other} 이고 해당 태그는 {other.tag}");
        if (other.CompareTag("Player") == false) return;
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
        collisionInfo.MyPosition = new Vec3() { X = myPos.x, Y = myPos.y, Z = myPos.z };
        collisionInfo.MyHeight = _collider.height;
        collisionInfo.MyRadius = _collider.radius;
        collisionInfo.TargetType = 1;
        collisionInfo.TargetId = targetId;
        collisionInfo.TargetPosition = new Vec3() { X = targetPos.x, Y = targetPos.y, Z = targetPos.z };
        collisionInfo.TargetHeight = playerCollider.height;
        collisionInfo.TargetRadius = playerCollider.radius;
        collisionPacket.CollisionInfo = collisionInfo;

        GameManager.Network.Send(collisionPacket);


    }

    public void SetPosition(Vector3 position)
    {
        _targetPosition = position;
    }
}
