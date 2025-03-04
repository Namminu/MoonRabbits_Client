using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSItemsContentUI : MonoBehaviour
{
	[SerializeField] private GameObject HSItemPrefab;
	[SerializeField][ReadOnly] List<GameObject> HSItems;

	private async void Awake()
	{
		/* Json 파일 로드 임시 코드 */
		await ItemDataLoader.GenerateAllItems();

		CreateItemIconButton();
	}

	#region Private Method
	private int CreateItemIconButton()
	{
		foreach(var hsItem in ItemDataLoader.HousingItemsList)
		{
			GameObject newHSItem = Instantiate(HSItemPrefab, this.transform);
			HSItems.Add(newHSItem);
			if(HSItemPrefab.TryGetComponent<HSItemButton>(out var itemBtnCode))
			{
				itemBtnCode.InitializeButton(hsItem.ItemIcon, hsItem.ItemId);
			}
		}
		return -1;
	}
	#endregion
}
