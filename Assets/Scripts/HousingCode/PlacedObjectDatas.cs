using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacedObjectDatas
{
	public int ItemId;
	public ObjectTransInfo ItemTransInfo;
	public int DataType;

	public PlacedObjectDatas(int itemId, ObjectTransInfo itemTransInfo, int dataType)
	{
		ItemId = itemId;
		ItemTransInfo = itemTransInfo;
		DataType = dataType;
	}
}

[System.Serializable]
public class ObjectTransInfo
{
	public Vector3Int ItemPosition;
	public float ItemYRotation;

	public ObjectTransInfo(Vector3Int itemPosition, float itemYRotation)
	{
		ItemPosition = itemPosition;
		ItemYRotation = itemYRotation;
	}
}
