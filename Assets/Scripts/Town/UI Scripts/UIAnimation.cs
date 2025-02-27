using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    //[SerializeField] private Button btnBattle;
    [SerializeField] private Button[] btnList;

    private MyPlayer mPlayer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        mPlayer = TownManager.Instance.MyPlayer != null && TownManager.Instance.MyPlayer.MPlayer != null
        ? TownManager.Instance.MyPlayer.MPlayer
        : S1Manager.Instance.MyPlayer != null && S1Manager.Instance.MyPlayer.MPlayer != null
            ? S1Manager.Instance.MyPlayer.MPlayer
            : S2Manager.Instance.MyPlayer != null && S2Manager.Instance.MyPlayer.MPlayer != null
                ? S2Manager.Instance.MyPlayer.MPlayer
                : null;

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