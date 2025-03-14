using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


public class PlacedObjectDatas
{
	public int ItemId;
	public ObjectTransInfo ItemTrans;
	public int DataType;

	public PlacedObjectDatas(int itemId, ObjectTransInfo itemTrans, int dataType)
	{
		ItemId = itemId;
		ItemTrans = itemTrans;
		DataType = dataType;
	}
}

[System.Serializable]
public class ObjectTransInfo
{
	public Vector3Int ObjectPosition;
	public float ObjectYRotation;

	public ObjectTransInfo(Vector3Int objectPosition, float objectYRotation)
	{
		ObjectPosition = objectPosition;
		ObjectYRotation = objectYRotation;
	}
}