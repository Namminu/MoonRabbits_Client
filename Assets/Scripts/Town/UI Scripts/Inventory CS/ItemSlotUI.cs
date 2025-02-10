using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Header("Item Info")] 
    private Item item;  //ȹ�� ������ ��ü
    [SerializeField] private TMP_Text text_ItemAmount;  //������ ����
    private int itemCount;
	[SerializeField] private Image itemImage;    //������ �̹���
    
    //[SerializeField] private GameObject itemHighlighter;    // ������ ���̶���Ʈ

    // Start is called before the first frame update
    void Start()
    {
        // DB���� �κ��丮 ���� �޾ƿ��� ����..?
        // AddItem(DB���� �޾ƿ� ����)?
    }

    // ������ ��� �� �̹��� ��ü�� ���� ������ ���� �޼���
    private void SetItemImageColor(float alpha)
    {
        Color newColor = itemImage.color;
        newColor.a = alpha;
        itemImage.color = newColor;
	}

    // ������ ��� �޼���
    public void AddItem(Item insertItem, int insertItemCount = 1)
    {
        item = insertItem;
        itemCount = insertItemCount;
        /* ���� UI�� ������ �̹����� �߰��ϴ� ���� - Item �ڵ� �ϼ� �� DB �������� �ʿ�
		//itemImage.sprite = insertItem.itemImage;  */

        text_ItemAmount.text = insertItemCount.ToString();

		SetItemImageColor(1);
	}
}
