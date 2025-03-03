using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlacementState : IBuildingState
{
	private int selectedObjectItemId = -1;
	int ID;
	Grid grid;
	PreviewSystem previewSystem;
	ObjectDataBaseSo db;
	GridData floorData;
	GridData furnitureData;
	ObjectPlacer objectPlacer;

	public PlacementState(int iD,
					   Grid grid,
					   PreviewSystem previewSystem,
					   ObjectDataBaseSo db,
					   GridData floorData,
					   GridData furnitureData,
					   ObjectPlacer objectPlacer)
	{
		ID = iD;
		this.grid = grid;
		this.previewSystem = previewSystem;
		this.db = db;
		this.floorData = floorData;
		this.furnitureData = furnitureData;
		this.objectPlacer = objectPlacer;

		/* selectedObjectItemId = ItemDataLoader.HousingItemsList.FindIndex(data => data.ItemId == itemId); */
		selectedObjectItemId = db.objectDatas.FindIndex(data => data.ID == ID);
		if (selectedObjectItemId > -1)
		{
			previewSystem.StartShowingPlacementPreview(
				db.objectDatas[selectedObjectItemId].Prefab,
				db.objectDatas[selectedObjectItemId].Size);
		}
		else throw new System.Exception($"No Object with ID {iD}");
	}

	public void EndState()
	{
		previewSystem.StopShowingPreview();
	}

	public void OnAction(Vector3Int gridPosition)
	{
		bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectItemId);
		if (placementValidity == false) return;

		int index = objectPlacer.PlaceObject(db.objectDatas[selectedObjectItemId].Prefab, grid.CellToWorld(gridPosition));

		GridData selectedData = db.objectDatas[selectedObjectItemId].ID == 0 ?
			floorData : furnitureData;
		selectedData.AddObjectAt(gridPosition,
			db.objectDatas[selectedObjectItemId].Size,
			db.objectDatas[selectedObjectItemId].ID,
			index);

		previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
	}

	private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectItemId)
	{
		GridData selectedData = db.objectDatas[selectedObjectItemId].ID == 0 ?
			floorData : furnitureData;

		return selectedData.CanPlaceObjectAt(gridPosition, db.objectDatas[selectedObjectItemId].Size);
	}

	public void UpdateState(Vector3Int gridPosition)
	{
		bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectItemId);

		previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
	}
}
