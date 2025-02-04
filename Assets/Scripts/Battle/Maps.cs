using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maps : MonoBehaviour
{
    [SerializeField] private Skybox sky;
    [SerializeField] private Material[] skies;
    [SerializeField] private GameObject[] maps;

    private int mapId = 0;

    public void SetMap(int id)
    {
        var calcId = id - Constants.DungeonCodeFactor;
        
        if(calcId < 0 || calcId >= maps.Length)
            return;
        
        maps[mapId].SetActive(false);
        mapId = calcId;

        maps[mapId].SetActive(true);
        sky.material = skies[mapId];
    }
}
