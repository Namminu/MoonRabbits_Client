using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HSItemViewButtonControl : MonoBehaviour
{
	[Header("ItemList UI")]
	[SerializeField] private GameObject HSItemUI;

	[Header("Buttons")]
	[SerializeField] private Button btn_Close;
	[SerializeField] private Button btn_Open;

	[Header("UI Toggle Prop")]
	[SerializeField] float duration = 0.5f;

	private float initYPos;
	private bool isExpand;

	private void Awake()
	{
		initYPos = HSItemUI.transform.position.y;
		isExpand = true;

		btn_Close.onClick.AddListener(UISlideUpDown);
		btn_Open.onClick.AddListener(UISlideUpDown);

		btn_Open.gameObject.SetActive(false);
	}

	private void UISlideUpDown() => StartCoroutine(UISlide());

	private IEnumerator UISlide()
	{
		float startYPos = HSItemUI.transform.position.y;
		float targetYPos = isExpand ? -initYPos : initYPos;
		float elapsedTime = 0f;

		while(elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float newY = Mathf.Lerp(startYPos, targetYPos, elapsedTime / duration);
			HSItemUI.transform.position = new Vector3(HSItemUI.transform.position.x, newY, HSItemUI.transform.position.z);
			
			yield return null;
		}
		HSItemUI.transform.position = new Vector3(HSItemUI.transform.position.x, targetYPos, HSItemUI.transform.position.z);

		isExpand = !isExpand;
		btn_Close.gameObject.SetActive(isExpand);
		btn_Open.gameObject.SetActive(!isExpand);
	}
}
