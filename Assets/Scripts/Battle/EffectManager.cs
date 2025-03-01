using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private ResourceManager _resourceManager;

    private void Awake()
    {
        _resourceManager = new ResourceManager();

        // 이펙트 리소스를 로드합니다.
        _resourceManager.LoadAll<GameObject>("Prefabs/Effects");
    }

    public GameObject CreateEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        // 이펙트를 인스턴스화합니다.
        GameObject effectInstance = _resourceManager.Instantiate("Effects", effectName, position, rotation);

        if (effectInstance != null)
        {
            effectInstance.transform.position = position;
            effectInstance.transform.rotation = rotation;
            return effectInstance;
        }

        Debug.LogError($"Effect not found: {effectName}");
        return null;
    }
}
