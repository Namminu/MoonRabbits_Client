using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
	/* 마우스 감지 및 그리드 표시를 위한 멤버 */
	[SerializeField] private GameObject mouseIndicator;
	//[SerializeField] private GameObject cellIndicator;
	[SerializeField] private InputManager inputManager;
	[SerializeField] private Grid grid;

	/* 아이템 데이터 관련 멤버 */
	private int selectedObjectItemId = -1;
	[SerializeField] private GameObject gridVisualization;

	[Tooltip("테스트용 멤버. 추후 HSItem Json 로드 후 변경 필요")]
	[SerializeField] private ObjectDataBaseSo db;

	private GridData floorData, furnitureData;
	//private Renderer previewRenderer;

	private List<GameObject> placedGameObject = new();

	[SerializeField] private PreviewSystem preview;

	private Vector3Int lastDetectedPosition = Vector3Int.zero;
		
	private void Start()
	{
		StopPlacement();
		floorData = new GridData();
		furnitureData = new GridData();
		//previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
	}

	private void Update()
	{
		if (selectedObjectItemId < 0) return;

		Vector3 mousePosition = inputManager.GetSelectedMapPosition();
		Vector3Int gridPosition = grid.WorldToCell(mousePosition);

		if(lastDetectedPosition != gridPosition)
		{
			bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectItemId);
			//previewRenderer.material.color = placementValidity ? Color.white : Color.red;

			mouseIndicator.transform.position = mousePosition;
			//cellIndicator.transform.position = grid.CellToWorld(gridPosition);
			preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
			lastDetectedPosition = gridPosition;
		}
	}

	public void StartPlacement(int itemId) 
	{
		StopPlacement();

		//selectedObjectItemId = ItemDataLoader.HousingItemsList.FindIndex(data => data.ItemId == itemId);
		selectedObjectItemId = db.objectDatas.FindIndex(data => data.ID == itemId);

		if (selectedObjectItemId < 0)
		{
			Debug.LogError($"No Id found in Housing Item List {itemId}");
			return;
		}
		gridVisualization.SetActive(true);
		//cellIndicator.SetActive(true);
		preview.StartShowingPlacementPreview(
			db.objectDatas[selectedObjectItemId].Prefab,
			db.objectDatas[selectedObjectItemId].Size);
		inputManager.OnClicked += PlaceStructure;
		inputManager.OnExit += StopPlacement;
	}

	private void PlaceStructure()
	{
		if (inputManager.IsPointerOverUI()) return;

		Vector3 mousePosition = inputManager.GetSelectedMapPosition();
		Vector3Int gridPosition = grid.WorldToCell(mousePosition);

		bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectItemId);
		if (placementValidity == false) return;

		//GameObject newObject = Instantiate(ItemDataLoader.HousingItemsList[selectedObjectItemId].ItemPrefab);
		GameObject newObject = Instantiate(db.objectDatas[selectedObjectItemId].Prefab);
		newObject.transform.position = grid.CellToWorld(gridPosition);
		placedGameObject.Add(newObject);
		 
		GridData selectedData = db.objectDatas[selectedObjectItemId].ID == 0 ?
			floorData : furnitureData;
		selectedData.AddObjectAt(gridPosition,
			db.objectDatas[selectedObjectItemId].Size,
			db.objectDatas[selectedObjectItemId].ID,
			placedGameObject.Count-1);

		preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
	}

	private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectItemId)
	{
		GridData selectedData = db.objectDatas[selectedObjectItemId].ID == 0 ? 
			floorData : furnitureData;

		return selectedData.CanPlaceObjectAt(gridPosition, db.objectDatas[selectedObjectItemId].Size);
	}

	private void StopPlacement()
	{
		selectedObjectItemId = -1;
		gridVisualization.SetActive(false);
		//cellIndicator.SetActive(false);
		preview.StopShowingPreview();
		inputManager.OnClicked -= PlaceStructure;
		inputManager.OnExit -= StopPlacement;

		lastDetectedPosition = Vector3Int.zero;
	}
}
