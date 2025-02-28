using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    private static SceneTransition _transition;
    private static bool _isTransition;
    public static void SetTransition()
    {
        if (_isTransition) return;

        _isTransition = true;
        var resource = Resources.Load("Prefabs/UI/ChangeSceneCanvas");
        _transition = GameObject.Instantiate(resource).GetComponent<SceneTransition>();
    }
    public static void SetScene(string sceneName)
    {
        _transition.SetScene(sceneName);
    }

}
