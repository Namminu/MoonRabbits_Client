using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public static UIPlayer instance { get; private set; }
    public int player_level;

    private int APButtonsOffsetY = 39;
    private int APTextOffsetX = 152;

    public Button btnAddExp;
    public Slider staminaSlider;
    public Slider expSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI pickSpeedText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI APText;
    public GameObject APButtons;
    public GameObject APFrame;
    public Button btnStaminaUp;
    public Button btnPickSpeedUp;
    public Button btnMoveSpeedUp;
    public GameObject heartPrefab;
    public GameObject heartBgPrefab;
    public Transform heartsPos;
    public Transform heartBgsPos;
    private List<GameObject> hearts = new List<GameObject>();
    private List<GameObject> heartBgs = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        btnAddExp.onClick.AddListener(OnClickAddExp);
        btnStaminaUp.onClick.AddListener(OnClickStaminaUp);
        btnPickSpeedUp.onClick.AddListener(OnClickPickSpeedUp);
        btnMoveSpeedUp.onClick.AddListener(OnClickMoveSpeedUp);
    }

    public void OnClickAddExp()
    {
        var addExpPacket = new C2SAddExp
        {
            Count = 9,
        };
        GameManager.Network.Send(addExpPacket);
    }

    void OnClickStaminaUp()
    {
        Debug.Log("스테미나 올림");
        var pkt = new C2SInvestPoint
        {
            StatCode = 1,
        };
        GameManager.Network.Send(pkt);
    }

    public void OnClickPickSpeedUp()
    {
        Debug.Log("채집속도 올림");
        var pkt = new C2SInvestPoint
        {
            StatCode = 2,
        };
        GameManager.Network.Send(pkt);
    }

    void OnClickMoveSpeedUp()
    {
        Debug.Log("이동속도 올림");
        var pkt = new C2SInvestPoint
        {
            StatCode = 3,
        };
        GameManager.Network.Send(pkt);
    }

    public void SetStatInfo(StatInfo statInfo)
    {
        staminaSlider.value = (float)statInfo.Stamina / statInfo.Stamina;
        expSlider.value = (float)statInfo.Exp / statInfo.TargetExp;
        levelText.text = $"Lv{statInfo.Level}";
        staminaText.text = statInfo.Stamina.ToString();
        pickSpeedText.text = statInfo.PickSpeed.ToString();
        moveSpeedText.text = statInfo.MoveSpeed.ToString();
        APText.text = statInfo.AbilityPoint.ToString();
        if (statInfo.AbilityPoint > 0)
        {
            Debug.Log(APButtons.transform.position);
            Debug.Log(APFrame.transform.position);
            Vector3 goalPos_APButtons = APButtons.transform.position + new Vector3(0, -APButtonsOffsetY, 0);
            Vector3 goalPos_APFrame = APFrame.transform.position + new Vector3(APTextOffsetX, 0, 0);
            Debug.Log(goalPos_APButtons);
            Debug.Log(goalPos_APFrame);
            // 처음부터 위치 이동시키면 이상한 위치로 이동돼서 코루틴으로 이동시킴
            // APButtons.transform.position = goalPos_APButtons;
            // APFrame.transform.position = goalPos_APFrame;
            StartCoroutine(SmoothChangeObjectPosition(APButtons, APButtons.transform.position, goalPos_APButtons, 1));
            StartCoroutine(SmoothChangeObjectPosition(APFrame, APFrame.transform.position, goalPos_APFrame, 1));
        }
    }

    public void SetNickname(string nickname)
    {
        nicknameText.text = $"{nickname}";
    }

    public void SetExp(int updatedExp, int targetExp)
    {
        float targetValue = (float)updatedExp / targetExp;
        StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, targetValue, 1f));
    }

    public void SetAbilityPoint(int ap)
    {
        APText.text = ap.ToString();
    }
    public void SetStamina(int cur_stamina, int stamina, bool hasAP)
    {
        staminaText.text = stamina.ToString();
        staminaSlider.maxValue = stamina;
        staminaSlider.value = cur_stamina;
        if (!hasAP) DeActiveAP();
    }

    public void SetPickSpeed(int pickSpeed, bool hasAP)
    {
        pickSpeedText.text = pickSpeed.ToString();
        if (!hasAP) DeActiveAP();
    }

    public void SetMoveSpeed(int moveSpeed, bool hasAP)
    {
        moveSpeedText.text = moveSpeed.ToString();
        if (!hasAP) DeActiveAP();
    }

    public void LevelUp(int newLevel, int newTargetExp, int updatedExp, int abilityPoint, int updatedAbilityPoint)
    {
        StartCoroutine(LevelUpSequence(newLevel, newTargetExp, updatedExp, abilityPoint, updatedAbilityPoint));
    }
    public void DeActiveAP()
    {
        Vector3 goalPos_APButtons = APButtons.transform.position + new Vector3(0, APButtonsOffsetY, 0);
        Vector3 goalPos_APFrame = APFrame.transform.position + new Vector3(-APTextOffsetX, 0, 0);
        StartCoroutine(SmoothChangeObjectPosition(APButtons, APButtons.transform.position, goalPos_APButtons, 1));
        StartCoroutine(SmoothChangeObjectPosition(APFrame, APFrame.transform.position, goalPos_APFrame, 1));
    }

    private IEnumerator LevelUpSequence(int newLevel, int newTargetExp, int updatedExp, int abilityPoint, int updatedAbilityPoint)
    {
        // 경험치바 끝까지 증가
        yield return StartCoroutine(SmoothChangeSliderValue(expSlider, expSlider.value, 1f, 1f));

        // 레벨 증가, 요구 경험치 증가
        levelText.text = $"Lv{newLevel}";

        // 파티원 정보 UI도 업데이트
        PartyMemberUI.instance.UpdateUI();

        // 기존에 올릴 수 있는 포인트가 0이었으면 ui 추가, 포인트가 남아있으면 ui 유지
        if (abilityPoint == 0)
        {
            Debug.Log("+버튼 코루틴");
            Vector3 goalPos_APButtons = APButtons.transform.position + new Vector3(0, -APButtonsOffsetY, 0);
            Vector3 goalPos_APFrame = APFrame.transform.position + new Vector3(APTextOffsetX, 0, 0);
            StartCoroutine(SmoothChangeObjectPosition(APButtons, APButtons.transform.position, goalPos_APButtons, 1f));
            StartCoroutine(SmoothChangeObjectPosition(APFrame, APFrame.transform.position, goalPos_APFrame, 1f));
        }
        APText.text = updatedAbilityPoint.ToString();

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

    // HP
    public void InitHp(int hp) // 접속시 HP 초기 설정
    {
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        for (int i = 0; i < hp; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsPos);
            GameObject heartBg = Instantiate(heartBgPrefab, heartBgsPos);
            hearts.Add(heart);
            heartBgs.Add(heartBg);
        }
    }

    private void AddHp() // HP 증가 -> hearts 그리드에 하트 추가
    {
        if (hearts.Count <= heartBgs.Count)
        {
            GameObject heart = Instantiate(heartPrefab, heartsPos);
            hearts.Add(heart);
        }
    }

    private void SubHp() // HP 감소 -> hearts 그리드에서 하트 삭제
    {
        if (hearts.Count > 0 && heartBgs.Count > 0)
        {
            GameObject heart = hearts[hearts.Count - 1];
            Destroy(heart);
            hearts.RemoveAt(hearts.Count - 1);
        }
    }

    private void AddMaxHp() // 최대 HP 증가 -> hearts 그리드에 하트 배경 추가
    {
        GameObject heartBg = Instantiate(heartBgPrefab, heartsPos);
        heartBgs.Add(heartBg);
    }

    private void SubMaxHp() // 최대 HP 감소 -> hearts 그리드에서 하트 배경 삭제
    {
        if (heartBgs.Count > 0)
        {
            GameObject heartBg = heartBgs[heartBgs.Count - 1];
            Destroy(heartBg);
            heartBgs.RemoveAt(heartBgs.Count - 1);
        }
    }

    private void UpdateHp(int curHp) // 주어진 HP만큼 하트 다시 그리기
    {
        foreach (GameObject heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        for (int i = 0; i < curHp; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsPos);
            hearts.Add(heart);
            hearts[i] = heart;
        }
    }
}
