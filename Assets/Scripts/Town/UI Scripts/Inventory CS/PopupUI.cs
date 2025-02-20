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
		Debug.Log("�˾�UI ȣ���");
		gameObject.SetActive(true);

		p_Text.text = _text.ToString();
		eventTrigger = eventName;

		return 0;
	}

	private void YesButton()
	{
		if (!string.IsNullOrEmpty(eventTrigger)) // �̺�Ʈ�� ����Ǿ� ������ ����
		{
			Debug.Log($"[PopupUI] '{eventTrigger}' �̺�Ʈ ����");
			EventManager.Trigger(eventTrigger); // ����� �̺�Ʈ ����
			gameObject.SetActive(false);
		}
	}
}
