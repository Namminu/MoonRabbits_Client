using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Sensor : MonoBehaviour
{
    private MonsterController monsterController;

    private List<Transform> targetList = new(); // new() = new List<Transform>

    private Coroutine coSetTarget;

    private void Start()
    {
        monsterController = transform.parent.GetComponent<MonsterController>();
    }

    public bool IsDuplicate(Transform tr) // 새로 발견한 녀석이 이미 감지한 녀석인지 체크하는 함수
    {
        return targetList.Contains(tr) ? false : true;
    }

    private Transform FindTarget() // 타겟리스트에서 랜덤 타겟 반환하는 함수
    {
        if (targetList.Count == 0)
        {
            return null;
        }

        return targetList[Random.Range(0, targetList.Count)];
    }

    IEnumerator SetTarget() // 3초 대기 후 타겟 설정하는 함수
    {
        yield return new WaitForSeconds(3f);
        monsterController.Target = FindTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.GetComponent<TempPlayer>();

            if (IsDuplicate(other.transform) && player.IsAlive)
            {
                targetList.Add(other.transform);
            }

            monsterController.Target = FindTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            targetList.Remove(other.transform);

            if (coSetTarget != null)
            {
                StopCoroutine(coSetTarget);
                coSetTarget = null;
            }

            coSetTarget = StartCoroutine(SetTarget());
        }
    }
}
