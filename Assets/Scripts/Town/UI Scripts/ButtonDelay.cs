using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonDelay : MonoBehaviour
{
    public Button targetButton1;
    public Button targetButton2;
    public Button targetButton4;
    public Button targetButton5;
    public Button targetButton6;
    public float delayTime = 1.5f; // 딜레이 시간(초)

    void Start()
    {
        targetButton1.onClick.AddListener(OnButtonClickSort);
        targetButton2.onClick.AddListener(OnButtonClickParty1);
        targetButton4.onClick.AddListener(OnButtonClickRanking1);
        targetButton5.onClick.AddListener(OnButtonClickRanking2);
        targetButton6.onClick.AddListener(OnButtonClickRanking3);

    }

    void OnButtonClickSort()
    {
        // 추가 작업 처리 가능
        StartCoroutine(DisableButtonCoroutineSort());
    }

    void OnButtonClickParty1()
    {
        // 추가 작업 처리 가능
        StartCoroutine(DisableButtonCoroutineParty1());
    }


    void OnButtonClickRanking1()
    {
        // 추가 작업 처리 가능
        StartCoroutine(DisableButtonCoroutineRanking1());
    }

    void OnButtonClickRanking2()
    {
        // 추가 작업 처리 가능
        StartCoroutine(DisableButtonCoroutineRanking2());
    }

    void OnButtonClickRanking3()
    {
        // 추가 작업 처리 가능
        StartCoroutine(DisableButtonCoroutineRanking3());
    }

    private IEnumerator DisableButtonCoroutineSort()
    {
        // 버튼 비활성화
        targetButton1.interactable = false;
        // 설정한 시간만큼 대기 (게임 시간 기준)
        yield return new WaitForSeconds(delayTime);
        // 작업 완료 후 버튼 다시 활성화
        targetButton1.interactable = true;
    }

    private IEnumerator DisableButtonCoroutineParty1()
    {
        // 버튼 비활성화
        targetButton2.interactable = false;
        // 설정한 시간만큼 대기 (게임 시간 기준)
        yield return new WaitForSeconds(delayTime);
        // 작업 완료 후 버튼 다시 활성화
        targetButton2.interactable = true;
    }

    private IEnumerator DisableButtonCoroutineRanking1()
    {
        // 버튼 비활성화
        targetButton4.interactable = false;
        // 설정한 시간만큼 대기 (게임 시간 기준)
        yield return new WaitForSeconds(delayTime);
        // 작업 완료 후 버튼 다시 활성화
        targetButton4.interactable = true;
    }

    private IEnumerator DisableButtonCoroutineRanking2()
    {
        // 버튼 비활성화
        targetButton5.interactable = false;
        // 설정한 시간만큼 대기 (게임 시간 기준)
        yield return new WaitForSeconds(delayTime);
        // 작업 완료 후 버튼 다시 활성화
        targetButton5.interactable = true;
    }

    private IEnumerator DisableButtonCoroutineRanking3()
    {
        // 버튼 비활성화
        targetButton6.interactable = false;
        // 설정한 시간만큼 대기 (게임 시간 기준)
        yield return new WaitForSeconds(delayTime);
        // 작업 완료 후 버튼 다시 활성화
        targetButton6.interactable = true;
    }


}