using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
	[SerializeField] private InputManager inputManager;
	[SerializeField] private Grid grid;

	[SerializeField] private GameObject gridVisualization;

	private GridData floorData, furnitureData;

	[SerializeField] private ObjectPlacer objectPlacer;

	[SerializeField] private PreviewSystem preview;

	private Vector3Int lastDetectedPosition = Vector3Int.zero;
	private float yRotateValue = 0;

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
			ObjectTransInfo gridInfo = new(gridPosition, yRotateValue);

			buildingState.UpdateState(gridInfo);
			lastDetectedPosition = gridPosition;
		}
		RotateObject();
	}

	public void StartPlacement(int itemId) 
	{
		StopPlacement();

		gridVisualization.SetActive(true);

		buildingState = new PlacementState(itemId, grid, preview, floorData, furnitureData, objectPlacer);
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
		ObjectTransInfo gridInfo = new(gridPosition, yRotateValue);

		buildingState.OnAction(gridInfo);
	}

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

	private void RotateObject()
	{
		if (Input.GetKeyUp(KeyCode.Q))
		{
			yRotateValue = 90;
		}
		else if (Input.GetKeyUp(KeyCode.E))
		{
			yRotateValue = -90;
		}
	}

	public GridData GetFloorData() => floorData;
	public GridData GetFurnitureData() => furnitureData;
}
