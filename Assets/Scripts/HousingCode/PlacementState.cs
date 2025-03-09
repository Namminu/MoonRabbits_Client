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

	public void OnAction(ObjectTransInfo gridInfo)
	{
		bool placementValidity = CheckPlacementValidity(gridInfo, selectedObjectIndex);
		if (placementValidity == false) return;

		int index = objectPlacer.PlaceObject(ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemPrefab, 
			grid.CellToWorld(gridInfo.ObjectPosition), gridInfo.ObjectYRotation);

		GridData selectedData = ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId == 0 ?
			floorData : furnitureData;

		selectedData.AddObjectAt(gridInfo,
			ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemGridSize,
			ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId,
			index);

		ObjectTransInfo objectInfo = Helper.ChangeDataToTransInfo(
			Helper.VectorDataToInt(grid.CellToWorld(gridInfo.ObjectPosition)), gridInfo.ObjectYRotation);
		previewSystem.UpdatePosition(objectInfo, false);
	}

	private bool CheckPlacementValidity(ObjectTransInfo gridInfo, int selectedObjectItemId)
	{
		GridData selectedData = ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId == 0 ?
			floorData : furnitureData;

		return selectedData.CanPlaceObjectAt(gridInfo, ItemDataLoader.HousingItemsList[selectedObjectItemId].ItemGridSize);
	}

	public void UpdateState(ObjectTransInfo gridInfo)
	{
		bool placementValidity = CheckPlacementValidity(gridInfo, selectedObjectIndex);
		ObjectTransInfo objectInfo = Helper.ChangeDataToTransInfo(
			Helper.VectorDataToInt(grid.CellToWorld(gridInfo.ObjectPosition)), gridInfo.ObjectYRotation);

		previewSystem.UpdatePosition(objectInfo, placementValidity);
	}
}