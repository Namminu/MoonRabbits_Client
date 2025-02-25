using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;

public class S1Manager : SManagerBase
{
    private static S1Manager _instance;
    public static S1Manager Instance => _instance;

    private int sectorCode = 101;
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
    }
}
