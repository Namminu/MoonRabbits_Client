using Google.Protobuf.Protocol;
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

        LoadHousingData();

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
        yield return new WaitUntil(() =>
            placementSystem.GetFloorData() != null &&
            placementSystem.GetFurnitureData() != null);

        GridData floorData = placementSystem.GetFloorData();
        GridData furnitureData = placementSystem.GetFurnitureData();

        foreach (var obj in serverData)
        {
            GridData selectedData = obj.DataType == 0 ? floorData : furnitureData;
            int index = objectPlacer.PlaceObject(
                ItemDataLoader.HousingItemsList.Find(
                x => x.ItemId == obj.ItemId).ItemPrefab,
                grid.CellToWorld(obj.ItemTrans.ObjectPosition) /*+ new Vector3(0.5f, 0f, 0.5f)*/,
                obj.ItemTrans.ObjectYRotation
            );
            Vector2Int objGridSize = ItemDataLoader.HousingItemsList.Find(x => x.ItemId == obj.ItemId).ItemGridSize;

            selectedData.AddObjectAt(obj.ItemTrans, objGridSize,
                obj.ItemId, index);
        }

        SavePlacedObjects();
    }

    public void HandleHousingLoad(S2CHousingLoad pkt)
    {
        if (pkt == null)
        {
            Debug.LogError("[HousingLoad] 패킷이 null입니다.");
            return;
        }
        if (pkt.Status != "success")
        {
            Debug.LogError($"[HousingLoad] 가구 로드 실패: {pkt.Msg}");
            return;
        }

        // 서버 데이터를 PlacedObjectDatas 리스트로 가공하기
        List<PlacedObjectDatas> serverData = new List<PlacedObjectDatas>();
        foreach (var info in pkt.HousingInfo)
        {
            int itemId = info.ItemId;
            int dataType = info.DataType;

            // transform 정보가 없으면 기본값 사용
            if (info.Transform == null)
            {
                var defaultTrans = new ObjectTransInfo(Vector3Int.zero, 0f);
                serverData.Add(new PlacedObjectDatas(itemId, defaultTrans, dataType));
            }
            else
            {
                // PlacedObjectDatas.cs에 정의된 ObjectTransInfo는 Vector3Int와 float를 사용
                // 서버에서 전달받은 float 좌표 값을 반올림하여 Vector3Int로 변환
                Vector3Int position = new(
                    Mathf.RoundToInt(info.Transform.PosX),
                    Mathf.RoundToInt(info.Transform.PosY),
                    Mathf.RoundToInt(info.Transform.PosZ)
                );
                // 필드명이 다를 수 있으니 프로토콜의 rotation 필드명이 Rotation이라고 가정
                float rotation = info.Transform.Rot;
                var transInfo = new ObjectTransInfo(position, rotation);
                serverData.Add(new PlacedObjectDatas(itemId, transInfo, dataType));
            }
        }

        // 가공된 serverData 리스트를 이용하여 grid에 가구를 배치
        StartLoadingPlacedObjectData(serverData);

        Debug.Log("가구 데이터 로드 및 배치 완료");
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
                data.ID, new ObjectTransInfo(data.anchorPoint, data.ObjectYRotation), dataType);

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

    #region 서버 패킷 전송

    /// <summary>
    /// 하우징 정보를 서버에 전송하는 메소드
    /// </summary>
    public void SendHousingSave()
    {
        // 하우징 데이터 저장(업데이트) 후 placedObjects 리스트를 최신화
        int savedCount = SavePlacedObjects();

        C2SHousingSave packet = new C2SHousingSave();

        // placedObjects 리스트에 있는 각 데이터를 HousingInfo 메시지로 변환하여 패킷에 추가
        foreach (var placedObj in placedObjects)
        {
            HousingInfo info = new HousingInfo();
            info.ItemId = placedObj.ItemId;            // 아이템 ID
            info.DataType = placedObj.DataType;          // 데이터 타입 (0: Floor, 1: Furniture)
            info.Transform = ConvertToTransformInfo(placedObj.ItemTrans); // 위치 및 회전값 변환

            packet.HousingInfo.Add(info);
        }

        // 네트워크 매니저를 통해 패킷 전송 (InventoryManager.cs의 전송 방식과 유사)
        GameManager.Network.Send(packet);
        Debug.Log("하우징 저장 패킷 전송 완료. 데이터 개수: " + savedCount);
    }

    /// <summary>
    /// 서버에 하우징 로드 요청을 전송합니다.
    /// </summary>
    public void LoadHousingData()
    {
        // 로드 요청 전용 패킷 생성 (데이터가 존재하지 않으므로 별도의 파라미터 없이 빈 패킷)
        C2SHousingLoad loadPacket = new C2SHousingLoad();
        Debug.Log("하우징 로드 요청 전송");
        GameManager.Network.Send(loadPacket);
    }

    /// <summary>
    /// ObjectTransInfo를 protobuf의 TransformInfo로 변환하는 헬퍼 메소드
    /// </summary>
    /// <param name="objTrans">ObjectTransInfo 타입의 객체</param>
    /// <returns>프로토콜에 맞춘 TransformInfo 객체</returns>
    private TransformInfo ConvertToTransformInfo(ObjectTransInfo objTrans)
    {
        TransformInfo tInfo = new TransformInfo();
        // 예시로 Vector3Int를 각각 x, y, z 값으로 할당하고, 회전값은 ObjectYRotation으로 처리합니다.
        tInfo.PosX = objTrans.ObjectPosition.x;
        tInfo.PosY = objTrans.ObjectPosition.y;
        tInfo.PosZ = objTrans.ObjectPosition.z;
        tInfo.Rot = objTrans.ObjectYRotation;
        return tInfo;
    }

        #endregion
    }
