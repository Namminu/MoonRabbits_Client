using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class UIBattleLog : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtLog;

    [SerializeField]
    private Button[] btns;

    [SerializeField]
    private TMP_Text[] btnTexts;

    [SerializeField]
    private Image imgContinue;

    // private BtnInfo[] btnInfos;
    private bool isLogComplete;
    private string currentMessage;

    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int idx = i + 1; // 버튼 인덱스를 캡처
            btns[i].onClick.AddListener(() => OnButtonClick(idx));
        }
    }

    // public void Initialize(BattleLog battleLog)
    // {
    //     btnInfos = battleLog.Btns?.ToArray();

    //     if (btnInfos == null || btnInfos.Length == 0)
    //     {
    //         Debug.Log("No button info available");
    //         btnInfos = null;
    //     }

    //     DisplayLog(battleLog.Msg, battleLog.TypingAnimation);
    // }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (BattleManager.Instance.UiScreen.gameObject.activeSelf)
                return;

            if (!isLogComplete)
            {
                SkipTypingAnimation();
            }
            // else if (btnInfos == null)
            // {
            //     SendResponse(0);
            // }
        }
    }

    private void DisplayLog(string log, bool useTypingAnimation = true)
    {
        isLogComplete = false;
        DOTween.KillAll();
        txtLog.text = string.Empty;

        SetImageAlpha(imgContinue, 0);
        currentMessage = log;

        if (useTypingAnimation)
        {
            txtLog
                .DOText(currentMessage, currentMessage.Length / 20f)
                .SetEase(Ease.Linear)
                .OnComplete(OnLogComplete);
        }
        else
        {
            txtLog.text = currentMessage;
            OnLogComplete();
        }
    }

    private void OnLogComplete()
    {
        isLogComplete = true;

        // if (btnInfos == null)
        // {
        //     imgContinue.DOFade(1, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
        // }
        // else
        // {
        //     UpdateButtons(btnInfos);
        // }
    }

    // private void UpdateButtons(BtnInfo[] btnInfos)
    // {
    //     for (int i = 0; i < btns.Length; i++)
    //     {
    //         bool isActive = i < btnInfos.Length;
    //         btns[i].gameObject.SetActive(isActive);

    //         if (isActive)
    //         {
    //             btns[i].interactable = btnInfos[i].Enable;
    //             btnTexts[i].text = btnInfos[i].Msg;
    //         }
    //     }
    // }

    private void SkipTypingAnimation()
    {
        DOTween.KillAll();
        txtLog.text = currentMessage;
        OnLogComplete();
    }

    private void OnButtonClick(int index)
    {
        /* 패킷 수정으로 보류된 메서드 */
        // SendResponse(index);
    }

    /* 패킷 수정으로 보류된 메서드 */
    // private void SendResponse(int index)
    // {
    //     var response = new C_PlayerResponse { ResponseCode = index };
    //     GameManager.Network.Send(response);
    // }

    private static void SetImageAlpha(Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
