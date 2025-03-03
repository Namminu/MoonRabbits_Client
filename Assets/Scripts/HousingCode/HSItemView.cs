using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HSItemView : MonoBehaviour
{
	[SerializeField] private Button btn_Close;

	private void Awake()
	{
		btn_Close.onClick.AddListener(ToggleUIPanel);
	}

	private void ToggleUIPanel()
	{

	}
}
