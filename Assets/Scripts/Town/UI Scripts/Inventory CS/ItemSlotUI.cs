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
	[SerializeField] [ReadOnly] private int itemCount;
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

	/// <summary>
	/// Add Item to Slot Method
	/// </summary>
	public void AddItem(Item insertItem, int insertItemCount = 1)
    {
        item = insertItem;
        itemCount = insertItemCount;
        /* //���� UI�� ������ �̹����� �߰��ϴ� ���� - Item �ڵ� �ϼ� �� DB �������� �ʿ�
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


	#region Getter
	public Item GetItem() {  return item; }

	public bool HasItem() {  return item != null; }
	#endregion
}
