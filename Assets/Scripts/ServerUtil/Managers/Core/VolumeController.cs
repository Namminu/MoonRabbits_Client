using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
  public Slider bgmSlider;
  public Slider effectSlider;

  private void Awake()
  {
        // 현재 씬에 존재하는 InventoryUI 인스턴스들을 모두 찾습니다.
        VolumeController[] existingInstances = FindObjectsOfType<VolumeController>(true);

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
    // 슬라이더 값 설정 (저장된 값 불러오기)
    bgmSlider.value = PlayerPrefs.HasKey("Volume_Bgm") ? PlayerPrefs.GetFloat("Volume_Bgm") : 0.5f;
    effectSlider.value = PlayerPrefs.HasKey("Volume_Effect") ? PlayerPrefs.GetFloat("Volume_Effect") : 0.5f;

    // 슬라이더 변경 시 볼륨 조절
    bgmSlider.onValueChanged.AddListener((value) => SoundManager.Instance.SetVolume(value, Define.Sound.Bgm));
    effectSlider.onValueChanged.AddListener((value) => SoundManager.Instance.SetVolume(value, Define.Sound.Effect));
  }
}
