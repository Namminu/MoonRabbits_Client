using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	private static UIManager _instance;

	public static UIManager Instance
	{
		get
		{
			if (_instance == null)
			{
				// UIManager가 없을 경우 새로운 게임 오브젝트를 생성
				GameObject uiManagerObject = new GameObject("UIManager");
				_instance = uiManagerObject.AddComponent<UIManager>();
				DontDestroyOnLoad(uiManagerObject); // 씬이 변경되어도 파괴되지 않도록 설정
				_instance.LoadUIPrefabs(); // UI 프리팹 로드
			}
			return _instance;
		}
	}

	private Dictionary<string, GameObject> _uiPrefabs = new Dictionary<string, GameObject>();
	private Dictionary<string, GameObject> _uiInstances = new Dictionary<string, GameObject>();

	private void LoadUIPrefabs()
	{
		// Resources 폴더에서 모든 UI 프리팹 로드
		GameObject[] uiPrefabs = Resources.LoadAll<GameObject>("Prefabs/UI");
		foreach (var prefab in uiPrefabs)
		{
			_uiPrefabs[prefab.name] = prefab; // 이름을 키로 하여 등록
		}
	}

	// UI 인스턴스화
	public GameObject ShowUI(string uiName, Transform parent = null)
	{
		if (_uiPrefabs.TryGetValue(uiName, out GameObject prefab))
		{
			GameObject uiInstance = Object.Instantiate(prefab, parent);
			_uiInstances[uiName] = uiInstance; // 인스턴스 관리
			return uiInstance;
		}
		else
		{
			Debug.LogError($"UI Prefab Missing! {uiName}");
			return null;
		}
	}

	// UI 숨기기
	public void HideUI(string uiName)
	{
		if (_uiInstances.TryGetValue(uiName, out GameObject uiInstance))
		{
			Object.Destroy(uiInstance);
			_uiInstances.Remove(uiName); // 인스턴스 관리에서 제거
		}
		else
		{
			Debug.LogError($"UI Instance Missing! {uiName}");
		}
	}

	// 모든 UI 숨기기
	public void HideAllUI()
	{
		foreach (var kvp in _uiInstances)
		{
			Object.Destroy(kvp.Value);
		}
		_uiInstances.Clear();
	}
}
