using System;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBattlePopup : MonoBehaviour
{
    [SerializeField]
    private Button[] btns;

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int dungeonIndex = i + 1;
            btns[i].onClick.AddListener(() => OnButtonClicked(dungeonIndex));
        }
    }

    private void OnButtonClicked(int dungeonIndex)
    {
        // SceneManager.LoadScene("Test");

        var pkt = new C2SLeave { TargetScene = dungeonIndex + 100 };

        GameManager.Network.Send(pkt);
        // EnterDungeon(dungeonIndex);
    }

    /// <summary>
    /// 던전에 입장하는 패킷 전송
    /// </summary>
    /// <param name="dungeonIndex">입장할 던전의 코드</param>
    private void EnterDungeon(int dungeonIndex)
    {
        C2SSectorEnter enterPacket = new C2SSectorEnter { SectorId = dungeonIndex };
        GameManager.Network.Send(enterPacket);
    }
}
