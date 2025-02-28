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

    // 사운드 정보 저장
    private Dictionary<Define.Sound, List<string>> _soundPaths = new Dictionary<Define.Sound, List<string>>();

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

        LoadSoundPaths();
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

    private void LoadSoundPaths()
    {
        // Loop 사운드 설명
        _soundPaths[Define.Sound.Bgm] = new List<string>
        {
            "Sounds/Loop/0",  // 행복한 동산에서 희망찬 꿈을 꾸는 아이들
            "Sounds/Loop/1",  // 평화 평온 평정심 명상
            "Sounds/Loop/2",  // 개구쟁이 아이들이 동산을 뛰어다니는 분위기
            "Sounds/Loop/3",  // 잔잔한 분위기의 석양이 지면서 헤어져야 할 분위기
            "Sounds/Loop/4",  // 로그인 화면에 들어갈것같은 분위기의 노래 테마곡 느낌
            "Sounds/Loop/5",  // 크레딧에 잘 어울리는 분위기의 노래
            "Sounds/Loop/6",  // 굉장히 수상한 분위기 도박장 혹은 퍼리퍼리 한 분위기
            "Sounds/Loop/7",  // 쉬어가는 휴식시간 같은 분위기
            "Sounds/Loop/8",  // 시상식 소감 이어가는 듯한 분위기
            "Sounds/Loop/9",  // 중국 산골자기의 카르마가 나타날것만 같은 분위기
            "Sounds/Loop/10", // 사막 한가온데의 마을같은 분위기
            "Sounds/Loop/11", // 이것봐 태초마을이야 할것같은 분위기
            "Sounds/Loop/12", // 시끌벅적 북적북적한 느낌의 신나는 분위기
            "Sounds/Loop/13", // 굉장히 수상해지는 분위기 한명 몰아가서 마피아로 만들것만 같은 분위기
            "Sounds/Loop/14", // 뱀파이어가 나올듯한 짙은 밤의 분위기
            "Sounds/Loop/15", // 사건이 일어나서 수사를 하는듯한 분위기
            "Sounds/Loop/16", // 축제 분위기 오늘 무슨 축제야?!
            "Sounds/Loop/17", // 시큰둥한 분위기 설레발 치지말고 진정하라는 듯한 느낌의 분위기
            "Sounds/Loop/18", // 저글링 4마리를 부를듯한 느낌
            "Sounds/Loop/19"  // 북적북적 한 분위기 신비로움 한줌 수상한 분위기
        };

        // Jingle 사운드 설명 (부정적, 긍정적 분위기)
        _soundPaths[Define.Sound.Jingle] = new List<string>
        {
            "Sounds/Jingle/0",  // 부정적인 분위기 0
            "Sounds/Jingle/1",  // 부정적인 분위기 1
            "Sounds/Jingle/2",  // 부정적인 분위기 2
            "Sounds/Jingle/3",  // 부정적인 분위기 3
            "Sounds/Jingle/4",  // 부정적인 분위기 4
            "Sounds/Jingle/5",  // 부정적인 분위기 5
            "Sounds/Jingle/6",  // 부정적인 분위기 6
            "Sounds/Jingle/7",  // 부정적인 분위기 7
            "Sounds/Jingle/8",  // 부정적인 분위기 8
            "Sounds/Jingle/9",  // 부정적인 분위기 9
            "Sounds/Jingle/10", // 긍정적인 분위기 0
            "Sounds/Jingle/11", // 긍정적인 분위기 1
            "Sounds/Jingle/12", // 긍정적인 분위기 2
            "Sounds/Jingle/13", // 긍정적인 분위기 3
            "Sounds/Jingle/14", // 긍정적인 분위기 4
            "Sounds/Jingle/15", // 긍정적인 분위기 5
            "Sounds/Jingle/16", // 긍정적인 분위기 6
            "Sounds/Jingle/17", // 긍정적인 분위기 7
            "Sounds/Jingle/18", // 긍정적인 분위기 8
            "Sounds/Jingle/19"  // 긍정적인 분위기 9
        };

        // Effect 사운드 설명
        _soundPaths[Define.Sound.Effect] = new List<string>
        {
            "Sounds/Effect/0",  // 8bit click
            "Sounds/Effect/1",  // Book Page
            "Sounds/Effect/2",  // Buzz Error
            "Sounds/Effect/3",  // Cash Register
            "Sounds/Effect/4",  // Click
            "Sounds/Effect/5",  // Sting
            "Sounds/Effect/6",  // 톡
            "Sounds/Effect/7",  // 맑고 청량한 퇑
            "Sounds/Effect/8",  // 띵
            "Sounds/Effect/9",  // 띠릭
            "Sounds/Effect/10", // 띠로링
            "Sounds/Effect/11", // 띠로리로링
            "Sounds/Effect/12", // 땅.땅.땅.띵!
            "Sounds/Effect/13", // 똥.똥.똥.띵!
            "Sounds/Effect/14", // 트릑. 틔릭. 트릐릭!
            "Sounds/Effect/15", // 팝!
            "Sounds/Effect/16", // 뽭!
            "Sounds/Effect/17", // 슬롯머신
            "Sounds/Effect/18", // 정답?!
            "Sounds/Effect/19", // 트뤽?!
            "Sounds/Effect/20", // 둔탁한 바람 소리
            "Sounds/Effect/21", // 파워업
            "Sounds/Effect/22", // 더 강한 파워업
            "Sounds/Effect/23", // 획득
            "Sounds/Effect/24", // 애매한 획득
            "Sounds/Effect/25", // 메시지 도착한 소리
            "Sounds/Effect/26", // 애매한 알림소리
            "Sounds/Effect/27", // 의미 불분명한 소리
            "Sounds/Effect/28", // 또롱!
            "Sounds/Effect/29", // 퐙뽭푸왑!
            "Sounds/Effect/30", // 도끼소리
            "Sounds/Effect/31", // 던지는소리
            "Sounds/Effect/32", // 장비변경 소리
            "Sounds/Effect/33", // 곡갱이 광석 소리
            "Sounds/Effect/34", // 곡갱이 돌 소리
            "Sounds/Effect/35", // 말벌 Loop 소리
        };
    }

    private void LoadAudioClips()
    {
        foreach (var soundType in _soundPaths.Keys)
        {
            foreach (var path in _soundPaths[soundType])
            {
                AudioClip audioClip = Managers.Resource.Load<AudioClip>(path);
                if (audioClip != null)
                {
                    _audioClips[path] = audioClip; // 등록
                }
                else
                {
                    Debug.Log($"AudioClip Missing! {path}");
                }
            }
        }
    }

    public void Play(int index, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (index < 0 || index >= _soundPaths[type].Count)
        {
            Debug.LogError("Invalid sound index");
            return;
        }

        string path = _soundPaths[type][index];
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
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

    private AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        // 경로에 "Sounds/"가 없으면 추가
        if (!path.Contains("Sounds/"))
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (!_audioClips.TryGetValue(path, out audioClip))
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing! {path}");

        return audioClip;
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

