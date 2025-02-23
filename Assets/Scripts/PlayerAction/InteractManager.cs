using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    [SerializeField]
    private MyPlayer player;

    [SerializeField]
    private ResourceController target = null;
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
        if (isInteracting || target == null)
            return;

        if (player.currentEquip != target.resourceId)
        {
            Debug.Log("자원에 맞는 도구를 장비해주세요");
            return;
        }

        if (target.Durability > 0)
        {
            isInteracting = true;

            player.NavAgent.isStopped = true;
            player.NavAgent.destination = player.transform.position;

            Vector3 direction = (target.transform.position - player.transform.position).normalized;
            direction.y = 0;
            player.transform.rotation = Quaternion.LookRotation(direction);

            player.Anim.SetTrigger(anims[player.currentEquip]);

            Invoke(nameof(InteractOut), 0.7f); // 재상호작용은 일단 0.7초 후에 가능 (애니메이션 출력시간)
        }
        else
        {
            Debug.Log("더 이상 캘 수 없는 자원임다");
        }
    }

    private void InteractOut()
    {
        target.DecreaseDurability(1); // 숫자 만큼 감소시킴 나중에 능력치만큼 적용?
        isInteracting = false;
        player.NavAgent.isStopped = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (target == null & other.gameObject.CompareTag("Resource"))
        {
            target = other.GetComponent<ResourceController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (target != null && other.gameObject.CompareTag("Resource"))
        {
            target = null;
        }
    }
}
