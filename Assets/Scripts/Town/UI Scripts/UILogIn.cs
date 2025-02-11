using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TMPro;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;
using Google.Protobuf.Protocol;

public class UILogIn : MonoBehaviour
{
	[SerializeField] private GameObject UIStart;
    private UIStart uiStartCS;
    [SerializeField] private InputField userEmail;
	[SerializeField] private InputField userPW;
	[SerializeField] private InputField userPWC;
	[SerializeField] private Button btn_Back;
	[SerializeField] private Button btn_Reigster;
	[SerializeField] private Button btn_Confirm;
	[SerializeField] private TMP_Text txt_Title;
	[SerializeField] private static TMP_Text txt_Error;

	private bool isLogin;

    private string useremail;
    private string userpw;
    private string userpwc;

	// Start is called before the first frame update
	void Awake()
    {
		uiStartCS = UIStart.GetComponent<UIStart>();

		isLogin = true;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButton()
    {
        if(isLogin) // ���� �α��� ���¿��� �ڷΰ���
        {
            UIStart.gameObject.SetActive(true);

            gameObject.SetActive(false);
        }
        else // ȸ������ ���¿��� �ڷΰ���
        {
            isLogin = true;
			txt_Title.text = "�α���";
            ClearInputField();
			userPWC.gameObject.SetActive(false);
			btn_Reigster.gameObject.SetActive(true);
		}
    }

    public void RegisterButton()
    {
        // �α��� ���¿����� ǥ��Ǵ� ��ư, ���� ������
        if (!isLogin) return;

        isLogin = false;
        txt_Title.text = "ȸ������";
        ClearInputField();
		userPWC.gameObject.SetActive(true);
		btn_Reigster.gameObject.SetActive(false);
	}

    public void ConfirmButton()
    {
        if(isLogin) // 로그인 시도
		{
			var dataPacket = new C_Login
			{
				Email = userEmail.text,
				Pw = userPW.text
			};
			GameManager.Network.Send(dataPacket);

			// Temp Code
			string userData = userEmail.text + userPW.text;
			Debug.Log("로그인 시도 : " + userData);

            CheckHasChar(true); 
		}
        else // 회원가입 시도
        {
			var dataPacket = new C_Register
			{
				Email = userEmail.text,
				Pw = userPW.text,
				PwCheck = userPWC.text
			};
			GameManager.Network.Send(dataPacket);

			// Temp Code
			string userData = userEmail.text + userPW.text + userPWC.text;
			Debug.Log("회원가입 시도 : " + userData);

            DisplayMessage("회원가입 결과 메세지");
		}
	}

    public static void DisplayMessage(string msg)
    {
		  txt_Error.text = msg;
    }

    private void ClearInputField()
    {
		userEmail.text = string.Empty;
		userPW.text = string.Empty;
		userPWC.text = string.Empty;
	}

    private void CheckHasChar(bool hasChar)
    {
		if (hasChar)   // this Account has Character Already
		{
			//TownManager.Instance.GameStart(serverUrl, port, nickname, classIdx);
			gameObject.SetActive(false);
			UIStart.SetActive(false);
		}
		else // this Account has to create Character
		{
			UIStart.SetActive(true);
			uiStartCS.SetNicknameUI();

			gameObject.SetActive(false);
		}
	}
}
