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

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            base.Awake();
            SectorCode = 103;
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
