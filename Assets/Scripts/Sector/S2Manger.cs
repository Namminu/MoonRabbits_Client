using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class S2Manager : SManagerBase
{
    private static S2Manager _instance;
    public static S2Manager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            SectorCode = 102;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetPrefabPath(); // 플레이어 프리펩 찾아갈 경로 미리 설정
    }

    protected override void ActivateUI()
    {
        UiChat.gameObject.SetActive(true);
    }
}
