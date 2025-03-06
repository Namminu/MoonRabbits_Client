using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

public class UISkillCheck : MonoBehaviour
{
    private static UISkillCheck _instance = null;
    public static UISkillCheck Instance => _instance;

    public Image blackCircle;
    public Image whiteCircle;
    public Image clockhand;
    private int targetResource;
    public int TargetResource { get { return targetResource; } }
    private int angle = 90;
    private int difficulty = 1;
    private bool isEnabled = false;
    private bool isSuccess = false;
    private bool isFailed = false;
    private Stopwatch skillChekcTime = new();


    void Start()
    {
        if (_instance == null)
        {
            _instance = this;

        }
    }

    void Update()
    {
        if (isEnabled)
        {
            float curAngle = (int)clockhand.transform.eulerAngles.z;
            if (this.angle == 0 && curAngle > 0 && curAngle < 30)
            {
                this.angle = 1;
                GameManager.Network.Send(new C2SGatheringDone { });
                GameManager.Network.Send(new C2SGatheringStart { PlacedId = targetResource });

            }
            else
            {
                clockhand.transform.Rotate(0, 0, 90f * Time.deltaTime);
            }
        }
        else
        {
            this.angle = 0;
            clockhand.transform.Rotate(0, 0, 0);
        }
    }


    public void StartSkillCheck(int placedId, int angle, int difficulty)
    {

        this.whiteCircle.color = Color.white;
        this.targetResource = placedId;
        this.angle = angle;
        this.whiteCircle.transform.rotation = Quaternion.Euler(0, 0, this.angle + 60/difficulty);
        this.difficulty = difficulty;
        this.whiteCircle.fillAmount = 1f / (float)difficulty / 6f;
        this.isEnabled = true;
        this.isSuccess = false;
        this.isFailed = false;
        clockhand.transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Find("Circle").gameObject.SetActive(true);
    }
    public void EndSkillCheck()
    {
        transform.Find("Circle").gameObject.SetActive(false);
        this.isEnabled = false;
        this.isSuccess = false;
        this.isFailed = false;
        this.whiteCircle.color = Color.white;
        MyPlayer.instance.Anim.SetTrigger("Exit");
    }
    public void SkillCheck()
    {
        if (this.isSuccess || this.isFailed)
        {
            return;
        }

        int skillCheckAngle = (int)clockhand.transform.eulerAngles.z;
        UnityEngine.Debug.Log(skillCheckAngle.ToString() + "   " + this.angle.ToString());
        GameManager.Network.Send(new C2SGatheringSkillCheck { DeltaTime = 0 });
        if (skillCheckAngle > this.angle && skillCheckAngle < (this.angle + 60 / this.difficulty))
        {
            this.isSuccess = true;
            this.whiteCircle.color = Color.blue;
        }
        else
        {
            this.isFailed = true;
            this.whiteCircle.color = Color.red;
        }
        this.angle = 0;
    }
    public void ResourcesUpdateDurability(int durability)
    {
        if (durability <= 0)
        {
            EndSkillCheck();
            //즉시 종료
        }
    }
    public void ResourcesGatheringSkillCheck(int durability)
    {
        this.isSuccess = true;
        if (durability <= 0)
        {
            MyPlayer.instance.InteractManager.GatherOut(false);
            //즉시 종료
        }
    }



}
