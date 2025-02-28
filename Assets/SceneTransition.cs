using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isDondestroy = false;

    public RectTransform topShutter;  // 위쪽 셔터 패널
    public RectTransform bottomShutter; // 아래쪽 셔터 패널

    public CanvasGroup cg;

    public Slider progressBar;

    public TextMeshProUGUI tmp;

    [SerializeField] private string sceneName = "Sector1";

    private void OnEnable()
    {
        if (isDondestroy == false)
        {
            DontDestroyOnLoad(gameObject);
            isDondestroy = true;
            gameObject.SetActive(false);
            return;
        }
        LoadScene();
    }

    public void SetScene(string sceneName)
    {
        this.sceneName = sceneName;
        gameObject.SetActive(true);
    }

    public void LoadScene()
    {
        StartCoroutine(ShutterAndLoad());
    }

    private IEnumerator ShutterAndLoad()
    {
        tmp.text = sceneName;
        yield return CloseShutters();
        yield return FadeIn();

        yield return LoadSceneAsync();

        ApplyRenderSettings();

        yield return FadeOut();
        yield return OpenShutters();

        gameObject.SetActive(false);
    }

    //프로그래스바 업데이트 끝나면 씬전환
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // 즉시 씬 전환 방지

        // 프로그래스 바 업데이트
        while (!operation.isDone)
        {
            progressBar.value = operation.progress; // 프로그래스 바 업데이트
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true; // 씬 전환 허용
            }
            yield return null; // 다음 프레임 대기
        }
        progressBar.value = 0f;
    }
    private IEnumerator CloseShutters()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(topShutter.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(bottomShutter.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutQuad));

        yield return sequence.WaitForCompletion();
    }

    private IEnumerator OpenShutters()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(topShutter.DOAnchorPosY(540, 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(bottomShutter.DOAnchorPosY(-540, 0.5f).SetEase(Ease.OutQuad));

        yield return sequence.WaitForCompletion();
    }

    private IEnumerator FadeIn()
    {
        cg.alpha = 0f;
        cg.gameObject.SetActive(true);
        yield return cg.DOFade(1, 0.5f).WaitForCompletion(); // 페이드 인
    }

    private IEnumerator FadeOut()
    {
        yield return cg.DOFade(0, 0.5f).WaitForCompletion(); // 페이드 아웃
        cg.gameObject.SetActive(false);
    }

    //Unity Editor 에서 로드시 색감차이 나는 문제를 해결하기 위한 함수
    void ApplyRenderSettings()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        DynamicGI.UpdateEnvironment();
    }
}
