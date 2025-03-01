using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private Dictionary<string, Dictionary<string, Object>> _resourceCache = new Dictionary<string, Dictionary<string, Object>>();

    public void RegisterResource<T>(string type, string key, T resource) where T : Object
    {
        if (!_resourceCache.ContainsKey(type))
        {
            _resourceCache[type] = new Dictionary<string, Object>();
        }

        if (!_resourceCache[type].ContainsKey(key))
        {
            _resourceCache[type][key] = resource;
        }
    }

    public T GetResource<T>(string type, string name) where T : Object
    {
        if (_resourceCache.ContainsKey(type) && _resourceCache[type].TryGetValue(name, out Object cachedResource))
        {
            return cachedResource as T; // T로 캐스팅하여 반환
        }

        Debug.Log($"Resource not found: {name} in type: {type}");
        return null;
    }

    public void LoadAll<T>(string folderPath) where T : Object
    {
        string folderName = folderPath.Substring(folderPath.LastIndexOf('/') + 1); // 마지막 폴더 이름 추출

        if (!_resourceCache.ContainsKey(folderName))
        {
            _resourceCache[folderName] = new Dictionary<string, Object>();
        }

        T[] resources = Resources.LoadAll<T>(folderPath);

        foreach (var resource in resources)
        {
            string key = resource.name; // 파일 이름을 키로 사용
            RegisterResource(folderName, key, resource);
        }
    }

    public T Instantiate<T>(string type, string name, Vector3 position, Quaternion rotation = default, Transform parent = null) where T : MonoBehaviour
    {
        if (_resourceCache.ContainsKey(type) && _resourceCache[type].TryGetValue(name, out Object cachedResource))
        {
            // T로 캐스팅
            if (cachedResource is GameObject gameObject)
            {
                GameObject instance = Object.Instantiate(gameObject, position, rotation, parent);
                T component = instance.GetOrAddComponent<T>();

                return component; // 인스턴스화된 컴포넌트 반환
            }
            else
            {
                Debug.Log($"Resource is not a GameObject: {name}");
                return null;
            }
        }

        Debug.Log($"Resource not found: {name} in type: {type}");
        return null;
    }

    public GameObject Instantiate(string type, string name, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        if (_resourceCache.ContainsKey(type) && _resourceCache[type].TryGetValue(name, out Object cachedResource))
        {
            // T로 캐스팅
            if (cachedResource is GameObject gameObject)
            {
                GameObject instance = Object.Instantiate(gameObject, position, rotation, parent);

                return instance; // 인스턴스화된 컴포넌트 반환
            }
            else
            {
                Debug.Log($"Resource is not a GameObject: {name}");
                return null;
            }
        }

        Debug.Log($"Resource not found: {name} in type: {type}");
        return null;
    }



    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}
