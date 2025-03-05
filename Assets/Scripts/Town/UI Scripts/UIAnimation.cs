using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    //[SerializeField] private Button btnBattle;
    [SerializeField]
    private Button[] btnList;

    private MyPlayer mPlayer;

    private void Awake()
    {
        // 현재 씬에 존재하는 InventoryUI 인스턴스들을 모두 찾습니다.
        UIAnimation[] existingInstances = FindObjectsOfType<UIAnimation>(true);

        // 만약 이미 한 개 이상의 인스턴스가 존재한다면 (자기 자신 포함하여 2개 이상)
        if (existingInstances.Length > 1)
        {
            // 새로 들어온 객체는 파괴하여 중복 생성을 방지
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        mPlayer = GameManager.Instance.MPlayer.MPlayer;
        // mPlayer =
        //     TownManager.Instance.me != null && TownManager.Instance.me.MPlayer != null
        //         ? TownManager.Instance.me.MPlayer
        //     : S1Manager.Instance.me != null && S1Manager.Instance.me.MPlayer != null
        //         ? S1Manager.Instance.me.MPlayer
        //     : S2Manager.Instance.me != null && S2Manager.Instance.me.MPlayer != null
        //         ? S2Manager.Instance.me.MPlayer
        //     : null;

        if (mPlayer == null)
        {
            Debug.LogError("MyPlayer instance is missing or not initialized.");
            return;
        }

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < btnList.Length; i++)
        {
            int idx = i;
            btnList[i].onClick.AddListener(() => PlayAnimation(idx));
        }
    }

    private void PlayAnimation(int idx)
    {
        if (mPlayer == null)
        {
            Debug.LogWarning("Cannot play animation. MyPlayer instance is null.");
            return;
        }

        mPlayer.ExecuteAnimation(idx);
    }
}
