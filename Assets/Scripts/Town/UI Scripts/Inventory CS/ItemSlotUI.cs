using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Header("Item Info")] 
    private Item item;  //획득 아이템 객체
    [SerializeField] private TMP_Text text_ItemAmount;  //아이템 수량
    private int itemCount;
	[SerializeField] private Image itemImage;    //아이템 이미지
    
    //[SerializeField] private GameObject itemHighlighter;    // 아이템 하이라이트

    // Start is called before the first frame update
    void Start()
    {
        // DB에서 인벤토리 정보 받아오는 과정..?
        // AddItem(DB에서 받아온 정보)?
    }

    // 아이템 등록 시 이미지 객체의 투명도 조절을 위한 메서드
    private void SetItemImageColor(float alpha)
    {
        Color newColor = itemImage.color;
        newColor.a = alpha;
        itemImage.color = newColor;
	}

    // 아이템 등록 메서드
    public void AddItem(Item insertItem, int insertItemCount = 1)
    {
        item = insertItem;
        itemCount = insertItemCount;
        /* 슬롯 UI에 아이템 이미지를 추가하는 과정 - Item 코드 완성 및 DB 연동과정 필요
		//itemImage.sprite = insertItem.itemImage;  */

        text_ItemAmount.text = insertItemCount.ToString();

		SetItemImageColor(1);
	}
}
