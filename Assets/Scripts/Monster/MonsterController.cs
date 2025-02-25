using Google.Protobuf.Protocol;
using UnityEngine;

// 한 칸에 Extents (34.52, 0, 34.52)

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private int id;

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
        C2SMonsterCollision collisionPacket = new C2SMonsterCollision();
        collisionPacket.CollisionInfo = new CollisionInfo();
        var myPos = transform.position;
        var targetPos = playerCollider.transform.position;
        collisionPacket.MonsterId = id;

        collisionPacket.CollisionInfo.Height1 = _collider.height;
        collisionPacket.CollisionInfo.Position1 = new Vec3() { X = myPos.x, Y = myPos.y, Z = myPos.z };
        collisionPacket.CollisionInfo.Radius1 = _collider.radius;
        collisionPacket.CollisionInfo.Height2 = playerCollider.height;
        collisionPacket.CollisionInfo.Position2 = new Vec3() { X = targetPos.x, Y = targetPos.y, Z = targetPos.z };
        collisionPacket.CollisionInfo.Radius2 = playerCollider.radius;

        GameManager.Network.Send(collisionPacket);


    }

    public void SetPosition(Vector3 position)
    {
        _targetPosition = position;
    }
}
