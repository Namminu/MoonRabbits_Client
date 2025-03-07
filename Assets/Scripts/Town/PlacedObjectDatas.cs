using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacedObjectDatas
{
	public int ItemId;
	public Vector3Int ItemPosition;	
	public int DataType;

	public PlacedObjectDatas(int itemId, Vector3Int itemPosition, int dataType)
	{
		ItemId = itemId;
		ItemPosition = itemPosition;
		DataType = dataType;
	}
}
