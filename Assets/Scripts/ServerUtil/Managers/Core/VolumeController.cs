using System;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
  public Slider bgmSlider;
  public Slider effectSlider;

  public Toggle lowSpecMode;



  private void Awake()
  {
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

    lowSpecMode.onValueChanged.AddListener((value) =>
    {
      GameManager.Instance.IsLowSpecMode = value;
      PlayerPrefs.SetString($"LowSpecMode", value.ToString());
    });
    GameManager.Instance.IsLowSpecMode = Boolean.Parse(PlayerPrefs.GetString($"LowSpecMode"));
    lowSpecMode.isOn = GameManager.Instance.IsLowSpecMode;


  }
}
