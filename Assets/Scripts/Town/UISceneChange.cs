using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISceneChange : MonoBehaviour
{
	[SerializeField] private Button btn_Yes;
	[SerializeField] private Button btn_No;

	private void Awake()
	{
		btn_Yes.onClick.AddListener(OnClickYesBtn);
		btn_No.onClick.AddListener(OnClickNoBtn);
	}

	private void OnClickYesBtn()
	{
		EventManager.Trigger("OnChangeScene");
	}

	private void OnClickNoBtn()
	{
		Destroy(gameObject);
	}
}
