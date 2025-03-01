using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private static ResourceManager _instance;
    public static ResourceManager Instance => _instance ??= new ResourceManager();

    private readonly Dictionary<string, Dictionary<string, Object>> _resourceCache = new Dictionary<string, Dictionary<string, Object>>();

    private ResourceManager() { }

    public void RegisterResource<T>(string type, string key, T resource) where T : Object
    {
        if (!_resourceCache.TryGetValue(type, out var resourceDict))
        {
            resourceDict = new Dictionary<string, Object>();
            _resourceCache[type] = resourceDict;
        }

        if (!resourceDict.ContainsKey(key))
        {
            resourceDict[key] = resource;
        }
    }

    public T GetResource<T>(string type, string name) where T : Object
    {
        if (_resourceCache.TryGetValue(type, out var resourceDict) && resourceDict.TryGetValue(name, out var cachedResource))
        {
            return cachedResource as T;
        }

        Debug.LogWarning($"Resource not found: {name} in type: {type}");
        return null;
    }

    public void LoadAll<T>(string folderPath) where T : Object
    {
        string folderName = folderPath.Substring(folderPath.LastIndexOf('/') + 1);

        if (!_resourceCache.ContainsKey(folderName))
        {
            _resourceCache[folderName] = new Dictionary<string, Object>();
        }

        foreach (var resource in Resources.LoadAll<T>(folderPath))
        {
            RegisterResource(folderName, resource.name, resource);
        }
    }

    public T Instantiate<T>(string type, string name, Vector3 position, Quaternion rotation = default, Transform parent = null) where T : MonoBehaviour
    {
        if (_resourceCache.TryGetValue(type, out var resourceDict) && resourceDict.TryGetValue(name, out var cachedResource))
        {
            if (cachedResource is GameObject gameObject)
            {
                GameObject instance = Object.Instantiate(gameObject, position, rotation, parent);
                return instance.GetComponent<T>() ?? instance.AddComponent<T>();
            }
        }

        Debug.LogWarning($"Resource not found: {name} in type: {type}");
        return null;
    }

    public GameObject Instantiate(string type, string name, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        if (_resourceCache.TryGetValue(type, out var resourceDict) && resourceDict.TryGetValue(name, out var cachedResource))
        {
            if (cachedResource is GameObject gameObject)
            {
                return Object.Instantiate(gameObject, position, rotation, parent);
            }
        }

        Debug.LogWarning($"Resource not found: {name} in type: {type}");
        return null;
    }

    public void Destroy(GameObject go)
    {
        if (go != null)
        {
            Object.Destroy(go);
        }
    }
}
