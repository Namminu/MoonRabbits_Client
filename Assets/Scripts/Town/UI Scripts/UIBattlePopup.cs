using System;
using Google.Protobuf.Protocol;
using UnityEngine;
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
        EnterDungeon(dungeonIndex);
    }

    /// <summary>
    /// 던전에 입장하는 패킷 전송
    /// </summary>
    /// <param name="dungeonIndex">입장할 던전의 코드</param>
    private void EnterDungeon(int dungeonIndex)
    {
        C2SDungeonEnter enterPacket = new C2SDungeonEnter { DungeonCode = dungeonIndex };
        GameManager.Network.Send(enterPacket);
    }
}
