using System;
using Google.Protobuf.Protocol;

public class SceneManagerEx
{
    private static SceneTransition _transition;
    private static bool _isTransition;

    public static void SetTransition()
    {
        if (_isTransition)
            return;

        _isTransition = true;

        _transition = UIManager
            .Instance.ShowUI("ChangeSceneCanvas")
            .GetComponent<SceneTransition>();

        // ResourceManager를 통해 리소스 로드
        //var resource = Managers.Resource.Load<GameObject>("Prefabs/UI/ChangeSceneCanvas");
        //_transition = GameObject.Instantiate(resource).GetComponent<SceneTransition>();
    }

    public static void LoadScene(string sceneName, Action action = null)
    {
        _transition.SetScene(sceneName, action);
    }
}
