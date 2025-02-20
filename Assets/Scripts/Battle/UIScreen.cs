using System;
using DG.Tweening;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScreen : MonoBehaviour
{
    [SerializeField]
    private Image bg;

    [SerializeField]
    private TMP_Text txtMessage;

    [SerializeField]
    private TMP_Text txtContinue;

    private bool isMessageComplete;
    private string currentMessage;

    // public void Display(ScreenText screenText)
    // {
    //     if (!gameObject.activeSelf)
    //     {
    //         gameObject.SetActive(true);
    //     }

    //     ConfigureText(screenText.Msg, screenText.TypingAnimation);

    //     if (screenText.TextColor != null)
    //     {
    //         SetTextColor((byte)screenText.TextColor.R, (byte)screenText.TextColor.G, (byte)screenText.TextColor.B);
    //     }

    //     if (screenText.ScreenColor != null)
    //     {
    //         SetBackgroundColor((byte)screenText.ScreenColor.R, (byte)screenText.ScreenColor.G, (byte)screenText.ScreenColor.B);
    //     }

    //     if (screenText.Alignment != null)
    //     {
    //         SetTextAlignment(screenText.Alignment.X, screenText.Alignment.Y);
    //     }
    // }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (!isMessageComplete)
            {
                SkipTypingAnimation();
            }
            else
            {
                /* 패킷 제거로 사용되지 않는 메서드 */
                // SendResponse(0);
            }
        }
    }

    private void OnDisable()
    {
        ResetDisplay();
        isMessageComplete = false;
    }

    private void ResetDisplay()
    {
        txtMessage.text = string.Empty;
        txtContinue.alpha = 0;
        DOTween.KillAll();
    }

    private void CompleteMessageDisplay()
    {
        isMessageComplete = true;
        txtContinue.DOFade(1, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
    }

    private void ConfigureText(string message, bool typingAnimation)
    {
        isMessageComplete = false;
        currentMessage = message;

        ResetDisplay();

        if (typingAnimation)
        {
            txtMessage
                .DOText(message, message.Length / 10f)
                .SetEase(Ease.Linear)
                .OnComplete(CompleteMessageDisplay);
        }
        else
        {
            txtMessage.text = message;
            CompleteMessageDisplay();
        }
    }

    private void SkipTypingAnimation()
    {
        DOTween.KillAll();
        txtMessage.text = currentMessage;
        CompleteMessageDisplay();
    }

    /* 패킷 수정으로 보류된 메서드 */
    // private void SendResponse(int responseCode)
    // {
    //     var response = new C_PlayerResponse { ResponseCode = responseCode };
    //     GameManager.Network.Send(response);
    // }

    private void SetBackgroundColor(byte r, byte g, byte b)
    {
        bg.color = new Color32(r, g, b, 255);
    }

    private void SetTextColor(byte r, byte g, byte b)
    {
        txtMessage.color = new Color32(r, g, b, 255);
    }

    private void SetTextAlignment(int horizontal, int vertical)
    {
        txtMessage.alignment = TextAlignmentOptions.Midline;

        if (horizontal == 0)
        {
            if (vertical == 0)
                txtMessage.alignment = TextAlignmentOptions.TopLeft;
            else if (vertical == 1)
                txtMessage.alignment = TextAlignmentOptions.MidlineLeft;
            else if (vertical == 2)
                txtMessage.alignment = TextAlignmentOptions.BottomLeft;
        }
        else if (horizontal == 1)
        {
            if (vertical == 0)
                txtMessage.alignment = TextAlignmentOptions.Top;
            else if (vertical == 1)
                txtMessage.alignment = TextAlignmentOptions.Midline;
            else if (vertical == 2)
                txtMessage.alignment = TextAlignmentOptions.Bottom;
        }
        else if (horizontal == 2)
        {
            if (vertical == 0)
                txtMessage.alignment = TextAlignmentOptions.TopRight;
            else if (vertical == 1)
                txtMessage.alignment = TextAlignmentOptions.MidlineRight;
            else if (vertical == 2)
                txtMessage.alignment = TextAlignmentOptions.BottomRight;
        }
    }
}
