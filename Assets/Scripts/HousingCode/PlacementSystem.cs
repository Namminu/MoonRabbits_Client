using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
	/* 마우스 감지 및 그리드 표시를 위한 멤버 */
	[SerializeField] private GameObject mouseIndicator;
	[SerializeField] private GameObject cellIndicator;
	[SerializeField] private InputManager inputManager;
	[SerializeField] private Grid grid;

	/* 아이템 데이터 관련 멤버 */
	private int selectedObjectItemId = -1;
	[SerializeField] private GameObject gridVisualization;

	[Tooltip("테스트용 멤버. 추후 HSItem Json 로드 후 변경 필요")]
	[SerializeField] private ObjectDataBaseSo db;

	private void Start()
	{
		StopPlacement();
	}

	private void Update()
	{
		if (selectedObjectItemId < 0) return;

		Vector3 mousePosition = inputManager.GetSelectedMapPosition();
		Vector3Int gridPosition = grid.WorldToCell(mousePosition);

		mouseIndicator.transform.position = mousePosition;
		cellIndicator.transform.position = grid.CellToWorld(gridPosition);
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
		cellIndicator.SetActive(true);
		inputManager.OnClicked += PlaceStructure;
		inputManager.OnExit += StopPlacement;
	}

	private void PlaceStructure()
	{
		if (inputManager.IsPointerOverUI()) return;

		Vector3 mousePosition = inputManager.GetSelectedMapPosition();
		Vector3Int gridPosition = grid.WorldToCell(mousePosition);

		//GameObject newObject = Instantiate(ItemDataLoader.HousingItemsList[selectedObjectItemId].ItemPrefab);
		GameObject newObject = Instantiate(db.objectDatas[selectedObjectItemId].Prefab);
		newObject.transform.position = grid.CellToWorld(gridPosition);
	}

	private void StopPlacement()
	{
		selectedObjectItemId = -1;
		gridVisualization.SetActive(false);
		cellIndicator.SetActive(false);
		inputManager.OnClicked -= PlaceStructure;
		inputManager.OnExit -= StopPlacement;
	}
}
