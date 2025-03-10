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
		// unrotated ������ �� ������ (���⼭ objectSize�� '�� ����' ũ����)
		// ������, ���� ��踦 ���� objectSize ���� �״�� ���� (��: (1,1)���� 4��3�̸� ������ ���� 1+4 = 5)
		Vector3Int A = gridPosition;
		Vector3Int B = gridPosition + new Vector3Int(objectSize.x, 0, 0);
		Vector3Int C = gridPosition + new Vector3Int(0, 0, objectSize.y);
		Vector3Int D = gridPosition + new Vector3Int(objectSize.x, 0, objectSize.y);

		// ȿ���� ȸ��: ����ڰ� yRotation�� 270�� ������ ���� ȸ���� +90�� counterclockwise�� �Ǿ�� ��.
		// ���� effectiveRotation = (360 - yRotation) mod 360.
		int effectiveRot = ((360 - (int)yRotation) % 360);

		// ȸ�� �Լ�: �־��� ���� gridPosition(�ǹ�) �������� effectiveRot��ŭ ȸ���մϴ�.
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
					// 90�� counterclockwise: (x, z) -> (-z, x)
					rx = -oz;
					rz = ox;
					break;
				case 180:
					rx = -ox;
					rz = -oz;
					break;
				case 270:
					// 270�� counterclockwise: (x, z) -> (z, -x)
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

		// ���� �� �������� �࿡ ������ �ٿ���ڽ��� ���մϴ�.
		int minX = Mathf.Min(Arot.x, Brot.x, Crot.x, Drot.x);
		int maxX = Mathf.Max(Arot.x, Brot.x, Crot.x, Drot.x);
		int minZ = Mathf.Min(Arot.z, Brot.z, Crot.z, Drot.z);
		int maxZ = Mathf.Max(Arot.z, Brot.z, Crot.z, Drot.z);

		// �ٿ���ڽ��� ũ��� (maxX - minX) x (maxZ - minZ)
		// unrotated�� �� (��: (1,1)���� (5,4))��� width = 4, height = 3, �� 12 ��.
		// for������ x�� minX���� maxX-1, z�� minZ���� maxZ-1 ���� �ݺ��մϴ�.
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