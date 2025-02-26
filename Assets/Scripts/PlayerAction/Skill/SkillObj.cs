using System;
using System.Collections;
using System.Collections.Generic;
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

    private MeshRenderer mesh;
    private Rigidbody rigid;
    private TrailRenderer trail;
    private GameObject effect;

    private const float explosionRange = 5f;
    private const float sensorRange = 1f;
    private const float stunTimer = 5f;

    private bool isActive = false;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        effect = transform.Find("Effect").gameObject;
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
                target.GetComponent<MonsterController>().Stun(stunTimer);
            }
            else if (target.CompareTag("Player"))
            {
                target.GetComponent<TempPlayer>().Stun(stunTimer); // 나중에 myplayer로 수정
            }
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

        target.GetComponent<TempPlayer>().Stun(stunTimer); // 나중에 myplayer로 수정

        yield return new WaitForSeconds(stunTimer);
        Destroy(gameObject);
    }
}
