using Google.Protobuf.Protocol;
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
    public bool IsAvailable
    {
        get => isAvailable;
        set => isAvailable = value;
    }


    public int idx = 100;

    public int resourceId; // 1이면 나무, 2면 바위 (도끼와 곡괭이 enum 및 index와 맞춰져있음다)

    [SerializeField]
    private int durability = 5;
    public int Durability
    {
        get => durability;
        set
        {
            if (value < 1)
            {
                isAvailable = false;
                ChangeIcon(isAvailable);
            }
            else
            {
                isAvailable = true;
                ChangeIcon(isAvailable);
            }
            durability = value;
        }
    }
    private int angle = 180;
    public int Angle
    {
        get => angle;
        set => angle = value;
    }
    private const int maxDurability = 5;
    private int difficulty;
    public int Difficulty
    {
        get => difficulty;
        set => difficulty = value;
    }
    private long starttime;
    public long Starttime
    {
        get => starttime;
        set => starttime = value;
    }

    private void Update()
    {
        if (isAvailable && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(this.durability.ToString() + this.difficulty.ToString());
        }
    }

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

    public void ResourcesGatheringStart(int angle, int difficulty)
    {
        Debug.Log("스킬체크 시작");
        this.angle = angle;
        this.Difficulty = difficulty;
        this.Starttime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        UISkillCheck.Instance.StartSkillCheck(this.idx, angle, difficulty);
    }
    public void ResourcesGatheringSkillCheck(int durability)
    {

    }
}
