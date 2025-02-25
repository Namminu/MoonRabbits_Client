using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;

public class S2Manager : SManagerBase
{
    private static S2Manager _instance;
    public static S2Manager Instance => _instance;

    private int sectorCode = 102;
    public int SectorCode => sectorCode;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetPrefabPath(); // 플레이어 프리펩 찾아갈 경로 미리 설정
    }
}
