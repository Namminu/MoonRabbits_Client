using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class SkillObj : MonoBehaviour
{
    public enum SkillType
    {
        grenade = 1,
        trap = 2,
    }

    public SkillType type;

    private int casterId;
    public int CasterId
    {
        get { return casterId; }
        set { casterId = value; }
    }

    private MeshRenderer mesh;
    private Rigidbody rigid;
    private TrailRenderer trail;
    private GameObject effect;

    private const float lifeTime = 5f;
    private const float explosionRange = 5f;

    private bool isActive = false;
    public bool IsActive => isActive;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        effect = transform.Find("Effect").gameObject;

        if (type == SkillType.trap)
        {
            SetTrapColor();
        }

        StartCoroutine(SetDestroyTimer());
    }

    public void SetTrapColor()
    {
        if (casterId == GameManager.Instance.MPlayer.PlayerId)
        {
            GetComponentInChildren<Light>().color = new Color(0.5f, 1.0f, 0.5f);
            // GetComponentInChildren<Light>().color = Color.green;
            // GetComponentInChildren<Light>().color = new Color32(144, 238, 144, 255);
        }
    }

    IEnumerator SetDestroyTimer()
    {
        switch (type)
        {
            case SkillType.grenade:
                Destroy(gameObject, lifeTime);
                break;
            case SkillType.trap:
                yield return new WaitForSeconds(lifeTime * 4);

                if (casterId == GameManager.Instance.MPlayer.PlayerId)
                {
                    var pkt = new C2SRemoveTrap
                    {
                        TrapInfo = new TrapInfo
                        {
                            CasterId = casterId,
                            Pos = new Vec3
                            {
                                X = Mathf.Round(transform.position.x * 10f),
                                Y = 0,
                                Z = Mathf.Round(transform.position.z * 10f),
                            },
                        },
                    };

                    GameManager.Network.Send(pkt);
                }
                break;
        }
    }

    public void RemoveThis()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive && type == SkillType.grenade && collision.gameObject.CompareTag("Ground"))
            StartCoroutine(nameof(Explode));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (
            !isActive
            && type == SkillType.trap
            && other.gameObject.CompareTag("Player")
            && casterId != other.GetComponent<Player>().PlayerId
        )
            StartCoroutine(ActivateTrap(other.gameObject));
    }

    IEnumerator Explode()
    {
        isActive = true;

        yield return new WaitForSeconds(0.5f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        yield return new WaitForSeconds(0.5f);
        mesh.enabled = false;
        trail.enabled = false;

        yield return new WaitForSeconds(0.1f);
        effect.SetActive(true);

        if (casterId == GameManager.Instance.MPlayer.PlayerId)
        {
            RaycastHit[] rayHits = Physics.SphereCastAll(
                transform.position, // 이 위치에서 (슈류탄 현 위치)
                explosionRange, // 설정한 반경의 구체 Ray를
                Vector3.up, // 위 방향으로
                0, // 현 위치와 이격 없이 쏘는데
                LayerMask.GetMask("Monster", "Player") // 레이어에 맞는 오브젝트들을 배열로 반환
            );

            if (rayHits.Count() > 0)
            {
                var pkt = new C2SStun();
                pkt.SkillType = (int)type;

                foreach (RaycastHit hitObj in rayHits)
                {
                    var target = hitObj.transform.gameObject;

                    if (target.CompareTag("Monster"))
                    {
                        pkt.MonsterIds.Add(target.GetComponent<MonsterController>().ID);
                    }
                    else if (target.CompareTag("Player"))
                    {
                        pkt.PlayerIds.Add(target.GetComponent<Player>().PlayerId);
                    }
                }

                GameManager.Network.Send(pkt);
            }
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    IEnumerator ActivateTrap(GameObject target)
    {
        yield return null;
        isActive = true;

        var rune = transform.Find("Rune").gameObject;
        rune.SetActive(false);
        effect.SetActive(true);

        Player targetPlayer = target.GetComponent<Player>();

        var stunPkt = new C2SStun { SkillType = (int)type };
        stunPkt.PlayerIds.Add(targetPlayer.PlayerId);

        GameManager.Network.Send(stunPkt);

        yield return new WaitForSeconds(2f); // 스턴 패킷 답장 기다려야해...
        isActive = false;

        yield return new WaitUntil(() => !targetPlayer.IsStun);

        var removePkt = new C2SRemoveTrap
        {
            TrapInfo = new TrapInfo
            {
                CasterId = casterId,
                Pos = new Vec3
                {
                    X = Mathf.Round(transform.position.x * 10f),
                    Y = 0,
                    Z = Mathf.Round(transform.position.z * 10f),
                },
            },
        };

        GameManager.Network.Send(removePkt);
    }
}
