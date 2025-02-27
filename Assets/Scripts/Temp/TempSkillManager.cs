using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TempSkillManager : MonoBehaviour
{
    [SerializeField]
    private TempPlayer player;

    private const int throwPower = 4;
    private bool isCasting = false;

    private bool isGrenadeReady = true;
    private const float coolTimeQ = 5f;

    private bool isTrapReady = true;
    private const float coolTimeE = 5f;
    private Queue<GameObject> traps = new();
    private const int maxTraps = 3;

    private const int recallTimer = 5;

    public Action eventQ; // Q키 누르면 발동
    public Action eventE; // E키 누르면 발동
    public Action eventT; // T키 누르면 발동

    private void Start()
    {
        player = GetComponentInParent<TempPlayer>();
        eventQ += () => StartCoroutine(nameof(ThrowGrenade));
        eventE += () => StartCoroutine(nameof(SetTrap));
        eventT += () => StartCoroutine(nameof(Recall));
    }

    IEnumerator ThrowGrenade()
    {
        if (isCasting || !isGrenadeReady)
            yield break;

        isCasting = true;
        isGrenadeReady = false;

        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit throwTargetPos;

        if (
            Physics.Raycast(
                mousePos,
                out throwTargetPos,
                Mathf.Infinity,
                LayerMask.GetMask("Ground")
            )
        )
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = throwTargetPos.point;

            float gravity = Mathf.Abs(Physics.gravity.y); // 중력 (절댓값 사용)

            // 수평 거리 및 높이 차이 계산
            Vector3 flatTarget = new Vector3(targetPos.x, startPos.y, targetPos.z);
            float distance = Vector3.Distance(startPos, flatTarget);
            float heightDifference = targetPos.y - startPos.y;

            // 🟢 수직 방향 속도 계산
            float initialVelocityY = Mathf.Sqrt(2 * gravity * throwPower); // throwPower는 목표 높이
            float timeUp = initialVelocityY / gravity; // 상승 시간
            float timeDown = Mathf.Sqrt(2 * Mathf.Max(0, heightDifference) / gravity); // 하강 시간
            float timeToTarget = timeUp + timeDown; // 총 비행 시간

            // 🟢 수평 방향 속도 계산 (X, Z)
            float initialVelocityXZ = distance / timeToTarget;
            Vector3 direction = (flatTarget - startPos).normalized;
            Vector3 velocity = direction * initialVelocityXZ;
            velocity.y = initialVelocityY; // Y축 속도 추가

            // 🟢 수류탄 생성 및 속도 적용
            GameObject skillObj = Instantiate(player.grenade, startPos, Quaternion.identity);
            Rigidbody rigid = skillObj.GetComponent<Rigidbody>();
            rigid.velocity = velocity; // 물리 엔진 기반 속도 설정
            rigid.AddTorque(Vector3.back, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(0.3f);
        isCasting = false;

        yield return new WaitForSeconds(coolTimeQ);
        isGrenadeReady = true;
    }

    IEnumerator SetTrap()
    {
        if (isCasting || !isTrapReady)
            yield break;

        isCasting = true;
        isTrapReady = false;

        NavMeshAgent navAgent = player.NavAgent;
        navAgent.isStopped = true;
        navAgent.destination = player.transform.position;

        player.Anim.SetTrigger("SetTrap");

        GameObject trap = Instantiate(
            player.trap,
            player.transform.position,
            player.transform.rotation
        );

        traps.Enqueue(trap);

        if (traps.Count > maxTraps)
        {
            GameObject oldTrap = traps.Dequeue();
            Destroy(oldTrap);
        }

        yield return new WaitForSeconds(1f);
        isCasting = false;
        navAgent.isStopped = false;

        yield return new WaitForSeconds(coolTimeE);
        isTrapReady = true;
    }

    IEnumerator Recall()
    {
        if (isCasting)
            yield break;

        isCasting = true;

        NavMeshAgent agent = player.NavAgent;

        agent.isStopped = true;
        agent.destination = player.transform.position;

        GameObject effect = player.transform.Find("RecallEffect").gameObject;
        effect.SetActive(true);

        int castingTime = 0;

        while (castingTime < recallTimer)
        {
            if (agent.destination != player.transform.position)
            {
                isCasting = false;
                agent.isStopped = false;
                effect.SetActive(false);
                yield break;
            }

            yield return new WaitForSeconds(1);
            Debug.Log($"귀환까지 남은 시간 : {recallTimer - castingTime}초");
            castingTime += 1;
        }

        yield return new WaitForSeconds(0.5f);
        isCasting = false;
        agent.isStopped = false;
        effect.SetActive(false);
        // 서버로 마을 가는 패킷?!
    }
}
