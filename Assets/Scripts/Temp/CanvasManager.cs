using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    private static CanvasManager _instance;
    public static CanvasManager Instance => _instance;

    public UIAnimation uIAnimation;
    public PartyUI partyUI;
    public UIPlayer uIPlayer;
    public UIChat uIChat;
    public UIBattlePopup uIBattlePopup;
    public InventoryUI inventoryUI;
    public UIEnter uIEnter;
    public UICraft uiCraft;
    public GameObject uiMenu;
    public CraftManager craftManager;

    public Player player;
    // public UIDisconnect disconnectPopup;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        uIAnimation = GetComponentInChildren<UIAnimation>();
        partyUI = GetComponentInChildren<PartyUI>();
        uIPlayer = GetComponentInChildren<UIPlayer>();
        uIChat = GetComponentInChildren<UIChat>();
        uIBattlePopup = GetComponentInChildren<UIBattlePopup>();
        inventoryUI = GetComponentInChildren<InventoryUI>();
        uIEnter = GetComponentInChildren<UIEnter>();
        uiCraft = GetComponentInChildren<UICraft>();
        craftManager = GetComponentInChildren<CraftManager>();
        // disconnectPopup = GetComponentInChildren<UIDisconnect>();
    }

    private void Start()
    {
        uIAnimation.gameObject.SetActive(false);
        uIPlayer.gameObject.SetActive(false);
        uIChat.gameObject.SetActive(false);
        uIBattlePopup.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
        uIEnter.gameObject.SetActive(false);
        uiCraft.gameObject.SetActive(false);
        // disconnectPopup.gameObject.SetActive(false);

        if (player == null)
        {
            StartCoroutine(nameof(EnterTownAfterUILoad));
        }
    }

    IEnumerator EnterTownAfterUILoad()
    {
        yield return new WaitUntil(
            () =>
                uIAnimation != null
                && uIPlayer != null
                && uIChat != null
                && uIBattlePopup != null
                && inventoryUI != null
                && uIEnter != null
        );

        SceneManager.LoadScene("Town");
    }

    public void ActivateUI()
    {
        uIAnimation.gameObject.SetActive(true);
        uIPlayer.gameObject.SetActive(true);
        uIChat.gameObject.SetActive(true);
        uiCraft.InitUiCraft();
    }

}
