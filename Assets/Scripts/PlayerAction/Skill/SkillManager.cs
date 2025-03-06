using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using SRDebugger;
using UnityEngine;
using UnityEngine.AI;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private MyPlayer player;

    private bool isCasting = false;
    public bool IsCasting
    {
        get => isCasting;
        set { isCasting = value; }
    }

    private bool isGrenadeReady = true;
    public bool IsGrenadeReady
    {
        get => isGrenadeReady;
        set { isGrenadeReady = value; }
    }

    private bool isTrapReady = true;
    public bool IsTrapReady
    {
        get => isTrapReady;
        set { isTrapReady = value; }
    }

    public Action eventQ; // Q키 누르면 발동
    public Action eventE; // E키 누르면 발동
    public Action eventT; // T키 누르면 발동

    private void Start()
    {
        player = GetComponentInParent<MyPlayer>();
        eventQ += ThrowGrenade;
        eventE += SetTrap;
        eventT += Recall;
    }

    private void ThrowGrenade()
    {
        if (isCasting || !isGrenadeReady)
            return;

        isCasting = true;
        isGrenadeReady = false;

        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit throwTargetPos;

        if (
            Physics.Raycast(
                mousePos,
                out throwTargetPos,
                Mathf.Infinity,
                LayerMask.GetMask("Ground")
            )
        )
        {
            var sp = new Vec3
            {
                X = transform.position.x,
                Y = transform.position.y,
                Z = transform.position.z,
            };

            var tp = new Vec3
            {
                X = throwTargetPos.point.x,
                Y = throwTargetPos.point.y,
                Z = throwTargetPos.point.z,
            };

            var pkt = new C2SThrowGrenade { StartPos = sp, TargetPos = tp };
            GameManager.Network.Send(pkt);
        }
    }

    private void SetTrap()
    {
        if (isCasting || !isTrapReady)
            return;

        int nearTraps = Physics
            .OverlapSphere(player.transform.position, 1f, LayerMask.GetMask("Trap"))
            .Count();

        if (nearTraps > 0)
        {
            return;
        }

        isCasting = true;
        isTrapReady = false;

        var pkt = new C2SSetTrap
        {
            TrapPos = new Vec3
            {
                X = Mathf.Round(player.transform.position.x * 10),
                Y = 0,
                Z = Mathf.Round(player.transform.position.z * 10),
            },
        };

        GameManager.Network.Send(pkt);
    }

    private void Recall()
    {
        if (isCasting)
            return;

        isCasting = true;

        player.NavAgent.SetDestination(player.transform.position);

        var pkt = new C2SRecall { };
        GameManager.Network.Send(pkt);
    }
}
