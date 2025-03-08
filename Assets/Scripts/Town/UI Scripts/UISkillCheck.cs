using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
    public GameObject dropedItem;
    private int targetResource;
    public int TargetResource { get { return targetResource; } }
    private int angle = 90;
    private int difficulty = 1;
    private bool isEnabled = false;
    private bool isSuccess = false;
    private bool isFailed = false;
    private float failTimeOut = 0;
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
            clockhand.transform.Rotate(0, 0, 90f * Time.deltaTime);
            if (isFailed) failTimeOut += Time.deltaTime;
            else failTimeOut = 0;
            if (failTimeOut > 1)
            {
                GameManager.Network.Send(new C2SGatheringStart { PlacedId = targetResource });
                failTimeOut = 0;
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
        float pickSpeed = (float) MyPlayer.instance.GetPickSpeed();
        this.whiteCircle.color = Color.white;
        this.targetResource = placedId;
        this.angle = angle;
        this.whiteCircle.transform.rotation = Quaternion.Euler(0, 0, (float)(this.angle + 60 + (pickSpeed < 30 ? pickSpeed : 30 + pickSpeed * 0.3f) / difficulty));
        this.difficulty = difficulty;
        this.whiteCircle.fillAmount = 1f / (float)difficulty * (float)(60 + (pickSpeed < 30f ? pickSpeed : 30f + pickSpeed * 0.3f)) / 360f;
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
        MyPlayer.instance.InteractManager.IsInteracting = false;
        GameManager.Network.Send(new C2SGatheringAnimationEnd { });

    }
    public async void SkillCheck()
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
        }
        else
        {
            this.failTimeOut = 0;
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
        this.whiteCircle.color = Color.blue;
        this.isSuccess = true;
        if (durability <= 0)
        {
            MyPlayer.instance.InteractManager.GatherOut(false);
            //즉시 종료
        }
    }
    public void ResourcesGatheringDone(S2CGatheringDone pkt)
    {
        UnityEngine.Debug.Log(pkt);
        if (dropedItem != null)
        {
            Instantiate(dropedItem, new Vector3(0, 0, 0), Quaternion.identity);

            // Instantiate the prefab
            GameObject instance = Instantiate(dropedItem, new Vector3(0, 0, 0), Quaternion.identity);

            instance.transform.SetParent(gameObject.transform);
            // Get the DroppedItem component from the instantiated prefab
            UIItemDropOnCanvas droppedItem = instance.GetComponent<UIItemDropOnCanvas>();

            // Check if the DroppedItem component exists
            if (droppedItem != null)
            {
                // Initialize the prefab with custom values
                droppedItem.Initialize(pkt.ItemId, gameObject.transform.position);

                // Set the prefab to active
            }

        }
    }




}
