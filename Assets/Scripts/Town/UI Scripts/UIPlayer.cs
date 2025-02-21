using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    private int APButtonsOffsetY = 60;
    private int APTextOffsetY = 130;

    public Button btnAddExp;
    public Slider hpSlider;
    public Slider staminaSlider;
    public Slider expSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI curStaminaText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI pickSpeedText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI APText;
    public GameObject APButtons;
    public GameObject APFrame;
    public Button btnStaminaUp;
    public Button btnPickSpeedUp;
    public Button btnMoveSpeedUp;

    // Start is called before the first frame update
    void Start()
    {
        btnAddExp.onClick.AddListener(OnClickAddExp);
        btnStaminaUp.onClick.AddListener(OnClickStaminaUp);
        btnPickSpeedUp.onClick.AddListener(OnClickPickSpeedUp);
        btnMoveSpeedUp.onClick.AddListener(OnClickMoveSpeedUp);
    }

    public void SetStatInfo(StatInfo statInfo)
    {
        int player_hp = 5;
        int player_cur_hp = 5;
        hpSlider.value = player_cur_hp / player_hp;
        staminaSlider.value = statInfo.Stamina / statInfo.Stamina;
        expSlider.value = statInfo.Exp / statInfo.TargetExp;
        hpText.text = $"{player_cur_hp} / {player_hp}";
        curStaminaText.text = $"{statInfo.CurStamina} / {statInfo.Stamina}";
        levelText.text = $"Lv{statInfo.Level}";
        staminaText.text = statInfo.Stamina.ToString();
        pickSpeedText.text = statInfo.PickSpeed.ToString();
        moveSpeedText.text = statInfo.MoveSpeed.ToString();
        APText.text = statInfo.AbilityPoint.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClickAddExp()
    {
        var addExpPacket = new C2SAddExp
        {
            Count = 4,
        };
        GameManager.Network.Send(addExpPacket);
    }

    public void SetExp(int updatedExp, int targetExp)
    {
        float targetValue = (float)updatedExp / targetExp;
        StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, targetValue, 1f));
    }

    public void SetAbilityPoint(int ap){
        APText.text = ap.ToString();
    }

    public void SetStamina(int cur_stamina, int stamina)
    {
        //player_cur_stamina++;
        staminaText.text = stamina.ToString();
        curStaminaText.text = $"{cur_stamina} / {stamina}";
        staminaSlider.value = (float)cur_stamina / stamina;
    }

    public void SetPickSpeed(int pickSpeed)
    {
        pickSpeedText.text = pickSpeed.ToString();
    }

    public void SetMoveSpeed(int moveSpeed)
    {
        moveSpeedText.text = moveSpeed.ToString();
    }

    public void SetNickname(string nickname)
    {
        nicknameText.text = $"{nickname}";
    }

    public void LevelUp(int newLevel, int newTargetExp, int updatedExp, int abilityPoint)
    {
        StartCoroutine(LevelUpSequence(newLevel, newTargetExp, updatedExp, abilityPoint));
    }
    public void DeActiveAP()
    {
        Vector3 goalPos_APButtons = APButtons.transform.position + new Vector3(0, APButtonsOffsetY, 0);
        Vector3 goalPos_APFrame = APFrame.transform.position + new Vector3(-APTextOffsetY, 0, 0);
        StartCoroutine(SmoothChangeObjectPosition(APButtons, APButtons.transform.position, goalPos_APButtons, 1));
        StartCoroutine(SmoothChangeObjectPosition(APFrame, APFrame.transform.position, goalPos_APFrame, 1));
    }

    void OnClickStaminaUp()
    {
        var pkt = new C2SInvestPoint
        {
            StatCode = 1,
        };
        GameManager.Network.Send(pkt);
    }

    public void OnClickPickSpeedUp()
    {
        var pkt = new C2SInvestPoint
        {
            StatCode = 2,
        };
        GameManager.Network.Send(pkt);
    }

    void OnClickMoveSpeedUp()
    {
        var pkt = new C2SInvestPoint
        {
            StatCode = 3,
        };
        GameManager.Network.Send(pkt);
    }



    private IEnumerator LevelUpSequence(int newLevel, int newTargetExp, int updatedExp, int abilityPoint)
    {
        Debug.Log("레벨업 코루틴");
        // 경험치바 끝까지 증가
        yield return StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, 1f, 1f));

        // 레벨 증가, 요구 경험치 증가
        levelText.text = $"Lv{newLevel}";

        // 기존에 올릴 수 있는 포인트가 0이면 ui 추가, 포인트가 남아있으면 ui 유지
        if (abilityPoint == 0)
        {
            Debug.Log("+버튼 코루틴");
            Vector3 goalPos_APButtons = APButtons.transform.position + new Vector3(0, -APButtonsOffsetY, 0);
            Vector3 goalPos_APFrame = APFrame.transform.position + new Vector3(APTextOffsetY, 0, 0);
            StartCoroutine(SmoothChangeObjectPosition(APButtons, APButtons.transform.position, goalPos_APButtons, 1f));
            StartCoroutine(SmoothChangeObjectPosition(APFrame, APFrame.transform.position, goalPos_APFrame, 1f));
        }
        APText.text = abilityPoint.ToString();

        // 경험치바 변경된 경험치까지 증가
        float targetValue = (float)updatedExp / newTargetExp;
        StartCoroutine(SmoothChangeSliderValue(expSlider, 0f, targetValue, 1f));
    }

    private IEnumerator SmoothChangeObjectPosition(GameObject gameObject, Vector3 startPos, Vector3 targetPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.position = targetPos;
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
