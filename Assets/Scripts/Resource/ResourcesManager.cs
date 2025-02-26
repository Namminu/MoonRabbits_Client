using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.Versioning;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    private static ResourcesManager _instance;
    public static ResourcesManager Instance => _instance;
    private GameObject[] resources;

    private JsonContainer<Resource> resourceContainer = GameManager.Instance.resourceContainer;
    // Start is called before the first frame update
    void Start()
    {
        resources = GameObject.FindGameObjectsWithTag("Resource").OrderBy(resource => resource.tag).ToArray();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResourcesInit(S2CResourceList pkt)
    {
        var resourcesPacket = pkt.Resources;

        foreach (var resource in resourcesPacket)
        {
            int resourceType = 0;
            foreach (var resourceJson in resourceContainer.data)
            {
                if (resourceJson.resource_id == resource.ResourceId)
                {
                    resourceType = resourceJson.resource_type == "Tree" ? 1 : 2;
                    break;
                }
            }
            var resourceController = resources[resource.ResourceIdx].GetComponent<ResourceController>();
            resourceController.idx = resource.ResourceIdx;
            resourceController.resourceId = resourceType;
        }
    }
    public void ResourcesUpdateDurability(S2CUpdateDurability pkt)
    {
        var resourceController = resources[pkt.PlacedId].GetComponent<ResourceController>();
        resourceController.Durability = pkt.Durability;
    }
    public void ResourcesGatheringStart(S2CGatheringStart pkt)
    {
        var resourceController = resources[pkt.PlacedId].GetComponent<ResourceController>();
        resourceController.Angle = pkt.Angle;
        resourceController. Difficulty = pkt.Difficulty;
        resourceController.Starttime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
    public void ResourcesGatheringSkillCheck(S2CGatheringSkillCheck pkt)
    {
        var resourceController = resources[pkt.PlacedId].GetComponent<ResourceController>();
        if( resourceController.Durability > pkt.Durability)
        {
            //성공
        }
        else
        {
            //실패
        }
    }


}
//ResourceIdx
//ResourceId