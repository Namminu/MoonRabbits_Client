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
        // ���� ���� �����ϴ� InventoryUI �ν��Ͻ����� ��� ã���ϴ�.
        UIAnimation[] existingInstances = FindObjectsOfType<UIAnimation>(true);

        // ���� �̹� �� �� �̻��� �ν��Ͻ��� �����Ѵٸ� (�ڱ� �ڽ� �����Ͽ� 2�� �̻�)
        if (existingInstances.Length > 1)
        {
            // ���� ���� ��ü�� �ı��Ͽ� �ߺ� ������ ����
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
