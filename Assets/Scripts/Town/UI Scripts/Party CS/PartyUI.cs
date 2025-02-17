using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyUI : MonoBehaviour
{
  public GameObject partyWindow;  // 파티 창
  public GameObject noPartyPanel; // "참가 중인 파티가 없습니다" 패널
  public GameObject inPartyPanel; // "현재 참가 중인 파티" 패널

  public Button createPartyButton; // 파티 생성 버튼
  public Button joinPartyButton;   // ID로 파티 참가 버튼
  public Button leavePartyButton;  // 파티 탈퇴 버튼
  public Button closeButton;       // 닫기 버튼

  public TMP_Text memberNickname; // 파티원 닉네임 텍스트
  public TMP_Text memberLevel;    // 파티원 레벨 텍스트

  private bool isInParty = false; // 파티 참가 여부

  void Start()
  {
    // 초기 UI 상태 설정
    UpdatePartyUI();

    // 버튼 클릭 이벤트 연결
    createPartyButton.onClick.AddListener(OnCreatePartyClicked);
    joinPartyButton.onClick.AddListener(OnJoinPartyClicked);
    leavePartyButton.onClick.AddListener(OnLeavePartyClicked);
    closeButton.onClick.AddListener(ClosePartyWindow);
  }

  void UpdatePartyUI()
  {
    if (isInParty)
    {
      noPartyPanel.SetActive(false);
      inPartyPanel.SetActive(true);
    }
    else
    {
      noPartyPanel.SetActive(true);
      inPartyPanel.SetActive(false);
    }
  }

  // 파티 생성 버튼 클릭 시 실행
  void OnCreatePartyClicked()
  {
    Debug.Log("파티 생성 버튼 클릭됨");

    

    isInParty = true;
    UpdatePartyUI();
    SetPartyMemberInfo("테스트 닉네임", 10);
  }

  // ID로 파티 참가 버튼 클릭 시 실행
  void OnJoinPartyClicked()
  {
    Debug.Log("ID로 파티 참가 버튼 클릭됨");
    isInParty = true;
    UpdatePartyUI();
    SetPartyMemberInfo("테스트 닉네임", 10);
  }

  // 파티 탈퇴 버튼 클릭 시 실행
  void OnLeavePartyClicked()
  {
    Debug.Log("파티 탈퇴 버튼 클릭됨");
    isInParty = false;
    UpdatePartyUI();
  }

  // 파티 창 닫기
  void ClosePartyWindow()
  {
    partyWindow.SetActive(false);
  }

  // 파티 창 열기
  public void OpenPartyWindow()
  {
    partyWindow.SetActive(true);
    UpdatePartyUI();
  }

  // 파티원 정보를 설정하는 함수
  void SetPartyMemberInfo(string nickname, int level)
  {
    if (memberNickname != null) memberNickname.text = nickname;
    if (memberLevel != null) memberLevel.text = level + " Lv.";
  }
}
