using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UIElements;

public class HouseManager : MonoBehaviour
{
    [Header("Scene Objects Ref")]
    [SerializeField]
    private PlacementSystem placementSystem;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    private Grid grid;

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
        else
            Destroy(gameObject);

        if (placementSystem == null)
            placementSystem = FindObjectOfType<PlacementSystem>();

        if (saveDelay == 0)
            saveDelay = 30f;

        StartCoroutine(AutoSaveRoutine());

        LoadHousingData();

        if (CanvasManager.Instance != null)
        {
            CanvasManager.Instance.DeactivateUI();
        }

        /* Temp Test Code */
        //List<PlacedObjectDatas> testServerData = new List<PlacedObjectDatas>
        //{
        //    new PlacedObjectDatas(30002, new ObjectTransInfo(new Vector3Int(-4, 0, 3), 270), 1),
        //    new PlacedObjectDatas(30002, new ObjectTransInfo(new Vector3Int(-2, 0, 3), 180), 1),
        //    new PlacedObjectDatas(30002, new ObjectTransInfo(new Vector3Int(-2, 0, 3), 90), 1),
        //    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(-3, 0, -2), 0), 1),
        //    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(-1, 0, -2), 270), 1),
        //    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(0, 0, -1), 180), 1),
        //    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(0, 0, -1), 90), 1),
        //    new PlacedObjectDatas(30001, new ObjectTransInfo(new Vector3Int(1, 0, -2), 0), 1),
        //    new PlacedObjectDatas(30003, new ObjectTransInfo(new Vector3Int(0, 0, 2), 0), 1),
        //    new PlacedObjectDatas(30003, new ObjectTransInfo(new Vector3Int(5, 0, -2), 270), 1),
        //    new PlacedObjectDatas(30003, new ObjectTransInfo(new Vector3Int(9, 0, 5), 180), 1),
        //};
    }

    public void StartLoadingPlacedObjectData(List<PlacedObjectDatas> serverData)
    {
        StartCoroutine(LoadPlacedObjects(serverData));
    }

    /// <summary>
    /// Install objects based on data received from the server
    /// </summary>
    private IEnumerator LoadPlacedObjects(List<PlacedObjectDatas> serverData)
    {
        /* Wait for FloorData&FurnitureData Instance Created */
        yield return new WaitUntil(
            () =>
                placementSystem.GetFloorData() != null && placementSystem.GetFurnitureData() != null
        );

        GridData floorData = placementSystem.GetFloorData();
        GridData furnitureData = placementSystem.GetFurnitureData();

        foreach (var obj in serverData)
        {
            GridData selectedData = obj.DataType == 0 ? floorData : furnitureData;
            int index = objectPlacer.PlaceObject(
                ItemDataLoader.HousingItemsList.Find(x => x.ItemId == obj.ItemId).ItemPrefab,
                grid.CellToWorld(
                    obj.ItemTrans.ObjectPosition
                ) /*+ new Vector3(0.5f, 0f, 0.5f)*/
                ,
                obj.ItemTrans.ObjectYRotation,
                craftItemId: obj.ItemId,
                true
            );
            Vector2Int objGridSize = ItemDataLoader
                .HousingItemsList.Find(x => x.ItemId == obj.ItemId)
                .ItemGridSize;

            selectedData.AddObjectAt(obj.ItemTrans, objGridSize, obj.ItemId, index);
        }

        SavePlacedObjects();
    }

    public void HandleHousingLoad(S2CHousingLoad pkt)
    {
        if (pkt == null)
        {
            Debug.LogError("[HousingLoad] ��Ŷ�� null�Դϴ�.");
            return;
        }
        if (pkt.Status != "success")
        {
            Debug.LogError($"[HousingLoad] ���� �ε� ����: {pkt.Msg}");
            return;
        }

        // ���� �����͸� PlacedObjectDatas ����Ʈ�� �����ϱ�
        List<PlacedObjectDatas> serverData = new List<PlacedObjectDatas>();
        foreach (var info in pkt.HousingInfo)
        {
            int itemId = info.ItemId;
            int dataType = info.DataType;

            // transform ������ ������ �⺻�� ���
            if (info.Transform == null)
            {
                var defaultTrans = new ObjectTransInfo(Vector3Int.zero, 0f);
                serverData.Add(new PlacedObjectDatas(itemId, defaultTrans, dataType));
            }
            else
            {
                // PlacedObjectDatas.cs�� ���ǵ� ObjectTransInfo�� Vector3Int�� float�� ���
                // �������� ���޹��� float ��ǥ ���� �ݿø��Ͽ� Vector3Int�� ��ȯ
                Vector3Int position = new(
                    Mathf.RoundToInt(info.Transform.PosX),
                    Mathf.RoundToInt(info.Transform.PosY),
                    Mathf.RoundToInt(info.Transform.PosZ)
                );
                // �ʵ���� �ٸ� �� ������ ���������� rotation �ʵ���� Rotation�̶�� ����
                float rotation = info.Transform.Rot;
                var transInfo = new ObjectTransInfo(position, rotation);
                serverData.Add(new PlacedObjectDatas(itemId, transInfo, dataType));
            }
        }

        // ������ serverData ����Ʈ�� �̿��Ͽ� grid�� ������ ��ġ
        StartLoadingPlacedObjectData(serverData);

        Debug.Log("���� ������ �ε� �� ��ġ �Ϸ�");
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
        foreach (var obj in gridData.placedObjectsList)
        {
            PlacementData data = obj.Value;

            PlacedObjectDatas objectData = new PlacedObjectDatas(
                data.ID,
                new ObjectTransInfo(data.anchorPoint, data.ObjectYRotation),
                dataType
            );

            placedObjects.Add(objectData);
        }
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveDelay);
            SavePlacedObjects();
            SendHousingSave();
            Debug.Log("Auto Save Complete. Save Objs Count : " + placedObjects.Count);
        }
    }

    private void OnApplicationQuit()
    {
        SavePlacedObjects();
        SendHousingSave();
        Debug.Log("Game Quit Save Complete. Save Objs Count : " + placedObjects.Count);
    }

    public void OnSaveButton()
    {
        SavePlacedObjects();
        SendHousingSave();
        Debug.Log("Button Click Save Complete. Save Objs Count : " + placedObjects.Count);
    }

    public void OnInventoryButton()
    {
        var inventory = CanvasManager.Instance.inventoryUI.gameObject;
        inventory.SetActive(!inventory.activeSelf);
    }

    public void OnRecallButton()
    {
        SavePlacedObjects();
        SendHousingSave();

        var pkt = new C2SMoveSector { TargetSector = 100 };
        GameManager.Network.Send(pkt);
    }

    #region ���� ��Ŷ ����

    /// <summary>
    /// �Ͽ�¡ ������ ������ �����ϴ� �޼ҵ�
    /// </summary>
    public void SendHousingSave()
    {
        // �Ͽ�¡ ������ ����(������Ʈ) �� placedObjects ����Ʈ�� �ֽ�ȭ
        int savedCount = SavePlacedObjects();

        C2SHousingSave packet = new C2SHousingSave();

        // placedObjects ����Ʈ�� �ִ� �� �����͸� HousingInfo �޽����� ��ȯ�Ͽ� ��Ŷ�� �߰�
        foreach (var placedObj in placedObjects)
        {
            HousingInfo info = new HousingInfo();
            info.ItemId = placedObj.ItemId; // ������ ID
            info.DataType = placedObj.DataType; // ������ Ÿ�� (0: Floor, 1: Furniture)
            info.Transform = ConvertToTransformInfo(placedObj.ItemTrans); // ��ġ �� ȸ���� ��ȯ

            packet.HousingInfo.Add(info);
        }

        // ��Ʈ��ũ �Ŵ����� ���� ��Ŷ ���� (InventoryManager.cs�� ���� ��İ� ����)
        GameManager.Network.Send(packet);
        Debug.Log("�Ͽ�¡ ���� ��Ŷ ���� �Ϸ�. ������ ����: " + savedCount);
    }

    /// <summary>
    /// ������ �Ͽ�¡ �ε� ��û�� �����մϴ�.
    /// </summary>
    public void LoadHousingData()
    {
        // �ε� ��û ���� ��Ŷ ���� (�����Ͱ� �������� �����Ƿ� ������ �Ķ���� ���� �� ��Ŷ)
        C2SHousingLoad loadPacket = new C2SHousingLoad();
        Debug.Log("�Ͽ�¡ �ε� ��û ����");
        GameManager.Network.Send(loadPacket);
    }

    /// <summary>
    /// ObjectTransInfo�� protobuf�� TransformInfo�� ��ȯ�ϴ� ���� �޼ҵ�
    /// </summary>
    /// <param name="objTrans">ObjectTransInfo Ÿ���� ��ü</param>
    /// <returns>�������ݿ� ���� TransformInfo ��ü</returns>
    private TransformInfo ConvertToTransformInfo(ObjectTransInfo objTrans)
    {
        TransformInfo tInfo = new TransformInfo();
        // ���÷� Vector3Int�� ���� x, y, z ������ �Ҵ��ϰ�, ȸ������ ObjectYRotation���� ó���մϴ�.
        tInfo.PosX = objTrans.ObjectPosition.x;
        tInfo.PosY = objTrans.ObjectPosition.y;
        tInfo.PosZ = objTrans.ObjectPosition.z;
        tInfo.Rot = objTrans.ObjectYRotation;
        return tInfo;
    }

    #endregion
}
