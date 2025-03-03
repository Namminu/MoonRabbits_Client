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

    //일정시간 동안 실행되어야하는 특정 좌표가 있는 이팩트

    public GameObject CreateTemporaryEffect(string effectName, Vector3 position, Quaternion rotation, float duration)
    {
        return CreateEffect(effectName, position, rotation, true, duration);
    }

    // 이건 일정 시간 동안 실행되어야 하는 부모가 있는 이팩트
    public GameObject CreateTemporaryEffect(string effectName, Transform parent, float duration)
    {
        return CreateEffect(effectName, default, default, true, duration, parent);
    }

    // 이건 지속적으로 실행되어야 하는 이펙트 자연물 같은 느낌
    public GameObject CreatePersistentEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        return CreateEffect(effectName, position, rotation, false, 0);
    }

    private GameObject CreateEffect(string effectName, Vector3 position, Quaternion rotation, bool isTemporary, float duration, Transform parent = null)
    {
        // ResourceManager의 Instantiate 메서드를 사용하여 이펙트를 생성합니다.
        GameObject effectInstance = ResourceManager.Instance.Instantiate("Effects", effectName, position, rotation, parent);

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
