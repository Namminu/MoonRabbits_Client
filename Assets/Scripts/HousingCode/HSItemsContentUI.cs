using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSItemsContentUI : MonoBehaviour
{
	[SerializeField] private GameObject HSItemPrefab;

	private async void Awake()
	{
		/* Json ���� �ε� �ӽ� �ڵ� */
		await ItemDataLoader.GenerateAllItems();
	}
}
