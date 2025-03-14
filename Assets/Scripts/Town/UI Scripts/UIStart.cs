using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    #region server
    [SerializeField]
    private GameObject serverImage;

    [SerializeField]
    private TMP_InputField inputIp;

    [SerializeField]
    private TMP_InputField inputPort;

    [SerializeField]
    private Button btnEnter;
    #endregion

    #region nickname
    [SerializeField]
    private GameObject NicknameImage;

    [SerializeField]
    private GameObject charList;

    [SerializeField]
    private Button[] charBtns;

    [SerializeField]
    private Button localServerBtn;

    [SerializeField]
    private Button btnConfirm;

    [SerializeField]
    private Button btnBack;

    [SerializeField]
    private TMP_InputField inputNickname;

    [SerializeField]
    private TMP_Text txtMessage;

    [SerializeField]
    private GameObject UILogin;

    [SerializeField]
    private TMP_Text placeHolder;
    #endregion

    private int classIdx = 0;
    private string serverUrl;
    private string nickname;
    private string port;

    private const string DefaultServerMessage = "Input Server";
    private const string DefaultNicknameMessage = "닉네임 (2~10글자)";
    private const string ShortNicknameError = "이름을 2글자 이상 입력해주세요!";
    private const string LongNicknameError = "이름을 10글자 이하로 입력해주세요!";

    void Awake()
    {
        // placeHolder = inputNickname.placeholder.GetComponent<TMP_Text>();
        btnBack.onClick.AddListener(SetServerUI);
        localServerBtn.onClick.AddListener(OnClickLocalServer);
        btnEnter.onClick.AddListener(ConfirmServer);
        btnConfirm.onClick.AddListener(ConfirmNickname);
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

    /*    void OnEnable()
        {
            // 씬이 활성화 될 때 네트워크 상태를 확인하는 코루틴 시작
            StartCoroutine(CheckConnectionCoroutine());
        }*/

    /*    private IEnumerator CheckConnectionCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
    
                // 예시로 GameManager.Network의 IsConnected 값을 통해 연결 상태를 파악합니다.
                // 실제 구현에서는 본인의 네트워크 관리 객체에 맞게 연결 상태를 확인하세요.
                if (GameManager.Network != null && !GameManager.Network.IsConnected)
                {
                    Debug.Log("서버 연결이 끊어져 게임을 재시작합니다.");
                    // 현재 씬을 다시 로드하여 상태 초기화 실행
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    // 코루틴 종료
                    yield break;
                }
            }
        }
    */
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
        serverImage.SetActive(true);
        NicknameImage.SetActive(false);
#if UNITY_EDITOR
        inputIp.gameObject.SetActive(true);
        inputPort.gameObject.SetActive(true);
        localServerBtn.gameObject.SetActive(true);
#else
        inputIp.gameObject.SetActive(false);
        inputPort.gameObject.SetActive(false);
        localServerBtn.gameObject.SetActive(false);
#endif
    }

    public void SetNicknameUI()
    {
        serverImage.SetActive(false);
        NicknameImage.SetActive(true);
        placeHolder.text = DefaultNicknameMessage;
        inputNickname.text = string.Empty;
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
        if (string.IsNullOrWhiteSpace(inputIp.text) && !string.IsNullOrEmpty(inputIp.text))
        {
            DisplayError(DefaultServerMessage);
            return;
        }

        serverUrl = inputIp.text;
        port = inputPort.text;
        TownManager.Instance.TryConnectToServer(serverUrl, port);
        gameObject.SetActive(false);
        UILogin.SetActive(true);
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

        /* 캐릭터 생성 패킷 전송 필요
                var dataPacket = C_Enter?
                {
                    Nickname = nickname,
                    Class = classIdx
                };
                GameManager.Network.Send(dataPacket);
                */

        nickname = inputNickname.text;

        var dataPacket = new C2SCreateCharacter
        {
            Nickname = nickname,
            ClassCode = classIdx + 1001,
        };
        GameManager.Network.Send(dataPacket);

        // GameManager.Instance.UserName = nickname;
        // GameManager.Instance.ClassIdx = classIdx;

        // TownManager.Instance.GameStart(nickname, classIdx);

        gameObject.SetActive(false);
    }

    private void DisplayError(string errorMessage)
    {
        txtMessage.text = errorMessage;
        txtMessage.color = UnityEngine.Color.red;
    }

    /// <summary>
    /// Return nickname, classIdx, serverUrl, port
    /// </summary>
    public (string nickname, int classIdx, string serverUrl, string port) GetSomeInfo()
    {
        return (nickname, classIdx, serverUrl, port);
    }
}
