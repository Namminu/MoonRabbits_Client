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

	IBuildingState buildingState;

	private float yRotation = 0;
	private float lastYRotation = 0;

	Vector3 mousePosition;
	Vector3Int gridPosition;
	ObjectTransInfo gridInfo;
	Vector2Int objectSize;

	private void Start()
	{
		StopPlacement();
		floorData = new GridData();
		furnitureData = new GridData();
	}

	private void Update()
	{
		if (buildingState == null) return;

		CalcGridInfo();

		if (lastDetectedPosition != gridPosition || yRotation != lastYRotation)
		{
			buildingState.UpdateState(gridInfo);
			lastDetectedPosition = gridPosition;
			lastYRotation = yRotation;
		}
	}

	public void StartPlacement(int itemId)
	{
		StopPlacement();
		gridVisualization.SetActive(true);

		objectSize = ItemDataLoader.HousingItemsList.Find(
			x => x.ItemId == itemId).ItemGridSize;
		buildingState = new PlacementState(itemId, grid, preview, floorData, furnitureData, objectPlacer);

		inputManager.OnClicked += PlaceStructure;
		inputManager.OnQEnter += EnterQ;
		inputManager.OnEEnter += EnterE;
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

		buildingState.OnAction(gridInfo);
	}

	private void StopPlacement()
	{
		if (buildingState == null) return;

		gridVisualization.SetActive(false);
		buildingState.EndState();
		inputManager.OnClicked -= PlaceStructure;
		inputManager.OnQEnter -= EnterQ;
		inputManager.OnEEnter -= EnterE;
		inputManager.OnExit -= StopPlacement;
		lastDetectedPosition = Vector3Int.zero;
		buildingState = null;
		yRotation = 0;
		lastYRotation = 0;
	}

	private void EnterQ() => RotateObject(0);
	private void EnterE() => RotateObject(1);

	private void RotateObject(int enterKey)
	{
		switch (enterKey)
		{
			case 0:
				yRotation -= 90;
				break;

			case 1:
				yRotation += 90;
				break;
			default: break;
		}
		yRotation = (yRotation % 360 + 360) % 360;
	}

	private void CalcGridInfo()
	{
		mousePosition = inputManager.GetSelectedMapPosition();
		gridPosition = grid.WorldToCell(mousePosition);
		gridInfo = Helper.ChangeDataToTransInfo(gridPosition, yRotation);
		//gridInfo = Helper.ChangeDataToTransInfo(GetRotatedGridPosition(), yRotation);
	}

	public GridData GetFloorData() => floorData;
	public GridData GetFurnitureData() => furnitureData;
}
