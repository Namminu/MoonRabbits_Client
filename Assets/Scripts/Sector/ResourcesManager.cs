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

    public GameObject UIskillCheck;

    private GameObject[] resources;

    private JsonContainer<Resource> resourceContainer;

    private void Awake()
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
        resourceContainer = GameManager.Instance.resourceContainer;

        var pkt = new C2SResourcesList {  };

        GameManager.Network.Send(pkt);
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResourcesInit(S2CResourcesList pkt)
    {
        var resourcesPacket = pkt.Resources;
        resources = GameObject.FindGameObjectsWithTag("Resource").OrderBy(resource => resource.GetComponent<ResourceController>().idx).ToArray();
        foreach (var resource in resources)
        {
            Debug.Log(resource.name + ": " + resource.GetComponent<ResourceController>().idx);
        }

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
            if(resourceType != 0 && resources.Length >= resource.ResourceIdx)
            {
                var resourceController = resources[resource.ResourceIdx - 1].GetComponent<ResourceController>();
                resourceController.idx = resource.ResourceIdx;
                resourceController.resourceId = resourceType;
                resourceController.Durability = resource.Durability;
            }
        }
    }
    public void ResourcesUpdateDurability(S2CUpdateDurability pkt)
    {
        Debug.Log("자원 id" + pkt.PlacedId);
        Debug.Log("자원 durability" + pkt.Durability);
        var resourceController = resources[pkt.PlacedId - 1].GetComponent<ResourceController>();
        resourceController.Durability = pkt.Durability;
        
        if (pkt.Durability <= 0 && UISkillCheck.Instance.TargetResource == pkt.PlacedId) UISkillCheck.Instance.EndSkillCheck();
    }
    public void ResourcesGatheringStart(S2CGatheringStart pkt)
    {
        Debug.Log("자원 id" + pkt.PlacedId);
        var resourceController = resources[pkt.PlacedId - 1].GetComponent<ResourceController>();
        resourceController.ResourcesGatheringStart(pkt.Angle, pkt.Difficulty);
    }
    public void ResourcesGatheringSkillCheck(S2CGatheringSkillCheck pkt)
    {
        Debug.Log("자원 id" + pkt.PlacedId);
        var resourceController = resources[pkt.PlacedId - 1].GetComponent<ResourceController>();
        resourceController.ResourcesGatheringSkillCheck(pkt.Durability);
    }


}
//ResourceIdx
//ResourceId