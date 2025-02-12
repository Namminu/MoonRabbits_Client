using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    [SerializeField] private GameObject charList;
    [SerializeField] private Button[] charBtns;
    [SerializeField] private Button localServerBtn;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnBack;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private TMP_InputField inputPort;
    [SerializeField] private TMP_Text txtMessage;

    [SerializeField] private GameObject UILogin;

    private TMP_Text placeHolder;

    private int classIdx = 0;
    private string serverUrl;
    private string nickname;
    private string port;

    private const string DefaultServerMessage = "Input Server";
    private const string DefaultNicknameMessage = "닉네임 (2~10글자)";
    private const string WelcomeMessage = "Welcome!";
    private const string ShortNicknameError = "이름을 2글자 이상 입력해주세요!";
    private const string LongNicknameError = "이름을 10글자 이하로 입력해주세요!";

    void Awake()
    {
		placeHolder = inputNickname.placeholder.GetComponent<TMP_Text>();
        btnBack.onClick.AddListener(SetServerUI);
        localServerBtn.onClick.AddListener(OnClickLocalServer);
        SetServerUI();
        InitializeCharacterButtons();
    }

    private void Update()
    { 
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputNickname.IsActive())
                btnConfirm.onClick.Invoke();
        }
    }

    private void InitializeCharacterButtons()
    {
        for (int i = 0; i < charBtns.Length; i++)
        {
            int idx = i;
            charBtns[i].onClick.AddListener(() => SelectCharacter(idx));
        }
    }

    private void SelectCharacter(int idx)
    {
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(false);
        classIdx = idx;
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetServerUI()
    {
        UpdateUI(WelcomeMessage, Color.white, DefaultServerMessage, false, true);
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(ConfirmServer);
    }

    public void SetNicknameUI()
    {
        UpdateUI(WelcomeMessage, Color.white, DefaultNicknameMessage, true, false);
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(ConfirmNickname);
    }

    private void UpdateUI(string message, Color messageColor, string placeholderText, bool showCharList, bool showPortInput)
    {
        txtMessage.text = message;
        txtMessage.color = messageColor;

        placeHolder.text = placeholderText;
        inputNickname.text = string.Empty;

        charList.SetActive(showCharList);
        btnBack.gameObject.SetActive(showCharList);
        localServerBtn.gameObject.SetActive(showPortInput);
        inputPort.gameObject.SetActive(showPortInput);
    }

    private void OnClickLocalServer()
    {
        serverUrl = "127.0.0.1";
        port = "3000";
		TownManager.Instance.TryConnectToServer(serverUrl, port);
		gameObject.SetActive(false);
		UILogin.SetActive(true);
    }

    private void ConfirmServer()
    {
        if (string.IsNullOrWhiteSpace(inputNickname.text))
        {
            DisplayError(DefaultServerMessage);
            return;
        }

        serverUrl = inputNickname.text;
        port = inputPort.text;
		TownManager.Instance.TryConnectToServer(serverUrl, port);
		SetNicknameUI();
    }

    private void ConfirmNickname()
    {
        if (inputNickname.text.Length < 2)
        {
            DisplayError(ShortNicknameError);
            return;
        }

        if (inputNickname.text.Length > 10)
        {
            DisplayError(LongNicknameError);
            return;
        }

        nickname = inputNickname.text;
		TownManager.Instance.GameStart(nickname, classIdx);
		/* 캐릭터 생성 패킷 전송 필요
		var dataPacket = C_Enter?
        {
            Nickname = nickname,
            Class = classIdx
        };
        GameManager.Network.Send(dataPacket);
        */
		gameObject.SetActive(false);
    }

    private void DisplayError(string errorMessage)
    {
        txtMessage.text = errorMessage;
        txtMessage.color = Color.red;
    }

    /// <summary>
    /// Return nickname, classIdx, serverUrl, port
    /// </summary>
	public (string nickname, int classIdx, string serverUrl, string port) GetSomeInfo()
	{
		return (nickname, classIdx, serverUrl, port);
	}

}
