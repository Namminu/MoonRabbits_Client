using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TempSkillManager : MonoBehaviour
{
    [SerializeField]
    private TempPlayer player;

    private const int throwPower = 5;
    private bool isCasting = false;

    private bool isGrenadeReady = true;
    private const float coolTimeQ = 5f;

    private bool isTrapReady = true;
    private const float coolTimeE = 5f;
    private Queue<GameObject> traps = new();
    private const int maxTraps = 3;

    public Action eventQ; // Q키 누르면 발동
    public Action eventE; // E키 누르면 발동

    private void Start()
    {
        player = GetComponentInParent<TempPlayer>();
        eventQ += () => StartCoroutine(nameof(ThrowGrenade));
        eventE += () => StartCoroutine(nameof(SetTrap));
    }

    IEnumerator ThrowGrenade()
    {
        if (isCasting || !isGrenadeReady)
            yield break;

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
            Vector3 forceVec = throwTargetPos.point - transform.position;
            forceVec.y = throwPower;

            GameObject skillObj = Instantiate(
                player.grenade,
                transform.position,
                transform.rotation
            );

            Rigidbody rigid = skillObj.GetComponent<Rigidbody>();
            rigid.AddForce(forceVec, ForceMode.Impulse);
            rigid.AddTorque(Vector3.back, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1f);
        isCasting = false;

        yield return new WaitForSeconds(coolTimeQ);
        isGrenadeReady = true;
    }

    IEnumerator SetTrap()
    {
        if (isCasting || !isTrapReady)
            yield break;

        isCasting = true;
        isTrapReady = false;

        NavMeshAgent navAgent = player.NavAgent;
        navAgent.isStopped = true;
        navAgent.destination = player.transform.position;

        player.Anim.SetTrigger("SetTrap");

        GameObject trap = Instantiate(
            player.trap,
            player.transform.position,
            player.transform.rotation
        );

        traps.Enqueue(trap);

        if (traps.Count > maxTraps)
        {
            GameObject oldTrap = traps.Dequeue();
            Destroy(oldTrap);
        }

        yield return new WaitForSeconds(1f);
        isCasting = false;
        navAgent.isStopped = false;

        yield return new WaitForSeconds(coolTimeE);
        isTrapReady = true;
    }
}
