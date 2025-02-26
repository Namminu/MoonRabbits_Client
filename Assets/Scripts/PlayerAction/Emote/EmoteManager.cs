using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class EmoteManager : MonoBehaviour
{
    [SerializeField]
    private MyPlayer player;

    private string[] anims = { "Happy", "Sad", "Greeting" };

    private int[] animKeys = { 111, 222, 333 };

    private bool isEmoting = false;

    public Action event1;
    public Action event2;
    public Action event3;

    private void Start()
    {
        player = gameObject.GetComponent<MyPlayer>();
        event1 += EmoteHappy;
        event2 += EmoteSad;
        event3 += EmoteGreeting;
    }

    public void EmoteHappy()
    {
        player.NavAgent.SetDestination(player.transform.position);
        var animPkt = new C2SAnimation { AnimCode = animKeys[0] };
        GameManager.Network.Send(animPkt);
    }

    public void EmoteSad()
    {
        player.NavAgent.SetDestination(player.transform.position);
        var animPkt = new C2SAnimation { AnimCode = animKeys[1] };
        GameManager.Network.Send(animPkt);
    }

    public void EmoteGreeting()
    {
        player.NavAgent.SetDestination(player.transform.position);
        var animPkt = new C2SAnimation { AnimCode = animKeys[2] };
        GameManager.Network.Send(animPkt);
    }
}
