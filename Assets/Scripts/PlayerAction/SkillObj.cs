using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private SphereCollider effectRange;

    private bool isActive = false;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        effectRange = GetComponent<SphereCollider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive && collision.gameObject.CompareTag("Ground"))
            if (type == ObjType.grenade)
            {
                GameObject effect = gameObject.transform.Find("ExplodeEffect").gameObject;
                StartCoroutine(nameof(Explode), effect);
            }
            else if (type == ObjType.trap)
            {
                StartCoroutine(nameof(SetTrap));
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 감지");
        }
        if (other.gameObject.CompareTag("Monster"))
        {
            Debug.Log("몬스터 감지"); // 웨 12마리가 발동되는 가, 웨 콜라이더 안 켜졌는데 발동하는가
        }
    }

    IEnumerator Explode(GameObject effect)
    {
        isActive = true;

        yield return new WaitForSeconds(0.5f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(0.5f);
        mesh.enabled = false;
        trail.enabled = false;

        yield return new WaitForSeconds(0.1f);
        effectRange.enabled = true;
        effect.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    IEnumerator SetTrap()
    {
        yield return null; // 로직 채우기
    }
}
