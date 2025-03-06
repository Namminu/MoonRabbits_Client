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
