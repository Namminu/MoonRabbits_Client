using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeperateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField stackInputField;
	[SerializeField] private Button btn_Confirm;
	[SerializeField] private Button btn_Refuse;
	[SerializeField] private Button btn_UpStack;
	[SerializeField] private Button btn_DownStack;

	private int curItemStack;
	private int totalItemStack;

	// Start is called before the first frame update
	void Awake()
    {
		stackInputField.onValueChanged.AddListener(OnStackInputChanged);

		btn_Confirm.onClick.AddListener(ConfirmSeperateItem);
		btn_Refuse.onClick.AddListener(CloseUI);

		btn_UpStack.onClick.AddListener(ItemStackUp);
		btn_DownStack.onClick.AddListener(ItemStackDown);

		CloseUI();
	}

	public void Seperating(int itemStack)
	{
		gameObject.SetActive(true);

		totalItemStack = itemStack;
		curItemStack = 1;
	}


	private void OnStackInputChanged(string input)
	{
		if(int.TryParse(input, out int parsedValue))
		{
			parsedValue = Mathf.Clamp(parsedValue, 1, totalItemStack - 1);
			curItemStack = parsedValue;

			stackInputField.text = curItemStack.ToString();
		}
		else stackInputField.text = curItemStack.ToString();
	}
	private void ItemStackUp()
	{
		if (curItemStack >= totalItemStack -1) return;
		
		curItemStack++;
		stackInputField.text = curItemStack.ToString();
	}
	private void ItemStackDown()
	{
		if (curItemStack <= 1) return;

		curItemStack--;
		stackInputField.text = curItemStack.ToString();
	}

	/// <returns>Seperated Item Stack </returns>
	private void ConfirmSeperateItem()
	{
		EventManager.Trigger<int>("OnSeperationConfirm", curItemStack);
		gameObject.SetActive(false);
	}
	private void CloseUI() => gameObject.SetActive(false);
}
