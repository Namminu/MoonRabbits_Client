using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;

public class TownManager : SManagerBase
{
    private static TownManager _instance;
    public static TownManager Instance => _instance;

    [SerializeField]
    private UIStart uiStart;

    [SerializeField]
    private UIAnimation uiAnimation;

    [SerializeField]
    private TMP_Text txtServer;

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            base.Awake();
            SectorCode = 100;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetPrefabPath();
    }

    private void Start()
    {
        if (!GameManager.Network.IsConnected)
        {
            uiStart.gameObject.SetActive(true);
        }
        else if (GameManager.Instance.NickName == null)
        {
            Connected();
        }
    }

    public void TryConnectToServer(string gameServer, string port)
    {
        GameManager.Network.Init(gameServer, port);
        txtServer.text = gameServer;
    }

    public void GameStart(string userName, int classCode)
    {
        GameManager.Instance.NickName = userName;
        GameManager.Instance.ClassCode = classCode;
        Connected();
    }

    public void Connected()
    {
        var enterPacket = new C2SEnterTown
        {
            Nickname = GameManager.Instance.NickName,
            ClassCode = GameManager.Instance.ClassCode,
        };

        GameManager.Network.Send(enterPacket);
    }

    protected override void ActivateUI()
    {
        uiStart.gameObject.SetActive(false);
        base.ActivateUI();
    }
}
