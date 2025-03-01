using System.Collections;
using UnityEngine;

public class Managers : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static Managers s_instance;
    private static bool _isInitialized = false; // 초기화 여부 확인 플래그
    private static Managers Instance
    {
        get
        {
            Init();
            return s_instance;
        }
    }

    #region Contents Managers
    private ObjectManager _obj = new ObjectManager();
    private NetworkManager _network = new NetworkManager();

    public static ObjectManager Obj => Instance._obj;
    public static NetworkManager Network => Instance._network;
    #endregion

    #region Core Managers
    private PoolManager _pool = new PoolManager();
    private SceneManagerEx _scene = new SceneManagerEx();

    private UIManager _ui = new UIManager();

    public static PoolManager Pool => Instance._pool;
    public static SceneManagerEx Scene => Instance._scene;

    public static UIManager UI => Instance._ui;
    #endregion

    // Unity 메서드
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        // 필요 시 다른 매니저도 추가 가능
        _network?.Update();
    }

    // 초기화 메서드
    private static void Init()
    {
        if (_isInitialized)
            return;

        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            // 개별 매니저 초기화
            // s_instance._network?.Init();
            s_instance._pool?.Init();

        }

        _isInitialized = true;
    }

    // 모든 매니저를 초기화 상태로 리셋
    public static void Clear()
    {
        Pool.Clear();
    }
}
