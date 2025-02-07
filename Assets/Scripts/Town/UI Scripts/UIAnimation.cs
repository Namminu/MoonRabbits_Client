using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    //[SerializeField] private Button btnBattle;
    [SerializeField] private Button[] btnList;

    private MyPlayer mPlayer;

    void Start()
    {
        mPlayer = TownManager.Instance.MyPlayer?.MPlayer;

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