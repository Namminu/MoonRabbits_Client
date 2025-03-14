using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    //[SerializeField] private Button btnBattle;
    [SerializeField]
    private Button[] btnList;

    private MyPlayer mPlayer;

    private Button btnRankingUI;
    private Button btnCraftUI;
    private Button btnInventoryUI;
    private Button btnPartyUI;

    private void Awake()
    {
        btnRankingUI = transform.Find("Button_Ranking").GetComponent<Button>();
        btnCraftUI = transform.Find("Button_Craft").GetComponent<Button>();
        btnInventoryUI = transform.Find("Button_Inventory").GetComponent<Button>();
        btnPartyUI = transform.Find("Button_Party").GetComponent<Button>();
    }

    void Start()
    {
        btnRankingUI.onClick.AddListener(OnBtnRankingUIClick);
        btnCraftUI.onClick.AddListener(OnBtnCraftUIClick);
        btnInventoryUI.onClick.AddListener(OnBtnInventoryUIClick);
        btnPartyUI.onClick.AddListener(OnBtnPartyUIClick);
        // mPlayer = GameManager.Instance.MPlayer.MPlayer;
        // mPlayer =
        //     TownManager.Instance.me != null && TownManager.Instance.me.MPlayer != null
        //         ? TownManager.Instance.me.MPlayer
        //     : S1Manager.Instance.me != null && S1Manager.Instance.me.MPlayer != null
        //         ? S1Manager.Instance.me.MPlayer
        //     : S2Manager.Instance.me != null && S2Manager.Instance.me.MPlayer != null
        //         ? S2Manager.Instance.me.MPlayer
        //     : null;

        // if (mPlayer == null)
        // {
        //     Debug.LogError("MyPlayer instance is missing or not initialized.");
        //     return;
        // }

        // InitializeButtons();
    }

    // private void InitializeButtons()
    // {
    //     for (int i = 0; i < btnList.Length; i++)
    //     {
    //         int idx = i;
    //         btnList[i].onClick.AddListener(() => PlayAnimation(idx));
    //     }
    // }

    // private void PlayAnimation(int idx)
    // {
    //     if (mPlayer == null)
    //     {
    //         Debug.LogWarning("Cannot play animation. MyPlayer instance is null.");
    //         return;
    //     }

    //     mPlayer.ExecuteAnimation(idx);
    // }

    public void OnBtnRankingUIClick()
    {
        GameObject uiRanking = CanvasManager.Instance.uiRanking.gameObject;
        uiRanking.SetActive(!uiRanking.activeSelf);
        uiRanking.transform.SetAsLastSibling();
    }

    public void OnBtnCraftUIClick()
    {
        GameObject uiCraft = CanvasManager.Instance.uiCraft.gameObject;
        uiCraft.SetActive(!uiCraft.activeSelf);
        uiCraft.transform.SetAsLastSibling();
    }

    public void OnBtnInventoryUIClick()
    {
        GameObject uiInven = CanvasManager.Instance.inventoryUI.gameObject;
        uiInven.SetActive(!uiInven.activeSelf);
        uiInven.transform.SetAsLastSibling();
    }

    public void OnBtnPartyUIClick()
    {
        GameObject partyWindow = CanvasManager.Instance.partyUI.partyWindow;
        partyWindow.SetActive(!partyWindow.activeSelf);
        GameObject partyUI = CanvasManager.Instance.partyUI.gameObject;
        partyUI.transform.SetAsLastSibling();
    }
}
