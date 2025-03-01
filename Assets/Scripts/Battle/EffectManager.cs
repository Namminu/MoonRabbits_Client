using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;

    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 새로운 GameObject를 생성하고 EffectManager를 추가합니다.
                GameObject obj = new GameObject("EffectManager");
                _instance = obj.AddComponent<EffectManager>();
                _instance.Initialize(); // 생성 시 초기화 호출
                DontDestroyOnLoad(obj); // 씬 전환 시에도 유지
            }
            return _instance;
        }
    }

    private void Initialize()
    {
        // 이펙트 리소스를 로드합니다.
        ResourceManager.Instance.LoadAll<GameObject>("Prefabs/Effects");
    }

    public GameObject CreateTemporaryEffect(string effectName, Vector3 position, Quaternion rotation, float duration)
    {
        GameObject effectPrefab = ResourceManager.Instance.GetResource<GameObject>("Effects", effectName);

        if (effectPrefab != null)
        {
            GameObject effectInstance = Object.Instantiate(effectPrefab, position, rotation);
            Effect effectComponent = effectInstance.AddComponent<Effect>();
            effectComponent.Initialize(true, duration);
            return effectInstance;
        }

        Debug.LogError($"Effect not found: {effectName}");
        return null;
    }

    public GameObject CreatePersistentEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        GameObject effectPrefab = ResourceManager.Instance.GetResource<GameObject>("Effects", effectName);

        if (effectPrefab != null)
        {
            GameObject effectInstance = Object.Instantiate(effectPrefab, position, rotation);
            Effect effectComponent = effectInstance.AddComponent<Effect>();
            effectComponent.Initialize(false, 0);
            return effectInstance;
        }

        Debug.LogError($"Effect not found: {effectName}");
        return null;
    }
}
