using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SoundManager();
                _instance.Init();
            }
            return _instance;
        }
    }

    private AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    private SoundManager() { } // private 생성자

    public void Init()
    {
        // AudioSource 배열 크기 설정
        _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++) // MaxCount는 마지막이므로 -1
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            // 배경 음악은 루프 설정
            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }

        LoadAudioClips(); // 사운드 클립 로드

        // 저장된 볼륨 값 불러오기
        if (PlayerPrefs.HasKey("Volume_Bgm"))
        {
            float savedBgmVolume = PlayerPrefs.GetFloat("Volume_Bgm");
            SetVolume(savedBgmVolume, Define.Sound.Bgm);
        }

        if (PlayerPrefs.HasKey("Volume_Effect"))
        {
            float savedEffectVolume = PlayerPrefs.GetFloat("Volume_Effect");
            SetVolume(savedEffectVolume, Define.Sound.Effect);
        }
    }

    private void LoadAudioClips()
    {
        // Effect 폴더에서 AudioClip 로드
        LoadAudioClipsFromFolder("Sounds/Effect", "Effect");
        // Jingle 폴더에서 AudioClip 로드
        LoadAudioClipsFromFolder("Sounds/Jingle", "Jingle");
        // Bgm 폴더에서 AudioClip 로드
        LoadAudioClipsFromFolder("Sounds/Bgm", "Bgm");
    }

    private void LoadAudioClipsFromFolder(string folderPath, string type)
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(folderPath);
        foreach (var audioClip in audioClips)
        {
            // 고유한 키 생성: "타입/파일이름"
            string key = $"{type}/{audioClip.name}";
            _audioClips[key] = audioClip; // 이름을 키로 하여 등록
        }
    }

    public void Play(int index, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        // 고유한 키 생성
        string soundType = type.ToString();
        string key = $"{soundType}/{index}"; // type에 따라 경로 설정
        if (_audioClips.TryGetValue(key, out AudioClip audioClip))
        {
            Play(audioClip, type, pitch);
        }
        else
        {
            Debug.Log($"AudioClip Missing! {key}");
        }
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void SetVolume(float volume, Define.Sound type = Define.Sound.Bgm)
    {
        if (type == Define.Sound.Bgm)
        {
            _audioSources[(int)Define.Sound.Bgm].volume = volume;
        }
        else if (type == Define.Sound.Effect)
        {
            _audioSources[(int)Define.Sound.Effect].volume = volume;
        }

        // 설정값 저장
        PlayerPrefs.SetFloat($"Volume_{type}", volume);
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }
}
