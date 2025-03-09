using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HouseManager : MonoBehaviour
{
    [Header("Scene Objects Ref")]
    [SerializeField] private PlacementSystem placementSystem;
	[SerializeField] private ObjectPlacer objectPlacer;
	[SerializeField] private Grid grid;

    [Header("Members")]
    [SerializeField, Tooltip("Placed Objects Auto Save Delay")]
    private float saveDelay;

	private static HouseManager _instance;
    public static HouseManager Instance => _instance;

    public List<PlacedObjectDatas> placedObjects = new();


    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
			_instance = this;
        }
        else Destroy(gameObject);

        if (placementSystem == null)
            placementSystem = FindObjectOfType<PlacementSystem>();

        if (saveDelay == 0) saveDelay = 30f;

        StartCoroutine(AutoSaveRoutine());
	}

	/// <summary>
	/// Save the coordinates and IDs of installed objects in the list
	/// </summary>
	/// <returns>Value To Save Success Data Count</returns>
	public int SavePlacedObjects()
    {
        placedObjects.Clear();

        /* Seperate Save FloorData / Furniture Data  */
        SaveGridData(placementSystem.GetFloorData(), 0);
		SaveGridData(placementSystem.GetFurnitureData(), 1);

		return placedObjects.Count;
    }

    private void SaveGridData(GridData gridData, int dataType)
    {
        foreach(var obj in gridData.placedObjects)
        {
			ObjectTransInfo position = obj.Key;
            PlacementData data = obj.Value;

            PlacedObjectDatas objectData = new(
                data.ID, position, dataType);

			placedObjects.Add(objectData);
		}
    }

	/// <summary>
	/// Install objects based on data received from the server
	/// </summary>
	public void LoadPlacedObjects(List<PlacedObjectDatas> serverData)
    {
		GridData floorData = placementSystem.GetFloorData();
        GridData furnitureData = placementSystem.GetFurnitureData();

        foreach(var obj in serverData)
        {
            GridData selectedData = obj.DataType == 0 ? floorData : furnitureData;
			int index = objectPlacer.PlaceObject(
				ItemDataLoader.HousingItemsList.Find(x => x.ItemId == obj.ItemId).ItemPrefab,
				Helper.Vector3ToInt(grid.CellToWorld(obj.ItemTransInfo.ItemPosition)),
                obj.ItemTransInfo.ItemYRotation
			);
            Vector2Int objGridSize = ItemDataLoader.HousingItemsList.Find(x => x.ItemId == obj.ItemId).ItemGridSize;

			selectedData.AddObjectAt(obj.ItemTransInfo, objGridSize, obj.ItemId, index);
		}
	}


    private IEnumerator AutoSaveRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(saveDelay);
            SavePlacedObjects();
            Debug.Log("Auto Save Complete. Save Objs Count : " + placedObjects.Count);
        }
    }

	private void OnApplicationQuit()
	{
		SavePlacedObjects();
		Debug.Log("Game Quit Save Complete. Save Objs Count : " + placedObjects.Count);
	}

    public void OnSaveButton()
    {
		SavePlacedObjects();
		Debug.Log("Button Click Save Complete. Save Objs Count : " + placedObjects.Count);
	}
}
