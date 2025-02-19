using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiTest : MonoBehaviour
{
    private int player_level;
    private int player_exp;
    private int player_targetExp;
    private int player_stat1;
    private int player_stat2;
    private int player_speed;
    private int player_abilityPoint;
    private int player_hp;
    private int player_maxHp;
    private string player_nickname;

    public Button btnAddExp;
    public Slider hpSlider;
    public Slider expSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        player_level = 1;
        player_maxHp = 100;
        player_hp = player_maxHp;
        player_exp = 0;
        player_targetExp = 10;
        player_stat1 = 100;
        player_stat2 = 5;
        player_speed = 10;
        player_abilityPoint = 0;

        btnAddExp.onClick.AddListener(OnClickAddExp);
        hpSlider.value = player_hp / player_maxHp;
        expSlider.value = player_exp / player_targetExp;
        hpText.text = $"{player_hp} / {player_maxHp}";
        levelText.text = $"Lv{player_level}";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClickAddExp()
    {
        var chatPacket = new C2SChat
        {
            PlayerId = 1,
            SenderName = "nn",
            ChatMsg = "경험치 올리고싶어",
        };

        //AddExp(1);
        //GameManager.Network.Send(addExpPacket);

        var addExpPacket = new C2SAddExp
        {
            Count = 3,
        };
        GameManager.Network.Send(addExpPacket);
    }

    void AddExp(int plusExp)
    {
        player_exp += plusExp;
        expSlider.value = (float)player_exp / player_targetExp;
    }

    public void SetExp(int updatedExp)
    {
        int origin_exp = player_exp;
        player_exp = updatedExp;
        if (player_exp > player_targetExp)
        {
            Debug.Log("WTF! exp exceeded requirement for lv up");
            return;
        }
        Debug.Log($"updatedExp : {updatedExp}");

        //expSlider.value = (float)player_exp / player_targetExp;
        float targetValue = (float)player_exp / player_targetExp;
        StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, targetValue, 1f));
    }

    public void LevelUp(int newLevel, int newTargetExp, int updatedExp, int abilityPoint)
    {
        StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, 1, 1f));
        player_level = newLevel;
        player_exp = updatedExp;
        player_targetExp = newTargetExp;
        levelText.text = $"Lv{player_level}";
        //expSlider.value = player_exp / player_targetExp;
        player_abilityPoint = abilityPoint;
        
        float targetValue = (float)player_exp / player_targetExp;
        StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, targetValue, 1f));
    }

    public void SetNickname(string nickname)
    {
        player_nickname = nickname;
        nicknameText.text = $"{player_nickname}";
    }
    private IEnumerator SmoothChangeSliderValue(Slider slider, float startValue, float targetValue, float duration)
    {
        float elapsedTime = 0f;
        float maxValue = slider.maxValue;

        if (targetValue < startValue)
        {
            // Max 값까지 부드럽게 증가
            while (elapsedTime < duration)
            {
                slider.value = Mathf.Lerp(startValue, maxValue, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            slider.value = maxValue;

            // 남은 시간을 계산하여 초과 값 부드럽게 증가
            elapsedTime = 0f;
            startValue = 0f;

            while (elapsedTime < duration)
            {
                slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            slider.value = targetValue;
        }
        else
        {
            while (elapsedTime < duration)
            {
                slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            slider.value = targetValue;
        }
    }
}
