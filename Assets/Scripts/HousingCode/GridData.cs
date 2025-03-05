using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData 
{
	public Dictionary<Vector3Int, PlacementData> placedObjects = new();

	public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
	{
		List<Vector3Int> positionToOccupy = CalculatePosition(gridPosition, objectSize);
		PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
		foreach (var pos in positionToOccupy)
		{
			if (placedObjects.ContainsKey(pos))
				throw new Exception($"Dictionary already contatins this cell position {pos}");

			placedObjects[pos] = data;
		}
	}

	private List<Vector3Int> CalculatePosition(Vector3Int gridPosition, Vector2Int objectSize)
	{
		List<Vector3Int> returnVal = new();
		for(int x = 0; x < objectSize.x; x++)
		{
			for(int y = 0; y < objectSize.y; y++)
			{
				returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
			}
		}
		return returnVal;
	}

	public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
	{
		List<Vector3Int> positionToOccupy = CalculatePosition(gridPosition, objectSize);
		foreach(var pos in positionToOccupy)
		{
			if (placedObjects.ContainsKey(pos)) return false;
		}
		return true;
	}

	internal int GetRepresentationIndex(Vector3Int gridPosition)
	{
		if (!placedObjects.ContainsKey(gridPosition)) return -1;

		return placedObjects[gridPosition].PlacedObjectIndex;
	}

	internal void RemoveObjectAt(Vector3Int gridPosition)
	{
		foreach(var pos in placedObjects[gridPosition].occupiedPosition)
		{
			placedObjects.Remove(pos);
		}
	}
}

public class PlacementData
{
	public List<Vector3Int> occupiedPosition;
	public int ID { get; private set; }
	public int PlacedObjectIndex { get; private set; }

	public PlacementData(List<Vector3Int> occupiedPosition, int iD, int placedObjectIndex)
	{
		this.occupiedPosition = occupiedPosition;
		ID = iD;
		PlacedObjectIndex = placedObjectIndex;
	}
}