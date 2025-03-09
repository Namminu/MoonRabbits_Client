using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData
{
	public Dictionary<ObjectTransInfo, PlacementData> placedObjects = new();

	public void AddObjectAt(ObjectTransInfo gridInfo, Vector2Int objectSize, int ID, int placedObjectIndex)
	{
		Debug.Log("GridData : " + gridInfo.ItemPosition + gridInfo.ItemYRotation);

		List<ObjectTransInfo> positionToOccupy = CalculatePosition(gridInfo, objectSize);	 
		PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

		foreach (var pos in positionToOccupy)
		{
			if (placedObjects.ContainsKey(pos))
				throw new Exception($"Dictionary already contains this cell position {pos}");

			placedObjects[pos] = data;
		}
	}

	private List<ObjectTransInfo> CalculatePosition(ObjectTransInfo gridInfo, Vector2Int objectSize)
	{
		List<ObjectTransInfo> returnVal = new();

		/* Switch between horizontal and vertical depending on the rotation of the object */
		bool isRotated = Mathf.Approximately(gridInfo.ItemYRotation, 90) ||
						Mathf.Approximately(gridInfo.ItemYRotation, 270);
		int width = isRotated ? objectSize.y : objectSize.x;
		int height = isRotated ? objectSize.x : objectSize.y;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				ObjectTransInfo newGridInfo = new(gridInfo.ItemPosition + new Vector3Int(x, 0, y), gridInfo.ItemYRotation);
				returnVal.Add(newGridInfo);
			}
		}
		return returnVal;
	}

	public bool CanPlaceObjectAt(ObjectTransInfo gridInfo, Vector2Int objectSize)
	{
		List<ObjectTransInfo> positionToOccupy = CalculatePosition(gridInfo, objectSize);

		/* Extract and save all occupied positions */
		HashSet<ObjectTransInfo> occupiedPositions = new();
		foreach (var pos in placedObjects.Keys)
		{
			occupiedPositions.Add(pos);
		}

		/* Compare with the new object's occupied positions */
		foreach (var pos in positionToOccupy)
		{
			if (occupiedPositions.Contains(pos)) return false;
		}
		return true;
	}

	internal int GetRepresentationIndex(ObjectTransInfo gridPosition)
	{
		if (!placedObjects.ContainsKey(gridPosition)) return -1;

		return placedObjects[gridPosition].PlacedObjectIndex;
	}

	internal void RemoveObjectAt(ObjectTransInfo gridPosition)
	{
		if (placedObjects.ContainsKey(gridPosition))
		{
			placedObjects.Remove(gridPosition);
		}
	}
}

public class PlacementData
{
	public List<ObjectTransInfo> occupiedPositions;
	public int ID { get; private set; }
	public int PlacedObjectIndex { get; private set; }

	public PlacementData(List<ObjectTransInfo> occupiedPositions, int iD, int placedObjectIndex)
	{
		this.occupiedPositions = occupiedPositions;
		ID = iD;
		PlacedObjectIndex = placedObjectIndex;
	}
}
