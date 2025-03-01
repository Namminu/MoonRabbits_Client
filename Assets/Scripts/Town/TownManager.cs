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

    private Dictionary<int, Player> playerList = new();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
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
        else if (GameManager.Instance.UserName == null)
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
        GameManager.Instance.UserName = userName;
        GameManager.Instance.ClassCode = classCode;
        if (!UiChat.gameObject.activeSelf)
            UiChat.gameObject.SetActive(true);
        Connected();
    }

    public void Connected()
    {
        var enterPacket = new C2SEnter
        {
            Nickname = GameManager.Instance.UserName,
            ClassCode = GameManager.Instance.ClassCode,
            TargetSector = SectorCode,
        };

        GameManager.Network.Send(enterPacket);
    }

    protected override void ActivateUI()
    {
        uiStart.gameObject.SetActive(false);
        uiAnimation.gameObject.SetActive(true);
        if (!UiChat.gameObject.activeSelf)
            UiChat.gameObject.SetActive(true);
    }
}
