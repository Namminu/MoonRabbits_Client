using Google.Protobuf.Protocol;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    [SerializeField]
    private MyPlayer player;

    [SerializeField]
    private ResourceController targetResource = null;

    [SerializeField]
    private Portal targetPortal = null;
    private const int portalTimer = 5;
    private bool isPortalReady = true;
    private bool isInteracting = false;
    private bool isEquipChanging = false;

    private string[] anims = { "none", "Axe", "PickAxe" };

    public Action eventF; // F키 누르면 발동
    public Action eventR; // R키 누르면 발동

    private void Start()
    {
        player = GetComponentInParent<MyPlayer>();
        eventF += Interact;
        eventR += EquipChange;
    }

    private void EquipChange()
    {
        if (isInteracting || isEquipChanging)
            return;

        isEquipChanging = true;

        switch (player.currentEquip)
        {
            case 0:
                player.axe.SetActive(true);
                player.currentEquip = (int)MyPlayer.EquipState.axe;
                break;
            case 1:
                player.axe.SetActive(false);
                player.pickAxe.SetActive(true);
                player.currentEquip = (int)MyPlayer.EquipState.pickAxe;
                break;
            case 2:
                player.axe.SetActive(true);
                player.pickAxe.SetActive(false);
                player.currentEquip = (int)MyPlayer.EquipState.axe;
                break;
        }

        Invoke(nameof(EquipChangeOut), 0.5f);
    }

    private void EquipChangeOut()
    {
        isEquipChanging = false;
    }

    private void Interact()
    {
        if (targetResource != null)
        {
            GatherResource();
        }
        else if (targetPortal != null)
        {
            UsePortal();
        }
    }

    private void GatherResource()
    {
        if (isInteracting)
            return;

        if (player.currentEquip != targetResource.resourceId)
        {
            Debug.Log("자원에 맞는 도구를 장비해주세요");
            return;
        }

        if (targetResource.Durability > 0)
        {
            isInteracting = true;

            player.NavAgent.isStopped = true;
            player.NavAgent.destination = player.transform.position;

            Vector3 direction = (
                targetResource.transform.position - player.transform.position
            ).normalized;
            direction.y = 0;
            player.transform.rotation = Quaternion.LookRotation(direction);

            player.Anim.SetTrigger(anims[player.currentEquip]);


            GameManager.Network.Send(new C2SGatheringStart { PlacedId = targetResource.idx });


            GameManager.Network.Send(new C2SGatheringSkillCheck { DeltaTime = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - targetResource.Starttime) });



            Invoke(nameof(GatherOut), 0.7f); // 재상호작용은 일단 0.7초 후에 가능 (애니메이션 출력시간)
        }
        else
        {
            Debug.Log("더 이상 캘 수 없는 자원임다");
        }
    }

    private void GatherOut()
    {
        //targetResource.DecreaseDurability(1); // 숫자 만큼 감소시킴 나중에 능력치만큼 적용?
        isInteracting = false;
        player.NavAgent.isStopped = false;
    }

    private void UsePortal()
    {
        if (isInteracting)
            return;
        if (!isPortalReady)
        {
            Debug.Log("아직 포탈을 이용할 수 없습니다");
            return;
        }

        isInteracting = true;
        isPortalReady = false;

        Vector3 portalPos = targetPortal.ConnectedPortal.position;
        portalPos.y = 0;

        player.NavAgent.Warp(portalPos);
        player.NavAgent.ResetPath();

        StartCoroutine(SetPortalTimer());
        isInteracting = false;
    }

    IEnumerator SetPortalTimer()
    {
        int remainingTime = 0;
        while (remainingTime < portalTimer)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log($"포탈 재이용까지 남은 시간 : {portalTimer - remainingTime}");
            remainingTime += 1;
        }
        yield return new WaitForSeconds(0.5f);
        isPortalReady = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (targetResource == null && other.gameObject.CompareTag("Resource"))
        {
            targetResource = other.GetComponent<ResourceController>();
        }
        else if (targetPortal == null && other.gameObject.CompareTag("Portal"))
        {
            targetPortal = other.GetComponent<Portal>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetResource != null && other.gameObject.CompareTag("Resource"))
        {
            targetResource = null;
        }
        else if (targetPortal != null & other.gameObject.CompareTag("Portal"))
        {
            targetPortal = null;
        }
    }
}
