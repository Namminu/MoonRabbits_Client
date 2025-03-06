using System;
using System.Collections;
using Google.Protobuf.Protocol;
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
    public bool isEquipChanging = false;

    private string[] anims = { "none", "Axe", "PickAxe" };

    public Action eventF; // F키 누르면 발동
    public Action eventR; // R키 누르면 발동

    private void Start()
    {
        player = GetComponentInParent<MyPlayer>();
        eventF += Interact;
        eventR += ChangeEquip;
    }

    private void ChangeEquip()
    {
        if (isInteracting || isEquipChanging)
            return;

        isEquipChanging = true;

        int nextEquip = player.currentEquip == 1 ? 2 : 1;
        var pkt = new C2SEquipChange { NextEquip = nextEquip };
        GameManager.Network.Send(pkt);
    }

    private void Interact()
    {
        if (targetResource != null)
        {
            GatherResource();
        }
        else if (targetPortal != null)
        {
            var portalPacket = new C2SPortal { InPortalId = targetPortal.id };
            GameManager.Network.Send(portalPacket);
        }
    }

    private void GatherResource()
    {
        if (isInteracting)
        {
            UISkillCheck.Instance.SkillCheck();
            return;
        }

        if (player.currentEquip != targetResource.resourceId)
        {
            Debug.Log("자원에 맞는 도구를 장비해주세요");
            return;
        }

        if (targetResource.Durability > 0)
        {
            isInteracting = true;

            // player.NavAgent.isStopped = true;
            player.NavAgent.ResetPath();
            player.NavAgent.destination = player.transform.position;
            player.NavAgent.velocity = Vector3.zero;

            Vector3 direction = (
                targetResource.transform.position - player.transform.position
            ).normalized;
            direction.y = 0;
            player.transform.rotation = Quaternion.LookRotation(direction);

            player.Anim.SetTrigger(anims[player.currentEquip]);

            GameManager.Network.Send(new C2SGatheringStart { PlacedId = targetResource.idx });





            //Invoke(nameof(GatherOut), 0.7f); // 재상호작용은 일단 0.7초 후에 가능 (애니메이션 출력시간)
        }
        else
        {
            Debug.Log("더 이상 캘 수 없는 자원임다");
        }
    }

    public void GatherOut(bool isinteracting)
    {
        if (!this.isInteracting)
        {
            return;
        }
        //targetResource.DecreaseDurability(1); // 숫자 만큼 감소시킴 나중에 능력치만큼 적용?
        this.isInteracting = isinteracting;
        UISkillCheck.Instance.EndSkillCheck();
    }

    public void UsePortal()
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
