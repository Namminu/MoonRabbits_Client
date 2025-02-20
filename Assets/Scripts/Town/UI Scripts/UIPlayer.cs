using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    private int player_level;
    private int player_exp;
    private int player_targetExp;
    private int player_stamina;
    private int player_cur_stamina;
    private int player_pickSpeed;
    private int player_moveSpeed;
    private int player_abilityPoint;
    private int player_cur_hp;
    private int player_hp;
    private string player_nickname;
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
        player_level = 1;
        player_hp = 100;
        player_cur_hp = player_hp;
        player_exp = 0;
        player_targetExp = 10;
        player_stamina = 100;
        player_cur_stamina = 100;
        player_pickSpeed = 5;
        player_moveSpeed = 10;
        player_abilityPoint = 0;

        btnAddExp.onClick.AddListener(OnClickAddExp);
        btnStaminaUp.onClick.AddListener(OnClickStaminaUp);
        btnPickSpeedUp.onClick.AddListener(OnClickPickSpeedUp);
        btnMoveSpeedUp.onClick.AddListener(OnClickMoveSpeedUp);
        hpSlider.value = player_cur_hp / player_hp;
        staminaSlider.value = player_cur_stamina / player_stamina;
        expSlider.value = player_exp / player_targetExp;
        hpText.text = $"{player_cur_hp} / {player_hp}";
        curStaminaText.text = $"{player_cur_stamina} / {player_stamina}";
        levelText.text = $"Lv{player_level}";
        staminaText.text = player_stamina.ToString();
        pickSpeedText.text = player_pickSpeed.ToString();
        moveSpeedText.text = player_moveSpeed.ToString();
        APText.text = player_abilityPoint.ToString();
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

        float targetValue = (float)player_exp / player_targetExp;
        StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, targetValue, 1f));
    }

    public void InvestPoint(StatInfo statInfo)
    {
        player_abilityPoint = statInfo.AbilityPoint;
        APText.text = player_abilityPoint.ToString();

        if(statInfo.Stamina > player_stamina){
            StaminaUp();
        }
        if(statInfo.PickSpeed > player_pickSpeed){
            PickSpeedUp();
        }
        if(statInfo.MoveSpeed > player_moveSpeed){
            MoveSpeedUp();
        }
    }
    void OnClickStaminaUp()
    {
        var pkt = new C2SInvestPoint
        {
            StatCode = 1,
        };
        GameManager.Network.Send(pkt);
    }

    void StaminaUp()
    {
        player_stamina++;
        //player_cur_stamina++;
        staminaText.text = player_stamina.ToString();
        curStaminaText.text = $"{player_cur_stamina} / {player_stamina}";
        staminaSlider.value = (float)player_cur_stamina / player_stamina;
        if (player_abilityPoint <= 0) DeActiveAP();
    }
    void OnClickPickSpeedUp()
    {
        var pkt = new C2SInvestPoint
        {
            StatCode = 2,
        };
        GameManager.Network.Send(pkt);
    }

    void PickSpeedUp()
    {
        player_pickSpeed++;
        pickSpeedText.text = player_pickSpeed.ToString();
        if (player_abilityPoint <= 0) DeActiveAP();
    }
    void OnClickMoveSpeedUp()
    {
        var pkt = new C2SInvestPoint
        {
            StatCode = 3,
        };
        GameManager.Network.Send(pkt);
    }

    void MoveSpeedUp()
    {
        player_moveSpeed++;
        moveSpeedText.text = player_moveSpeed.ToString();
        if (player_abilityPoint <= 0) DeActiveAP();
    }

    void DeActiveAP()
    {
        Vector3 goalPos_APButtons = APButtons.transform.position + new Vector3(0, APButtonsOffsetY, 0);
        Vector3 goalPos_APFrame = APFrame.transform.position + new Vector3(-APTextOffsetY, 0, 0);
        StartCoroutine(SmoothChangeObjectPosition(APButtons, APButtons.transform.position, goalPos_APButtons, 1));
        StartCoroutine(SmoothChangeObjectPosition(APFrame, APFrame.transform.position, goalPos_APFrame, 1));
    }

    public void LevelUp(int newLevel, int newTargetExp, int updatedExp, int abilityPoint)
    {
        StartCoroutine(LevelUpSequence(newLevel, newTargetExp, updatedExp, abilityPoint));
    }

    private IEnumerator LevelUpSequence(int newLevel, int newTargetExp, int updatedExp, int abilityPoint)
    {
        // 경험치바 끝까지 증가
        yield return StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, 1f, 1f));

        // 레벨 증가, 요구 경험치 증가
        player_level = newLevel;
        levelText.text = $"Lv{player_level}";
        player_targetExp = newTargetExp;

        // 기존에 올릴 수 있는 포인트가 0이면 ui 추가, 포인트가 남아있으면 ui 유지
        if (player_abilityPoint == 0)
        {
            Vector3 goalPos_APButtons = APButtons.transform.position + new Vector3(0, -APButtonsOffsetY, 0);
            Vector3 goalPos_APFrame = APFrame.transform.position + new Vector3(APTextOffsetY, 0, 0);
            StartCoroutine(SmoothChangeObjectPosition(APButtons, APButtons.transform.position, goalPos_APButtons, 1f));
            StartCoroutine(SmoothChangeObjectPosition(APFrame, APFrame.transform.position, goalPos_APFrame, 1f));
        }
        player_abilityPoint += abilityPoint;
        APText.text = player_abilityPoint.ToString();

        // 경험치바 변경된 경험치까지 증가
        player_exp = updatedExp;
        float targetValue = (float)player_exp / player_targetExp;
        StartCoroutine(SmoothChangeSliderValue(expSlider, 0f, targetValue, 1f));
    }

    public void SetNickname(string nickname)
    {
        player_nickname = nickname;
        nicknameText.text = $"{player_nickname}";
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
