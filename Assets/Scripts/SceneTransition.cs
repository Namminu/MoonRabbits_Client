using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    private bool _isPersisting = false; // 씬 전환 시 오브젝트를 유지할지 여부

    [SerializeField] private RectTransform topShutter;  // 위쪽 셔터 패널
    [SerializeField] private RectTransform bottomShutter; // 아래쪽 셔터 패널

    [SerializeField] private CanvasGroup cg; // 캔버스 그룹
    [SerializeField] private Image image; // 씬 전환 이미지
    [SerializeField] private Slider progressBar; // 프로그래스 바
    [SerializeField] private TextMeshProUGUI tmp; // 씬 이름 표시 텍스트

    private const string DefaultSceneName = "Sector1"; // 기본 씬 이름
    [SerializeField] private string sceneName = DefaultSceneName; // 현재 씬 이름

    private void OnEnable()
    {
        if (!_isPersisting)
        {
            DontDestroyOnLoad(gameObject);
            _isPersisting = true;
            gameObject.SetActive(false);

            // ResourceManager를 통해 모든 스프라이트 로드
            Managers.Resource.LoadAllSprites("SceneTransition");
            return;
        }
        LoadScene();
    }

    public void SetScene(string sceneName)
    {
        this.sceneName = sceneName; // 씬 이름 설정
        gameObject.SetActive(true); // 씬 전환 UI 활성화
    }

    private void LoadScene()
    {
        StartCoroutine(ShutterAndLoad()); // 씬 전환 코루틴 시작
    }

    private IEnumerator ShutterAndLoad()
    {
        // 씬 이름에 해당하는 스프라이트 설정
        Sprite sprite = Managers.Resource.GetSprite(sceneName);
        if (sprite != null)
            image.sprite = sprite;

        tmp.text = sceneName; // 씬 이름 텍스트 설정
        yield return CloseShutters(); // 셔터 닫기
        yield return FadeIn(); // 페이드 인
        yield return LoadSceneAsync(); // 씬 비동기 로드
        ApplyRenderSettings(); // 렌더 설정 적용
        yield return FadeOut(); // 페이드 아웃
        yield return OpenShutters(); // 셔터 열기

        gameObject.SetActive(false); // UI 비활성화
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // 즉시 씬 전환 방지

        while (!operation.isDone)
        {
            progressBar.value = operation.progress; // 프로그래스 바 업데이트
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true; // 씬 전환 허용
            }
            yield return null; // 다음 프레임 대기
        }
        progressBar.value = 0f; // 프로그래스 바 초기화
    }

    private IEnumerator CloseShutters()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(topShutter.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(bottomShutter.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutQuad));

        yield return sequence.WaitForCompletion(); // 애니메이션 완료 대기
    }

    private IEnumerator OpenShutters()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(topShutter.DOAnchorPosY(540, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(bottomShutter.DOAnchorPosY(-540, 0.5f).SetEase(Ease.OutQuad));

        yield return sequence.WaitForCompletion(); // 애니메이션 완료 대기
    }

    private IEnumerator FadeIn()
    {
        cg.alpha = 0f; // 초기 투명도 설정
        cg.gameObject.SetActive(true); // 캔버스 그룹 활성화
        yield return cg.DOFade(1, 0.5f).WaitForCompletion(); // 페이드 인
    }

    private IEnumerator FadeOut()
    {
        yield return cg.DOFade(0, 0.5f).WaitForCompletion(); // 페이드 아웃
        cg.gameObject.SetActive(false); // 캔버스 그룹 비활성화
    }

    void ApplyRenderSettings()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox; // 환경 설정
        DynamicGI.UpdateEnvironment(); // 환경 업데이트
    }
}
