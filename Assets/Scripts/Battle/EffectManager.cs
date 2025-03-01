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
                GameObject obj = new GameObject("EffectManager");
                _instance = obj.AddComponent<EffectManager>();
                ResourceManager.Instance.LoadAll<GameObject>("Prefabs/Effects");
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public GameObject CreateTemporaryEffect(string effectName, Vector3 position, Quaternion rotation, float duration)
    {
        return CreateEffect(effectName, position, rotation, true, duration);
    }

    public GameObject CreatePersistentEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        return CreateEffect(effectName, position, rotation, false, 0);
    }

    private GameObject CreateEffect(string effectName, Vector3 position, Quaternion rotation, bool isTemporary, float duration)
    {
        // ResourceManager의 Instantiate 메서드를 사용하여 이펙트를 생성합니다.
        GameObject effectInstance = ResourceManager.Instance.Instantiate("Effects", effectName, position, rotation);

        if (effectInstance != null)
        {
            Effect effectComponent = effectInstance.AddComponent<Effect>();
            effectComponent.Initialize(isTemporary, duration);
            return effectInstance;
        }

        Debug.LogError($"Effect not found: {effectName}");
        return null;
    }
}
