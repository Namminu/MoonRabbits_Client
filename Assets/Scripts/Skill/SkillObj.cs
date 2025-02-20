using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObj : MonoBehaviour
{
    public enum ObjType
    {
        grenade,
        trap,
    }

    public ObjType type;

    private bool isActive = false;

    void OnCollisionEnter(Collision collision)
    {
        if (type == ObjType.grenade && !isActive && collision.gameObject.CompareTag("Ground"))
        {
            GameObject effect = gameObject.transform.Find("ExplodeEffect").gameObject;
            StartCoroutine(nameof(Activate), effect);
        }
        if (type == ObjType.trap && !isActive && collision.gameObject.CompareTag("Player"))
        {
            // Trap 발동 로직
        }
    }

    IEnumerator Activate(GameObject effect)
    {
        Debug.Log("Activate 진입");
        isActive = true;
        yield return new WaitForSeconds(1f);
        effect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        yield break;
    }
}
