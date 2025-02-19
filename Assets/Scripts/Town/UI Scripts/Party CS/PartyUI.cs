using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{
  public static PartyUI instance { get; private set; }

  public GameObject partyWindow;
  public GameObject noPartyPanel;
  public GameObject inPartyPanel;

  // 팝업창
  public GameObject invitePartyPopUp;
  public GameObject allowInvitePopUp;

  public Transform partyMemberContainer;
  public GameObject memberCardPrefab;
  public ScrollRect scrollRect;

  public Button createPartyButton; // 파티 생성 버튼
  public Button invitePartyButton; // 파티 생성 버튼

  public Button joinPartyButton;   // ID로 파티 참가 버튼
  public Button closeButton;       // 닫기 버튼

  public bool isInParty = false; // 파티 참가 여부
  private int memberId = 100;

  private void Awake()
  {
    instance = this;
  }

  void Start()
  {
    // 초기 UI 상태 설정
    UpdateUI();

    // 버튼 클릭 이벤트 연결
    createPartyButton.onClick.AddListener(OnCreatePartyClicked);
    // joinPartyButton.onClick.AddListener(OnJoinPartyClicked);
    closeButton.onClick.AddListener(ClosePartyWindow);
  }

  // Party 인스턴스에서 데이터를 가져와 UI 업데이트
  public void UpdateUI()
  {
    // 현재 파티 정보 가져오기
    Party party = Party.instance;

    if (isInParty)
    {
      noPartyPanel.SetActive(false);
      inPartyPanel.SetActive(true);
      Button invitePartyButton = inPartyPanel.transform.Find("InvitePartyBtn").GetComponent<Button>();
      invitePartyButton.onClick.RemoveAllListeners();
      invitePartyButton.onClick.AddListener(OnInvitePartyClicked);

      // 기존 멤버 카드 삭제
      ClearPartyMembers();

      // 새로운 멤버 추가
      foreach (var member in party.members)
      {
        CreateMemberCard(member.Id, member.Nickname, member.Id == party.leaderId, member.IsMine);
      }
    }
    else
    {
      noPartyPanel.SetActive(true);
      inPartyPanel.SetActive(false);
      ClearPartyMembers();
    }

    // UpdateScrollView();
  }

  #region 상호작용 이벤트 함수
  // 파티 생성 버튼 클릭 시 실행
  void OnCreatePartyClicked()
  {
    Debug.Log("파티 생성 버튼 클릭됨");
    SendCreatePartyPacket();
    UpdateUI();
  }

  // ID로 파티 참가 버튼 클릭 시 실행
  // void OnJoinPartyClicked()
  // {
  //   Debug.Log("ID로 파티 참가 버튼 클릭됨");
  //   isInParty = true;

  //   var memberData = new S2CCreateParty
  //   {
  //     PartyId = 1234,
  //     LeaderId = 0, // 파티장이 없어짐
  //     MemberCount = 0,
  //   };
  //   Party.instance.SetPartyData(memberData);

  //   CreateMemberCard(100, "테스트", false, false);
  //   UpdateUI();
  // }

  // 파티 초대 버튼 클릭 시 실행
  // void OnInvitePartyClicked()
  // {
  //   Debug.Log("파티 초대 버튼 클릭됨");
  //   // CreateMemberCard(memberId++, "테스트", true, false);

  //   var memberData = new MemberCardInfo
  //   {
  //     Id = memberId++,
  //     Nickname = "테스트",
  //     IsPartyLeader = false,
  //     IsMine = false
  //   };
  //   Party.instance.InvitePartyData(memberData);
  // }

  // 파티 초대 버튼 클릭 시 실행
  void OnInvitePartyClicked()
  {
    Debug.Log("파티 초대 버튼 클릭됨");
    invitePartyPopUp.SetActive(true);
    Button submitBtn = invitePartyPopUp.transform.Find("SubmitBtn").GetComponent<Button>();

    submitBtn.onClick.AddListener(OnSubmitClicked);
  }

  void OnSubmitClicked()
  {
    TMP_InputField inviteNickname = invitePartyPopUp.transform.Find("InviteNickname").GetComponent<TMP_InputField>();
    string nickname = inviteNickname.text;

    SendInvitePartyPacket(nickname);
  }

  public void CloseInvitePopUp()
  {
    invitePartyPopUp.SetActive(false);
    UpdateUI();
  }

  public void OpenAllowInvitePopUp(string partyId, string leaderNickname, int memberId)
  {
    allowInvitePopUp.SetActive(true);

    TMP_Text allowText = allowInvitePopUp.transform.Find("RawImage/AllowText").GetComponent<TMP_Text>();
    allowText.text = $"{leaderNickname}님이 초대했습니다.";

    Button allowBtn = allowInvitePopUp.transform.Find("AllowBtn").GetComponent<Button>();

    allowBtn.onClick.RemoveAllListeners();
    allowBtn.onClick.AddListener(() => SendAllowInvitePacket(partyId, memberId));
  }

  // 파티 창 닫기
  void ClosePartyWindow()
  {
    partyWindow.SetActive(false);
  }

  // 파티 탈퇴 버튼 클릭 시
  public void OnLeavePartyClicked()
  {
    // 새로운 PartyData를 생성하여 members를 개별적으로 추가
    S2CCreateParty newPartyData = new S2CCreateParty
    {
      PartyId = Party.instance.partyId,
      LeaderId = 0, // 파티장이 없어짐
      MemberCount = 0
    };

    // SetPartyData 호출 (빈 멤버 리스트 적용)
    Party.instance.CreatePartyData(newPartyData);

    isInParty = false;
    UpdateUI();
  }
  #endregion

  #region 멤버 카드 생성
  private void CreateMemberCard(int playerId, string nickname, bool isLeader, bool isMine)
  {
    if (memberCardPrefab == null || partyMemberContainer == null)
    {
      Debug.LogError("MemberCard 프리팹 또는 Parent가 설정되지 않았습니다!");
      return;
    }

    isInParty = true;

    // 새로운 멤버 카드 생성
    GameObject newMemberCard = Instantiate(memberCardPrefab, partyMemberContainer);

    // MemberCard 내의 UI 요소 가져오기
    Image leaderIcon = newMemberCard.transform.Find("LeaderIcon").GetComponent<Image>();
    TMP_Text nameText = newMemberCard.transform.Find("MemberNickname").GetComponent<TMP_Text>();
    Button leaveButton = newMemberCard.transform.Find("PartyLeaveBtn").GetComponent<Button>();
    Button kickOutButton = newMemberCard.transform.Find("KickOutBtn").GetComponent<Button>();

    // 닉네임 설정
    if (nameText != null) nameText.text = nickname;

    // **리더 아이콘 설정**
    if (leaderIcon != null)
    {
      leaderIcon.gameObject.SetActive(isLeader);
    }

    // **내 카드일 경우 탈퇴 버튼 활성화**
    if (isMine)
    {
      if (leaveButton != null)
      {
        leaveButton.gameObject.SetActive(true);
        leaveButton.onClick.AddListener(OnLeavePartyClicked);
      }
      if (kickOutButton != null) kickOutButton.gameObject.SetActive(false);
    }
    else
    {
      if (leaveButton != null) leaveButton.gameObject.SetActive(false);
      // 내 카드가 아닌 경우
      if (Party.instance.leaderId == Party.instance.GetMyPlayerId())
      {
        // 내가 파티장인 경우: 강퇴 버튼 활성화
        if (kickOutButton != null)
        {
          kickOutButton.gameObject.SetActive(true);
          kickOutButton.onClick.RemoveAllListeners();
          kickOutButton.onClick.AddListener(delegate { RemovePartyMember(newMemberCard, playerId); });
          kickOutButton.onClick.AddListener(SendKickOutPartyPacket);
        }
      }
      else
      {
        // 내가 파티장이 아닌 경우
        if (kickOutButton != null) kickOutButton.gameObject.SetActive(false);
      }
    }
  }
  #endregion

  // 기존 멤버 삭제
  private void ClearPartyMembers()
  {
    foreach (Transform child in partyMemberContainer)
    {
      Destroy(child.gameObject);
    }
  }




  // 파티원 강퇴
  private void RemovePartyMember(GameObject memberCard, int playerId)
  {
    Party.instance.RemoveMember(playerId);

    // UI에서 멤버 카드 제거
    Destroy(memberCard);

    // UI 업데이트
    UpdateUI();
  }

  void UpdateScrollView()
  {
    // 스크롤을 맨 아래로 이동 (새로운 멤버가 추가될 때 자동 스크롤)
    Canvas.ForceUpdateCanvases();
    scrollRect.verticalNormalizedPosition = 0f;
  }

  #region 패킷 전송 함수
  private void SendCreatePartyPacket()
  {
    var createPartyPacket = new C2SCreateParty { };
    GameManager.Network.Send(createPartyPacket);
  }

  private void SendAllowInvitePacket(string partyId, int memberId)
  {
    var allowInvitePacket = new C2SAllowInvite { PartyId = partyId, MemberId = memberId };
    GameManager.Network.Send(allowInvitePacket);
    allowInvitePopUp.SetActive(false);
  }

  private void SendInvitePartyPacket(string nickname)
  {
    var invitePartyPacket = new C2SInviteParty { PartyId = Party.instance.partyId, Nickname = nickname };
    GameManager.Network.Send(invitePartyPacket);
    invitePartyPopUp.SetActive(false);
  }

  private void SendKickOutPartyPacket()
  {

  }
  #endregion
}
