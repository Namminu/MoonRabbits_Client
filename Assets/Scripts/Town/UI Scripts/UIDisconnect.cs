using UnityEngine;
using UnityEngine.UI;

public class UIDisconnect : MonoBehaviour
{
    // Singleton 형태로 전역 접근이 가능하도록 합니다.
    public static UIDisconnect Instance { get; private set; }

    // Inspector에서 할당할 Disconnect 팝업 패널
    [SerializeField]
    private GameObject disconnectPopup;

    // Inspector에서 할당할 확인 버튼 (팝업 내 버튼)
    [SerializeField]
    private Button confirmButton;

    private void Awake()
    {
        // Singleton 패턴 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Inspector에 연결되지 않은 경우, 자식에서 "DisconnectPopup"이라는 이름의 GameObject를 찾기
        if (disconnectPopup == null)
        {
            Transform popupTransform = transform.Find("UIDisconnect");
            if (popupTransform != null)
            {
                disconnectPopup = popupTransform.gameObject;
            }
            else
            {
                Debug.LogError("UIDisconnect: 'UIDisconnect'이라는 자식 오브젝트를 찾을 수 없습니다.");
            }
        }

        // Inspector에 연결되지 않은 경우, "DisconnectPopup/ConfirmButton" 경로로 확인 버튼 찾기
        if (confirmButton == null)
        {
            Transform buttonTransform = transform.Find("UIDisconnect/OfflineReward/Bg/btnConfirm");
            if (buttonTransform != null)
            {
                confirmButton = buttonTransform.GetComponent<Button>();
            }
            else
            {
                Debug.LogError("UIDisconnect: 'UIDisconnect/OfflineReward/Bg/btnConfirm' 경로의 버튼을 찾을 수 없습니다.");
            }
        }

        // 팝업 비활성화 처리
        if (disconnectPopup != null)
            disconnectPopup.SetActive(false);
        else
            Debug.LogError("UIDisconnect: disconnectPopup이 할당되지 않았습니다.");

        // 확인 버튼 이벤트 등록
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmDisconnect);
        else
            Debug.LogError("UIDisconnect: confirmButton이 할당되지 않았습니다.");
    }

    // 외부 호출용 함수 – 팝업을 활성화시킴
    public void ShowDisconnectPopup()
    {
        if (disconnectPopup != null)
            disconnectPopup.SetActive(true);
    }

    // 확인 버튼 클릭 시 호출될 함수
    public void OnConfirmDisconnect()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}