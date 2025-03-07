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

		/* Temp Test Code */
		List<PlacedObjectDatas> testServerData = new List<PlacedObjectDatas>
	    {
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(-1, 0, 0), 0), 1),
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(-1, 0, -1), 0), 1),
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(-1, 0, -2), 0), 1),
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(0, 0, -1), 90), 1),
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(1, 0, -1), 180), 1),
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(2, 0, -1), 270), 1),
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(0, 0, -2), 270), 1), 
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(1, 0, -2), 180), 1),
		    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(2, 0, -2), 90), 1),
	    };
		StartLoadingPlacedObjectData(testServerData); 

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
            Vector3Int position = obj.Key;
            PlacementData data = obj.Value;

            PlacedObjectDatas objectData = new PlacedObjectDatas(
                data.ID, new ObjectTransInfo(position, data.RotationY), dataType);

			placedObjects.Add(objectData);
		}
    }

	/// <summary>
	/// Install objects based on data received from the server
	/// </summary>
	private IEnumerator LoadPlacedObjects(List<PlacedObjectDatas> serverData)
    {
        /* Wait for FloorData&FurnitureData Instance Created */
        yield return new WaitUntil(() => 
            placementSystem.GetFloorData() != null && 
            placementSystem.GetFurnitureData() != null);

		GridData floorData = placementSystem.GetFloorData();
        GridData furnitureData = placementSystem.GetFurnitureData();

        foreach(var obj in serverData)
        {
            GridData selectedData = obj.DataType == 0 ? floorData : furnitureData;
            int index = objectPlacer.PlaceObject(
                ItemDataLoader.HousingItemsList.Find(
                x => x.ItemId == obj.ItemId).ItemPrefab,
                grid.CellToWorld(obj.ItemTransInfo.ItemPosition) + new Vector3(0.5f, 0f, 0.5f),
                Quaternion.Euler(0, obj.ItemTransInfo.ItemYRotation, 0)
			);
            Vector2Int objGridSize = ItemDataLoader.HousingItemsList.Find(x => x.ItemId == obj.ItemId).ItemGridSize;

			selectedData.AddObjectAt(obj.ItemTransInfo.ItemPosition, objGridSize,
                obj.ItemId, index, obj.ItemTransInfo.ItemYRotation);
		}

        SavePlacedObjects();
	}

    public void StartLoadingPlacedObjectData(List<PlacedObjectDatas> serverData)
    {
        StartCoroutine(LoadPlacedObjects(serverData));
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
