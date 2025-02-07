using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TMPro;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

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
	[SerializeField] private TMP_Text txt_Error;

	private bool isLogin;

    private string useremail;
    private string userpw;
    private string userpwc;

	// Start is called before the first frame update
	void Awake()
    {
		uiStartCS = UIStart.GetComponent<UIStart>();

		isLogin = true;    // true : 로그인 상태, false : 회원가입 상태
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButton()
    {
        if(isLogin) // 현재 로그인 상태에서 뒤로가기
        {
            UIStart.gameObject.SetActive(true);

            gameObject.SetActive(false);
        }
        else // 회원가입 상태에서 뒤로가기
        {
            isLogin = true;
			txt_Title.text = "로그인";
            ClearInputField();
			userPWC.gameObject.SetActive(false);
			btn_Reigster.gameObject.SetActive(true);
		}
    }

    public void RegisterButton()
    {
        // 로그인 상태에서만 표기되는 버튼, 에러 방지용
        if (!isLogin) return;

        isLogin = false;
        txt_Title.text = "회원가입";
        ClearInputField();
		userPWC.gameObject.SetActive(true);
		btn_Reigster.gameObject.SetActive(false);
	}

    public void ConfirmButton()
    {
        if(isLogin) // 로그인 상태에서 확인 버튼
        {
			//var dataPacket = new C_Login
			//{
			//	Email = userEmail.text,
			//	Password = userPW.text
			//};
			//GameManager.Network.Send(dataPacket);

			// Temp Code
			string userData = userEmail.text + userPW.text;
			Debug.Log("로그인 시도 : " + userData);
            CheckHasChar(); //계정 내 캐릭터 상태 체크
		}
        else //회원가입 상태에서 확인버튼
        {
			//var dataPacket = new C_Register
			//{
			//	Email = userEmail.text,
			//	Password = userPW.text,
			//	ConfirmPassword = userPWC.text
			//};
			//GameManager.Network.Send(dataPacket);

			// Temp Code
			string userData = userEmail.text + userPW.text + userPWC.text;
			Debug.Log("회원가입 시도 : " + userData);
            DisplayMessage("회원가입 됐어용");
		}
	}

    public void DisplayMessage(string msg)
    {
		txt_Error.text = msg;
    }

    private void ClearInputField()
    {
		userEmail.text = string.Empty;
		userPW.text = string.Empty;
		userPWC.text = string.Empty;
	}

    private void CheckHasChar()
    {
		if (true)   // 계정 내 보유 캐릭터가 존재하지 않을 경우
		{
			UIStart.SetActive(true);
			uiStartCS.SetNicknameUI();
			 
			gameObject.SetActive(false);
		}
		//else // 계정 내 생성해둔 캐릭터가 존재할 경우
		//{
		//	TownManager.Instance.GameStart(serverUrl, port, nickname, classIdx);
		//	gameObject.SetActive(false);
		//	UIStart.SetActive(false);
		//}
	}
}
