using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IMouseHoverable
{
	// void OnMouseHoverEnter();
	// void OnMouseHoverExit();
	// void OnMouseClicked();
}

public class SceneChanger : MonoBehaviour, IMouseHoverable
{
	// [SerializeField] private SceneAsset nextScene;
	// [SerializeField] private GameObject sceneChangeUIPrefab;

	// private GameObject currentUIInstance;
	// private Vector3 originScale;

	// // Start is called before the first frame update
	// void Awake()
	// {
	// 	originScale = transform.localScale;
	// }

	// private void OnChangeScene()
	// {
	// 	if (nextScene == null) return;

	// 	SceneManager.LoadScene(nextScene.name);
	// 	EventManager.Unsubscribe("OnChangeScene", OnChangeScene);
	// }

	// #region Mouse Hover Interface
	// public void OnMouseClicked()
	// {
	// 	if (currentUIInstance != null) return;
	// 	EventManager.Subscribe("OnChangeScene", OnChangeScene);
	// 	currentUIInstance = Instantiate(sceneChangeUIPrefab);
	// }

	// public void OnMouseHoverEnter()
	// {
	// 	transform.localScale = originScale * 1.1f;
	// }

	// public void OnMouseHoverExit()
	// {
	// 	transform.localScale = originScale;
	// }
	// #endregion
}
