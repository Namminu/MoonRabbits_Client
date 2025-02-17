using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text p_Text;
	[SerializeField] private Button btn_Yes;
	[SerializeField] private Button btn_No;

	private string eventTrigger;

	private void Awake()
	{
		btn_Yes.onClick.AddListener(YesButton);
		btn_No.onClick.AddListener(() => gameObject.SetActive(false));
	}

	public int SetPopupUI(string _text, string eventName)
	{
		Debug.Log("팝업UI 호출됨");
		gameObject.SetActive(true);

		p_Text.text = _text.ToString();
		eventTrigger = eventName;

		return 0;
	}

	private void YesButton()
	{
		if (!string.IsNullOrEmpty(eventTrigger)) // 이벤트가 저장되어 있으면 실행
		{
			Debug.Log($"[PopupUI] '{eventTrigger}' 이벤트 실행");
			EventManager.Trigger(eventTrigger); // 저장된 이벤트 실행
			gameObject.SetActive(false);
		}
	}
}
