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
		Debug.Log(placementValidity);
		if (placementValidity == false) return;

		int index = objectPlacer.PlaceObject(ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemPrefab, 
			grid.CellToWorld(gridPosition));

		GridData selectedData = ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId == 0 ?
			floorData : furnitureData;

		Debug.Log($"[DEBUG] OnAction - ���õ� GridData ��ü �ؽ��ڵ�: {selectedData.GetHashCode()}");
		Debug.Log($"[DEBUG] OnAction - furnitureData ��ü �ؽ��ڵ�: {furnitureData.GetHashCode()}");
		Debug.Log($"[DEBUG] HousingItemsList[{selectedObjectIndex}].GridData ��ü �ؽ��ڵ�: {ItemDataLoader.HousingItemsList[selectedObjectIndex].GetHashCode()}");

		selectedData.AddObjectAt(gridPosition,
			ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemGridSize,
			ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId,
			index);

		previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
	}

	private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectItemId)
	{
		GridData selectedData = ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemId == 0 ?
			floorData : furnitureData;

		Debug.Log($"[DEBUG] furnitureData placedObjects ����: {furnitureData.placedObjects.Count}");

		return selectedData.CanPlaceObjectAt(gridPosition, ItemDataLoader.HousingItemsList[selectedObjectIndex].ItemGridSize);
	}

	public void UpdateState(Vector3Int gridPosition)
	{
		bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

		previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
	}
}
