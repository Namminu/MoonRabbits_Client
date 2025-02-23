using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingItem : MonoBehaviour
{
	public HousingItemData HsItemData { get; private set; }
	private GameObject spawnObject;

	public void Initialize(HousingItemData data)
	{
		HsItemData = data;
	}

	/// <summary>
	/// UI의 2D 오브젝트를 클릭하여 마우스 위치에 미리보기 할당
	/// </summary>
	public void PreviewItem(Vector3 position)
	{
		Debug.Log("하우징 아이템 미리보기" + HsItemData.ItemName);

	}

	/// <summary>
	/// 씬의 원하는 위치에 클릭하여 3D 오브젝트 배치
	/// </summary>
	public void PlaceItem(Vector3 position) 
	{
		Debug.Log("하우징 아이템 배치" + position + HsItemData.ItemName);
		if(spawnObject != null)
		{
			spawnObject.transform.position = position;
			spawnObject = null;
		}
	}
}