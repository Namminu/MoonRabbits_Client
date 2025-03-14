using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;

public class S1Manager : SManagerBase
{
    private static S1Manager _instance;
    public static S1Manager Instance => _instance;

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            base.Awake();
            SectorCode = 101;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetPrefabPath(); // 플레이어 프리펩 찾아갈 경로 미리 설정
    }
}
