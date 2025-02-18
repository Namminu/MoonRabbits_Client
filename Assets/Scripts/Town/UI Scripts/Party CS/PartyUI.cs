using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google.Protobuf.Protocol;
using System.Collections.Generic;

public class PartyUI : MonoBehaviour
{
  private static PartyUI _instance;
  public static PartyUI Instance => _instance;
  public GameObject partyWindow;  // 파티 창
  public GameObject noPartyPanel; // "참가 중인 파티가 없습니다" 패널
  public GameObject inPartyPanel; // "현재 참가 중인 파티" 패널

  public Button createPartyButton; // 파티 생성 버튼
  public Button joinPartyButton;   // ID로 파티 참가 버튼
  public Button closeButton;       // 닫기 버튼

  public TMP_Text memberNickname; // 파티원 닉네임 텍스트
  public TMP_Text memberLevel;    // 파티원 레벨 텍스트

  public Transform partyMemberContainer; // 멤버 카드가 추가될 부모 오브젝트
  public GameObject memberCardPrefab; // MemberCard 프리팹

  private List<GameObject> partyMembers = new List<GameObject>(); // 생성된 파티원 리스트
  private bool isInParty = false; // 파티 참가 여부

  void Awake()
  {
    if (_instance == null)
    {
      _instance = this;
    }
    else
    {
      Destroy(gameObject);
      return;
    }
  }

  void Start()
  {
    // 초기 UI 상태 설정
    UpdatePartyUI();

    // 버튼 클릭 이벤트 연결
    createPartyButton.onClick.AddListener(OnCreatePartyClicked);
    joinPartyButton.onClick.AddListener(OnJoinPartyClicked);
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
      ClearPartyMembers(); // 파티 탈퇴 시 기존 멤버 카드 제거
    }
  }

  // 파티 생성 버튼 클릭 시 실행
  void OnCreatePartyClicked()
  {
    Debug.Log("파티 생성 버튼 클릭됨");

    SendCreatePartyPacket();
    isInParty = true;
    UpdatePartyUI();
  }

  // ID로 파티 참가 버튼 클릭 시 실행
  void OnJoinPartyClicked()
  {
    Debug.Log("ID로 파티 참가 버튼 클릭됨");
    isInParty = true;
    UpdatePartyUI();
    AddPartyMember("테스트 닉네임", 10);
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

  private void SendCreatePartyPacket()
  {
    var createPartyPacket = new C2SCreateParty { };
    GameManager.Network.Send(createPartyPacket);
  }

  // 파티원 추가 함수
  public void AddPartyMember(string nickname, int level)
  {
    if (memberCardPrefab == null || partyMemberContainer == null)
    {
      Debug.LogError("MemberCard 프리팹 또는 Parent가 설정되지 않았습니다!");
      return;
    }

    // 새로운 멤버 카드 생성
    GameObject newMemberCard = Instantiate(memberCardPrefab, partyMemberContainer);
    partyMembers.Add(newMemberCard);

    // MemberCard 내의 UI 요소 설정
    TMP_Text nameText = newMemberCard.transform.Find("MemberNickname").GetComponent<TMP_Text>();
    TMP_Text levelText = newMemberCard.transform.Find("MemberLv").GetComponent<TMP_Text>();
    Button leaveButton = newMemberCard.transform.Find("PartyLeaveBtn").GetComponent<Button>();

    if (nameText != null) nameText.text = nickname;
    if (levelText != null) levelText.text = level + " Lv.";
    if (leaveButton != null) leaveButton.onClick.AddListener(OnLeavePartyClicked);
  }

  // 파티원 제거 함수
  void RemovePartyMember(GameObject memberCard)
  {
    if (partyMembers.Contains(memberCard))
    {
      partyMembers.Remove(memberCard);
      Destroy(memberCard);
    }

    // 파티원이 모두 나가면 파티 종료
    if (partyMembers.Count == 0)
    {
      isInParty = false;
      UpdatePartyUI();
    }
  }

  // 기존 파티원 UI 초기화
  void ClearPartyMembers()
  {
    foreach (GameObject member in partyMembers)
    {
      Destroy(member);
    }
    partyMembers.Clear();
  }
}

