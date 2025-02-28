using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private Dictionary<string, Object> _resourceCache = new Dictionary<string, Object>();

    public void RegisterResource(string key, Object resource)
    {
        if (!_resourceCache.ContainsKey(key))
        {
            _resourceCache[key] = resource;
        }
    }

    public T Load<T>(string path) where T : Object
    {
        string key = path.Substring(path.LastIndexOf('/') + 1);

        if (_resourceCache.TryGetValue(key, out Object cachedResource))
        {
            return cachedResource as T;
        }

        T resource = Resources.Load<T>(path);
        if (resource != null)
        {
            RegisterResource(key, resource);
        }

        return resource;
    }

    public void LoadAll<T>(string folderPath) where T : Object
    {
        T[] resources = Resources.LoadAll<T>(folderPath);
        foreach (var resource in resources)
        {
            string key = resource.name; // 리소스의 이름을 키로 사용
            RegisterResource(key, resource);
        }
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
