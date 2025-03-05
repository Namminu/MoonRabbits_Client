using System;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBattlePopup : MonoBehaviour
{
    [SerializeField]
    private Button[] btns;

    private void Awake()
    {
        // 현재 씬에 존재하는 InventoryUI 인스턴스들을 모두 찾습니다.
        UIBattlePopup[] existingInstances = FindObjectsOfType<UIBattlePopup>(true);

        // 만약 이미 한 개 이상의 인스턴스가 존재한다면 (자기 자신 포함하여 2개 이상)
        if (existingInstances.Length > 1)
        {
            // 새로 들어온 객체는 파괴하여 중복 생성을 방지
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

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

        var pkt = new C2SMoveSector { TargetSector = dungeonIndex + 100 };

        GameManager.Network.Send(pkt);
        // EnterDungeon(dungeonIndex);
    }
}

/// <summary>
/// 던전에 입장하는 패킷 전송
/// </summary>
/// <param name="dungeonIndex">입장할 던전의 코드</param>
//     private void EnterDungeon(int dungeonIndex)
//     {
//         C2SSectorEnter enterPacket = new C2SSectorEnter { SectorId = dungeonIndex };
//         GameManager.Network.Send(enterPacket);
//     }
// }
