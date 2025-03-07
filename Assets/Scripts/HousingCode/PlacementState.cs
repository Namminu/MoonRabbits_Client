using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlacementState : IBuildingState
{
	private int selectedObjectIndex = -1;
	int ID;
	Grid grid;
	PreviewSystem previewSystem;
	GridData floorData;
	GridData furnitureData;
	ObjectPlacer objectPlacer;

	public PlacementState(int iD,
					   Grid grid,
					   PreviewSystem previewSystem,
					   GridData floorData,
					   GridData furnitureData,
					   ObjectPlacer objectPlacer)
	{
		ID = iD;
		this.grid = grid;
		this.previewSystem = previewSystem;
		this.floorData = floorData;
		this.furnitureData = furnitureData;
		this.objectPlacer = objectPlacer;

		selectedObjectIndex = ItemDataLoader.HousingItemsList.FindIndex(data => data.ItemId == ID);
		if (selectedObjectIndex > -1)
		{
			previewSystem.StartShowingPlacementPreview(
				ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemPrefab,
				ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemGridSize);
		}
		else throw new System.Exception($"No Object with ID {iD}");
	}

	public void EndState()
	{
		previewSystem.StopShowingPreview();
	}

	public void OnAction(Vector3Int gridPosition)
	{
		bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
		if (!placementValidity) return;

		float rotationY = previewSystem.GetCurrentRotation().eulerAngles.y;

		//  오브젝트 크기 가져오기 (gridSize 사용)
		Vector2Int gridSize = ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemGridSize;

		//  회전 보정을 위해 중심을 기준으로 위치 조정
		Vector3 pivotOffset = new Vector3(gridSize.x * 0.5f, 0, gridSize.y * 0.5f);
		Vector3 adjustedPosition = grid.CellToWorld(gridPosition) + pivotOffset;

		int index = objectPlacer.PlaceObject(
			ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemPrefab,
			adjustedPosition, //  위치 보정 적용
			Quaternion.Euler(0, rotationY, 0) //  회전값 적용
		);

		GridData selectedData = ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId == 0 ?
			floorData : furnitureData;

		selectedData.AddObjectAt(gridPosition,
			ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemGridSize,
			ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId,
			index,
			rotationY //  회전값 저장
		);

		previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
	}


	private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectItemId)
	{
		GridData selectedData = ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId == 0 ?
			floorData : furnitureData;

		return selectedData.CanPlaceObjectAt(gridPosition, ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemGridSize);
	}

	public void RotatePreview(int angle)
	{
		previewSystem.RotatePreview(angle);
	}

	public void UpdateState(Vector3Int gridPosition)
	{
		bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

		previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
	}
}
