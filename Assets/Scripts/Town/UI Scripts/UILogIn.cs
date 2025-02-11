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
using Unity.VisualScripting;

public class UILogIn : MonoBehaviour
{
	private static UILogIn _instance;
    public static UILogIn Instance => _instance;
	[SerializeField] private GameObject UIStart;
    private UIStart uiStartCS;
    [SerializeField] private InputField userEmail;
	[SerializeField] private InputField userPW;
	[SerializeField] private InputField userPWC;
	[SerializeField] private Button btn_Back;
	[SerializeField] private Button btn_Register;
	[SerializeField] private Button btn_Confirm;
	[SerializeField] private TMP_Text txt_Title;
	private TMP_Text txt_Error;
	private GameObject txtErrorObj;

	private bool isLogin;

    private string useremail;
    private string userpw;
    private string userpwc;

	// Start is called before the first frame update
	void Awake()
    {
			if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

		uiStartCS = UIStart.GetComponent<UIStart>();

		txtErrorObj = transform.Find("Image/Text_Error").gameObject;
		txt_Error = txtErrorObj.GetComponent<TMP_Text>();
		
		isLogin = true;
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
			txt_Title.text = "로그인";
			Debug.Log($"!!! {_instance}");
            ClearTextField();
			userPWC.gameObject.SetActive(false);
			btn_Register.gameObject.SetActive(true);
		}
    }

    public void RegisterButton()
    {
        // �α��� ���¿����� ǥ��Ǵ� ��ư, ���� ������
        if (!isLogin) return;

        isLogin = false;
        txt_Title.text = "회원가입";
				Debug.Log($"!!! {_instance}");
         ClearTextField();
		userPWC.gameObject.SetActive(true);
		btn_Register.gameObject.SetActive(false);
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
			Debug.Log("로그인 시도 : " + userEmail.text + " " + userPW.text);
			GameManager.Network.Send(dataPacket);
		}
        else // 회원가입 시도
        {
			var dataPacket = new C_Register
			{
				Email = userEmail.text,
				Pw = userPW.text,
				PwCheck = userPWC.text
			};
			Debug.Log("회원가입 시도 : " + userEmail.text + " " + userPW.text + " " + userPWC.text);
			GameManager.Network.Send(dataPacket);
		}
	}

	/// <summary>
	/// Display Packet Message On Login UI
	/// </summary>
	/// <param name="msg"></param>
    public void DisplayMessage(string msg)
    {
		  txt_Error.text = msg;
    }

    private void ClearTextField()
    {
		userEmail.text = string.Empty;
		userPW.text = string.Empty;
		userPWC.text = string.Empty;

		txt_Error.text = string.Empty;
	}

    public void CheckHasChar(object[] ownedCharacters)
    {
		if (ownedCharacters.Length > 0)   // this Account has Character Already
		{
			var character = ownedCharacters[0] as Google.Protobuf.Protocol.OwnedCharacters;
			gameObject.SetActive(false);
			UIStart.SetActive(false);
			TownManager.Instance.GameStart(character.Nickname,character.Class-1001,"127.0.0.1");

		}
		else // this Account has to create Character
		{
			gameObject.SetActive(false);
			UIStart.SetActive(true);
			uiStartCS.SetNicknameUI();
		}
	}
}
