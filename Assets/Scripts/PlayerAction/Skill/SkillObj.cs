using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class SkillObj : MonoBehaviour
{
    public enum ObjType
    {
        grenade,
        trap,
    }

    public ObjType type;

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
    private const float stunTimer = 5f;

    private bool isActive = false;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        effect = transform.Find("Effect").gameObject;

        DestroyObj();
    }

    private void DestroyObj()
    {
        switch (type)
        {
            case ObjType.grenade:
                Destroy(gameObject, lifeTime);
                break;
            case ObjType.trap:
                Destroy(gameObject, lifeTime * 4);
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive && type == ObjType.grenade && collision.gameObject.CompareTag("Ground"))
            StartCoroutine(nameof(Explode));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive && type == ObjType.trap && other.gameObject.CompareTag("Player"))
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
        Debug.Log($"던진 사람 : {casterId}");
        Debug.Log($"게임매니저 아이디 : {GameManager.Instance.PlayerId}");
        if (casterId == GameManager.Instance.PlayerId)
        {
            var pkt = new C2SStun();
            pkt.SkillType = (int)type;

            RaycastHit[] rayHits = Physics.SphereCastAll(
                transform.position, // 이 위치에서 (슈류탄 현 위치)
                explosionRange, // 설정한 반경의 구체 Ray를
                Vector3.up, // 위 방향으로
                0, // 현 위치와 이격 없이 쏘는데
                LayerMask.GetMask("Monster", "Player") // 레이어에 맞는 오브젝트들을 배열로 반환
            );

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

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    IEnumerator ActivateTrap(GameObject target)
    {
        isActive = true;

        var rune = transform.Find("Rune").gameObject;
        rune.SetActive(false);
        effect.SetActive(true);

        target.GetComponent<TempPlayer>().Stun(stunTimer); // 나중에 player로 수정

        yield return new WaitForSeconds(stunTimer);
        Destroy(gameObject);
    }
}
