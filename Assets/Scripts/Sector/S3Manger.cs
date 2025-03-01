using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class S3Manager : SManagerBase
{
    private static S3Manager _instance;
    public static S3Manager Instance => _instance;

    private int sectorCode = 103;
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
