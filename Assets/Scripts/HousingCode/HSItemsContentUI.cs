using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSItemsContentUI : MonoBehaviour
{
	[SerializeField] private GameObject HSItemPrefab;

	private async void Awake()
	{
		/* Json 파일 로드 임시 코드 */
		await ItemDataLoader.GenerateAllItems();
	}
}
