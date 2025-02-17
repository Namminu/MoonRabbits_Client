using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Item Info")] 
    private Item item;  //ȹ�� ������ ��ü
    [SerializeField] private TMP_Text text_ItemAmount;  //������ ����
    private int itemCount;
	[SerializeField] private Image itemImage;    //������ �̹���

    [Space] // ������ ���̶���Ʈ
	[SerializeField] private Image itemHighLighter;

    // Start is called before the first frame update
    void Start()
    {
        // DB���� �κ��丮 ���� �޾ƿ��� ����..?
        // AddItem(DB���� �޾ƿ� ����)?
    }

	// ������ ��� �� �̹��� ��ü�� ������ ������ ���� �޼���
	private void SetItemImageAlpha(float alpha)
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

        SetItemImageAlpha(1);
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
