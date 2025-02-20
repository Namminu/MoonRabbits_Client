using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ResourceController : MonoBehaviour
{
    [SerializeField]
    private GameObject availableIcon;

    [SerializeField]
    private GameObject unavailableIcon;
    private bool isAvailable = true;

    private int idx;
    private int resourceId;
    private int startTime;

    [SerializeField]
    private int durability = 5;
    public int Durability
    {
        get => durability;
    }
    private const int maxDurability = 5;
    private int difficulty;
    private int angle;

    public void DecreaseDurability(int cnt)
    {
        durability = Mathf.Max(durability - cnt, 0);

        if (durability <= 0)
        {
            isAvailable = false;
            ChangeIcon(isAvailable);
        }
    }

    public void RecoverDurability(int cnt)
    {
        durability = Mathf.Min(durability + cnt, maxDurability);

        if (durability > 0)
        {
            isAvailable = true;
            ChangeIcon(isAvailable);
        }
    }

    private void ChangeIcon(bool isAvailable)
    {
        if (isAvailable && unavailableIcon.activeSelf)
        {
            unavailableIcon.SetActive(false);
            availableIcon.SetActive(true);
        }
        else if (!isAvailable && availableIcon.activeSelf)
        {
            availableIcon.SetActive(false);
            unavailableIcon.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("상호작용 가능");
            if (isAvailable)
            {
                availableIcon.SetActive(true);
            }
            else
            {
                unavailableIcon.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("상호작용 불가");

            if (availableIcon.activeSelf)
                availableIcon.SetActive(false);
            else if (unavailableIcon.activeSelf)
                unavailableIcon.SetActive(false);
        }
    }
}
