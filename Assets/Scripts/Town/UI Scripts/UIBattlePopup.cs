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
            int btnIdx = i + 1;
            btns[i].onClick.AddListener(() => OnButtonClicked(btnIdx));
        }
    }

    private void OnButtonClicked(int btnIdx)
    {
        var pkt = new C2SMoveSector { TargetSector = btnIdx + 100 };

        GameManager.Network.Send(pkt);

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
