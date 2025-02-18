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

    public bool IsDuplicate(Transform tr)
    {
        return targetList.Contains(tr) ? false : true;
    }

    private Transform FindTarget()
    {
        if (targetList.Count == 0)
        {
            return null;
        }

        return targetList[Random.Range(0, targetList.Count)];
    }

    IEnumerator SetTarget()
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
