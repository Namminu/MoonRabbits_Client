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
	/// UI�� 2D ������Ʈ�� Ŭ���Ͽ� ���콺 ��ġ�� �̸����� �Ҵ�
	/// </summary>
	public void PreviewItem(Vector3 position)
	{
		Debug.Log("�Ͽ�¡ ������ �̸�����" + HsItemData.ItemName);

	}

	/// <summary>
	/// ���� ���ϴ� ��ġ�� Ŭ���Ͽ� 3D ������Ʈ ��ġ
	/// </summary>
	public void PlaceItem(Vector3 position) 
	{
		Debug.Log("�Ͽ�¡ ������ ��ġ" + position + HsItemData.ItemName);
		if(spawnObject != null)
		{
			spawnObject.transform.position = position;
			spawnObject = null;
		}
	}
}