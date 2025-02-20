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

    public Action eventF;

    private void Start()
    {
        player = GetComponentInParent<MyPlayer>();
        eventF += Interact;
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

    public void Interact()
    {
        if (isInteracting || target == null)
            return;

        Debug.Log($"f키 눌렸는감? {player.InteractInput}");

        if (target.Durability > 0)
        {
            isInteracting = true;

            player.NavAgent.isStopped = true;
            player.NavAgent.destination = player.transform.position;

            Vector3 direction = (target.transform.position - player.transform.position).normalized;
            direction.y = 0;
            player.transform.rotation = Quaternion.LookRotation(direction);

            player.Anim.SetTrigger("Interact");

            Invoke(nameof(InteractOut), 0.5f); // 재상호작용은 일단 0.5초 후에 가능 (애니메이션 출력시간)
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
}
