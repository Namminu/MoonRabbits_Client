using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scroll;

    [SerializeField]
    private RectTransform rectBg;

    [SerializeField]
    private Transform chatItemRoot;

    [SerializeField]
    private TMP_Text txtChatItemBase;

    [SerializeField]
    private TMP_InputField inputChat;

    [SerializeField]
    private Button btnSend;

    [SerializeField]
    private Button btnToggle;

    [SerializeField]
    private Transform icon;

    [SerializeField]
    private Transform alarm;

    private float baseChatItemWidth;
    private Player player;
    public Player Player
    {
        get { return player; }
        set { player = value; }
    }
    private bool isOpen = true;
    public TMP_Dropdown chatType;

    void Start()
    {
        if (chatType == null)
        {
            Debug.LogError(
                "Dropdown(chatType)이 null입니다. 인스펙터에서 할당되었는지 확인하세요."
            );
            return;
        }

        inputChat.text = "";

        baseChatItemWidth = txtChatItemBase.rectTransform.sizeDelta.x;

        btnSend.onClick.AddListener(SendMessage);
        btnToggle.onClick.AddListener(ToggleChatWindow);

        inputChat.onSubmit.AddListener(
            (text) =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    SendMessage();
                }
            }
        );

        chatType.value = 0;
        chatType.RefreshShownValue();

        // 이벤트 리스너 추가
        chatType.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int selectedIndex)
    {
        string selectedText = chatType.options[selectedIndex].text;
        Debug.Log("선택된 옵션: " + selectedText);

        // 선택된 채팅 타입에 따라 UI 변경
        if (selectedText == "전체")
        {
            inputChat.placeholder.GetComponent<TMP_Text>().text = "전체 채팅 입력";
        }
        else if (selectedText == "파티")
        {
            inputChat.placeholder.GetComponent<TMP_Text>().text = "파티 채팅 입력";
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputChat.IsActive() && inputChat.isFocused)
            {
                SendMessage();
            }
            else
            {
                ActivateInputFieldProperly();
            }
        }
    }

    private void ToggleChatWindow()
    {
        if (isOpen)
        {
            rectBg.DOSizeDelta(new Vector2(100, 40), 0.3f);
            icon.DORotate(new Vector3(0, 0, 180), 0.3f);
        }
        else
        {
            alarm.gameObject.SetActive(false);
            rectBg.DOSizeDelta(new Vector2(550, 500), 0.3f);
            icon.DORotate(new Vector3(0, 0, 0), 0.3f);
        }

        isOpen = !isOpen;
    }

    public void SendMessage()
    {
        inputChat.DeactivateInputField();

        if (string.IsNullOrWhiteSpace(inputChat.text))
            return;

        if (chatType == null || chatType.options.Count == 0)
        {
            Debug.LogError("Dropdown(chatType)이 null이거나 옵션이 없습니다.");
            return;
        }

        int selectedIndex = chatType.value;
        string selectedText = chatType.options[selectedIndex].text;

        player.SendChatMessage(inputChat.text, selectedText);

        inputChat.text = string.Empty;
        ActivateInputFieldProperly();
    }

    private void ActivateInputFieldProperly()
    {
        inputChat.ActivateInputField();
        inputChat.caretPosition = 0;
        ResetIME();
    }

    public void PushMessage(string nickName, string msg, string chatType, bool myChat)
    {
        if (!isOpen)
        {
            alarm.gameObject.SetActive(true);
            alarm.DOShakePosition(1f, 10);
        }

        StopAllCoroutines();

        var msgItem = Instantiate(txtChatItemBase, chatItemRoot);
        // msgItem.color = myChat ? Color.green : Color.white;
        // msgItem.text = $"[{nickName}] {msg}";
        if (chatType == "System")
        {
            msgItem.color = Color.white;
            msgItem.text = $"[System] {msg}";
        }
        else if (chatType == "전체")
        {
            msgItem.color = Color.green;
            msgItem.text = $"[전체] {nickName} : {msg}";
        }
        else if (chatType == "파티")
        {
            msgItem.color = Color.cyan;
            msgItem.text = $"[파티] {nickName} : {msg}";
        }
        msgItem.gameObject.SetActive(true);

        StartCoroutine(AdjustTextSize(msgItem));
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        scroll.value = 0;
    }

    private IEnumerator AdjustTextSize(TMP_Text textComp)
    {
        yield return new WaitForEndOfFrame();

        if (textComp.textInfo.lineCount > 1)
        {
            textComp.rectTransform.sizeDelta = new Vector2(
                baseChatItemWidth,
                textComp.preferredHeight + 12
            );
        }
    }

    private void ResetIME()
    {
        Input.imeCompositionMode = IMECompositionMode.Off;
        Input.imeCompositionMode = IMECompositionMode.On;
    }
}
