using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class TempEmoteManager : MonoBehaviour
{
    [SerializeField]
    private TempPlayer player;

    private string[] anims = { "Happy", "Sad", "Greeting" };

    private int[] animKeys = { 111, 222, 333 };

    private bool isEmoting = false;

    public Action event1;
    public Action event2;
    public Action event3;

    private void Start()
    {
        player = gameObject.GetComponent<TempPlayer>();
        event1 += EmoteHappy;
        event2 += EmoteSad;
        event3 += EmoteGreeting;
    }

    public void EmoteHappy()
    {
        player.NavAgent.SetDestination(player.transform.position);
        player.Anim.SetTrigger(anims[0]);
    }

    public void EmoteSad()
    {
        player.NavAgent.SetDestination(player.transform.position);
        player.Anim.SetTrigger(anims[1]);
    }

    public void EmoteGreeting()
    {
        player.NavAgent.SetDestination(player.transform.position);
        player.Anim.SetTrigger(anims[2]);
    }
}
