using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
	[SerializeField] private InputManager inputManager;
	[SerializeField] private Grid grid;

	[SerializeField] private GameObject gridVisualization;

	[Tooltip("테스트용 멤버. 추후 HSItem Json 로드 후 변경 필요")]
	[SerializeField] private ObjectDataBaseSo db;

	private GridData floorData, furnitureData;

	[SerializeField] private ObjectPlacer objectPlacer;

	[SerializeField] private PreviewSystem preview;

	private Vector3Int lastDetectedPosition = Vector3Int.zero;

	IBuildingState buildingState;

	private void Start()
	{
		StopPlacement();
		floorData = new GridData();
		furnitureData = new GridData();
	}

	private void Update()
	{
		if (buildingState == null) return;

		Vector3 mousePosition = inputManager.GetSelectedMapPosition();
		Vector3Int gridPosition = grid.WorldToCell(mousePosition);

		if(lastDetectedPosition != gridPosition)
		{
			buildingState.UpdateState(gridPosition);
			lastDetectedPosition = gridPosition;
		}
	}

	public void StartPlacement(int itemId) 
	{
		StopPlacement();
		gridVisualization.SetActive(true);

		buildingState = new PlacementState(itemId, grid, preview, db, floorData, furnitureData, objectPlacer);
		inputManager.OnClicked += PlaceStructure;
		inputManager.OnExit += StopPlacement;
	}

	public void StartRemoving()
	{
		StopPlacement();
		gridVisualization.SetActive(true);

		buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
		inputManager.OnClicked += PlaceStructure;
		inputManager.OnExit += StopPlacement;
	}

	private void PlaceStructure()
	{
		if (inputManager.IsPointerOverUI()) return;

		Vector3 mousePosition = inputManager.GetSelectedMapPosition();
		Vector3Int gridPosition = grid.WorldToCell(mousePosition);

		buildingState.OnAction(gridPosition);
	}

	//private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectItemId)
	//{
	//	GridData selectedData = db.objectDatas[selectedObjectItemId].ID == 0 ? 
	//		floorData : furnitureData;

	//	return selectedData.CanPlaceObjectAt(gridPosition, db.objectDatas[selectedObjectItemId].Size);
	//}

	private void StopPlacement()
	{
		if (buildingState == null) return;

		gridVisualization.SetActive(false);
		buildingState.EndState();
		inputManager.OnClicked -= PlaceStructure;
		inputManager.OnExit -= StopPlacement;
		lastDetectedPosition = Vector3Int.zero;
		buildingState = null;
	}
}
