using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Item Info")] 
    private Item item;  //획득 아이템 객체
    [SerializeField] private TMP_Text text_ItemAmount;  //아이템 수량
	[ReadOnly] public int itemCount;
	[SerializeField] private Image itemImage;    //아이템 이미지

    [Space] // 아이템 하이라이트
	[SerializeField] private Image itemHighLighter;

    // Start is called before the first frame update
    void Start()
    {
        // DB에서 인벤토리 정보 받아오는 과정..?
        // AddItem(DB에서 받아온 정보)?
    }

	// 아이템 등록 시 이미지 객체의 투명도 조절을 위한 메서드
	private void SetItemImageAlpha(float alpha)
	{
		Color newColor = itemImage.color;
		newColor.a = alpha;
		itemImage.color = newColor;
	}

	/// <summary>
	/// Add Item to Slot Method
	/// </summary>
	public void AddItem(Item insertItem, int insertItemCount = 1)
    {
        item = insertItem;
        itemCount = insertItemCount;
        /* //슬롯 UI에 아이템 이미지를 추가하는 과정 - Item 코드 완성 및 DB 연동과정 필요
		itemImage.sprite = insertItem.GetItemImage().sprite; */

        text_ItemAmount.text = insertItemCount.ToString();

        SetItemImageAlpha(1);
	}

	/// <summary>
	/// Update Item Count Method, When Count Under/Equal 0, auto call ClearSlot()
	/// </summary>
	public int UpdateItemCount(int newItemCount)
	{
		itemCount += newItemCount;
		if (itemCount <= 0)
		{
			ClearSlot();
			return -1;
		}
		text_ItemAmount.text = itemCount.ToString();
		return itemCount;
	}

	/// <summary>
	/// Clear Item Slot Method When Item Count <= 0
	/// </summary>
	private void ClearSlot()
	{
		item = null;
		itemCount = 0;
		itemImage.sprite = null;
		SetItemImageAlpha(0);

		text_ItemAmount.text = string.Empty;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		itemHighLighter.gameObject.SetActive(true);
	}
    public void OnPointerExit(PointerEventData eventData)
	{
		itemHighLighter.gameObject.SetActive(false);
	}
}
