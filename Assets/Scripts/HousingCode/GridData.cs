using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData 
{
	/* Save installed objects */
	public Dictionary<int, PlacementData> placedObjectsList = new();
	/* Save all coordinates of the object */
	private Dictionary<Vector3Int, PlacementData> placedObjectsPosition = new();

	public void AddObjectAt(ObjectTransInfo gridInfo, Vector2Int objectSize, int ID, int placedObjectIndex)
	{
		List<Vector3Int> positionToOccupy = CalculatePosition(gridInfo.ObjectPosition, objectSize, gridInfo.ObjectYRotation);
		PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex, gridInfo.ObjectYRotation, gridInfo.ObjectPosition);
		foreach (var pos in positionToOccupy)
		{
			if (placedObjectsPosition.ContainsKey(pos))
				throw new Exception($"Dictionary already contatins this cell position {pos}");

			placedObjectsPosition[pos] = data;
		}
		placedObjectsList[placedObjectIndex] = data;

		Debug.Log("GridData : " + placedObjectsPosition.Count); 
	}

	private List<Vector3Int> CalculatePosition(Vector3Int gridPosition, Vector2Int objectSize, float yRotation)
	{
		// unrotated 상태의 네 꼭지점 (여기서 objectSize는 '셀 단위' 크기임)
		// 오른쪽, 위쪽 경계를 위해 objectSize 값을 그대로 더함 (예: (1,1)에서 4×3이면 오른쪽 경계는 1+4 = 5)
		Vector3Int A = gridPosition;
		Vector3Int B = gridPosition + new Vector3Int(objectSize.x, 0, 0);
		Vector3Int C = gridPosition + new Vector3Int(0, 0, objectSize.y);
		Vector3Int D = gridPosition + new Vector3Int(objectSize.x, 0, objectSize.y);

		// 효과적 회전: 사용자가 yRotation에 270를 넣으면 실제 회전은 +90° counterclockwise가 되어야 함.
		// 따라서 effectiveRotation = (360 - yRotation) mod 360.
		int effectiveRot = ((360 - (int)yRotation) % 360);

		// 회전 함수: 주어진 점을 gridPosition(피벗) 기준으로 effectiveRot만큼 회전합니다.
		Vector3Int RotatePoint(Vector3Int point)
		{
			int ox = point.x - gridPosition.x;
			int oz = point.z - gridPosition.z;
			int rx = 0, rz = 0;
			switch (effectiveRot)
			{
				case 0:
					rx = ox;
					rz = oz;
					break;
				case 90:
					// 90° counterclockwise: (x, z) -> (-z, x)
					rx = -oz;
					rz = ox;
					break;
				case 180:
					rx = -ox;
					rz = -oz;
					break;
				case 270:
					// 270° counterclockwise: (x, z) -> (z, -x)
					rx = oz;
					rz = -ox;
					break;
				default:
					rx = ox;
					rz = oz;
					break;
			}
			return new Vector3Int(gridPosition.x + rx, gridPosition.y, gridPosition.z + rz);
		}

		Vector3Int Arot = RotatePoint(A);
		Vector3Int Brot = RotatePoint(B);
		Vector3Int Crot = RotatePoint(C);
		Vector3Int Drot = RotatePoint(D);

		// 구한 네 꼭지점의 축에 평행한 바운딩박스를 구합니다.
		int minX = Mathf.Min(Arot.x, Brot.x, Crot.x, Drot.x);
		int maxX = Mathf.Max(Arot.x, Brot.x, Crot.x, Drot.x);
		int minZ = Mathf.Min(Arot.z, Brot.z, Crot.z, Drot.z);
		int maxZ = Mathf.Max(Arot.z, Brot.z, Crot.z, Drot.z);

		// 바운딩박스의 크기는 (maxX - minX) x (maxZ - minZ)
		// unrotated일 때 (예: (1,1)에서 (5,4))라면 width = 4, height = 3, 총 12 셀.
		// for문에서 x는 minX부터 maxX-1, z는 minZ부터 maxZ-1 까지 반복합니다.
		List<Vector3Int> cells = new List<Vector3Int>();
		for (int x = minX; x < maxX; x++)
		{
			for (int z = minZ; z < maxZ; z++)
			{
				cells.Add(new Vector3Int(x, gridPosition.y, z));
			}
		}
		return cells;
	}

	public bool CanPlaceObjectAt(ObjectTransInfo gridInfo, Vector2Int objectSize)
	{
		List<Vector3Int> positionToOccupy = CalculatePosition(gridInfo.ObjectPosition, objectSize, gridInfo.ObjectYRotation);
		foreach(var pos in positionToOccupy)
		{
			if (placedObjectsPosition.ContainsKey(pos)) return false;
		}
		return true;
	}

	internal int GetRepresentationIndex(Vector3Int gridPosition)
	{
		if (!placedObjectsPosition.ContainsKey(gridPosition)) return -1;

		return placedObjectsPosition[gridPosition].PlacedObjectIndex;
	}

	internal void RemoveObjectAt(Vector3Int gridPosition)
	{
		foreach(var pos in placedObjectsPosition[gridPosition].occupiedPosition)
		{
			placedObjectsPosition.Remove(pos);
		}
	}
}

public class PlacementData
{
	public List<Vector3Int> occupiedPosition;
	public int ID { get; private set; }
	public int PlacedObjectIndex { get; private set; }
	public float ObjectYRotation {  get; private set; }
	public Vector3Int anchorPoint {  get; private set; }

	public PlacementData(List<Vector3Int> occupiedPosition, int iD, int placedObjectIndex, 
		float objectYRotation, Vector3Int anchorPoint)
	{
		this.occupiedPosition = occupiedPosition;
		ID = iD;
		PlacedObjectIndex = placedObjectIndex;
		ObjectYRotation = objectYRotation;
		this.anchorPoint = anchorPoint;
	}
}