using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINameChat : MonoBehaviour
{
    [SerializeField] private Mask bgMask;
    [SerializeField] private Image imgBg;
    [SerializeField] private TMP_Text txtNickname;
    [SerializeField] private TMP_Text txtChat;

    private Vector2 originBgSize;
    private Transform camTransform;
    private string userName;

    private readonly List<MsgData> msgList = new();
    private float messageTimer;
    private const float MessageHoldingTime = 5f;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        AlignWithCamera();
        ProcessMessageQueue();
    }

    public void SetName(string userName)
    {
        this.userName = userName;
        txtNickname.text = userName;

        originBgSize = txtNickname.GetPreferredValues() + new Vector2(70, 35);
        txtNickname.rectTransform.sizeDelta = originBgSize;
        txtChat.rectTransform.sizeDelta = new Vector2(originBgSize.x, 0);
    }

    public void PushText(string text)
    {
        if (msgList.Count == 0)
        {
            messageTimer = 0;
        }

        msgList.Add(new MsgData(messageTimer + MessageHoldingTime, text));
        UpdateChatText();
    }

    private void AlignWithCamera()
    {
        transform.rotation = camTransform.rotation;
    }

    private void ProcessMessageQueue()
    {
        if (msgList.Count > 0)
        {
            messageTimer += Time.deltaTime;

            if (msgList[0].Time < messageTimer)
            {
                msgList.RemoveAt(0);
                UpdateChatText();
            }
        }
    }

    private void UpdateChatText()
    {
        if (msgList.Count == 0)
        {
            ResetChatDisplay();
            return;
        }

        txtChat.DOFade(1, 0.2f);

        StringBuilder chatBuilder = new();
        foreach (var msg in msgList)
        {
            chatBuilder.AppendLine(msg.Text);
        }

        txtChat.text = chatBuilder.ToString();

        float chatWidth = Mathf.Min(txtChat.preferredWidth, 1000);
        Vector2 preferredSize = txtChat.GetPreferredValues(txtChat.text, chatWidth, 60);

        txtChat.rectTransform.DOSizeDelta(new Vector2(chatWidth, preferredSize.y), 0.2f);
        bgMask.enabled = true;
    }

    private void ResetChatDisplay()
    {
        Vector2 zeroHeight = originBgSize;
        zeroHeight.y = 0;

        txtChat.rectTransform.DOSizeDelta(zeroHeight, 0.2f);
        txtChat.DOFade(0, 0.1f).OnComplete(() => txtChat.text = string.Empty);
    }

    private struct MsgData
    {
        public float Time;
        public string Text;

        public MsgData(float time, string text)
        {
            Time = time;
            Text = text;
        }
    }
}