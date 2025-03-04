using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Google.Protobuf.Protocol;
using Unity.VisualScripting;
using UnityEngine;
public class Party : MonoBehaviour
{
  public string partyId { get; private set; }
  public int leaderId { get; private set; }
  public int memberCount { get; private set; }
  public List<MemberCardInfo> members { get; private set; } = new List<MemberCardInfo>();

  // 싱글톤 패턴 (필요 시 사용)
  public static Party instance { get; private set; }

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  #region 파티 정보 업데이트
  // 서버에서 받은 데이터로 파티 정보 업데이트
  public void CreatePartyData(S2CCreateParty partyData)
  {
    partyId = partyData.PartyId;
    leaderId = partyData.LeaderId;
    memberCount = partyData.MemberCount;

    // 기존 멤버 리스트 초기화 후 새 데이터로 채우기
    members.Clear();
    foreach (var member in partyData.Members)
    {
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, CurrentSector = member.CurrentSector, IsMine = member.IsMine });
    }

    // UI 업데이트 요청 (PartyUI에서 처리)
    PartyUI.instance.isInParty = true;
    PartyUI.instance.UpdateUI();
  }

  public void AllowInviteData(S2CAllowInvite partyData)
  {
    partyId = partyData.PartyId;
    leaderId = partyData.LeaderId;
    memberCount = partyData.MemberCount;

    // 기존 멤버 리스트 초기화 후 새 데이터로 채우기
    members.Clear();
    foreach (var member in partyData.Members)
    {
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, CurrentSector = member.CurrentSector, IsMine = member.IsMine });
    }

    // UI 업데이트 요청 (PartyUI에서 처리)
    PartyUI.instance.isInParty = true;
    PartyUI.instance.UpdateUI();
  }

  public void LeavePartyData(S2CLeaveParty partyData)
  {
    partyId = partyData.PartyId;
    leaderId = partyData.LeaderId;
    memberCount = partyData.MemberCount;

    // 기존 멤버 리스트 초기화 후 새 데이터로 채우기
    members.Clear();
    foreach (var member in partyData.Members)
    {
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, CurrentSector = member.CurrentSector, IsMine = member.IsMine });
    }

    // UI 업데이트 요청 (PartyUI에서 처리)
    PartyUI.instance.isInParty = true;
    PartyUI.instance.UpdateUI();
  }

  public void KickedOutData(S2CKickOutMember partyData)
  {
    partyId = partyData.PartyId;
    leaderId = partyData.LeaderId;
    memberCount = partyData.MemberCount;

    // 기존 멤버 리스트 초기화 후 새 데이터로 채우기
    members.Clear();
    foreach (var member in partyData.Members)
    {
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, CurrentSector = member.CurrentSector, IsMine = member.IsMine });
    }

    // UI 업데이트 요청 (PartyUI에서 처리)
    PartyUI.instance.isInParty = true;
    PartyUI.instance.UpdateUI();
  }

  public void JoinPartyData(S2CJoinParty partyData)
  {
    partyId = partyData.PartyId;
    leaderId = partyData.LeaderId;
    memberCount = partyData.MemberCount;

    // 기존 멤버 리스트 초기화 후 새 데이터로 채우기
    members.Clear();
    foreach (var member in partyData.Members)
    {
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, CurrentSector = member.CurrentSector, IsMine = member.IsMine });
    }

    // UI 업데이트 요청 (PartyUI에서 처리)
    PartyUI.instance.isInParty = true;
    PartyUI.instance.UpdateUI();
  }
  #endregion

  #region 멤버 관리

  public void RemoveMember(int playerId)
  {
    members.RemoveAll(m => m.Id == playerId);
    memberCount = members.Count;

    if (memberCount == 0)
    {
      // 마지막 멤버가 나간 경우, 파티 해산 처리
      members.Clear();
      memberCount = 0;
      partyId = null; // 파티 ID 초기화
      leaderId = -1;  // 리더 ID 초기화
      PartyUI.instance.isInParty = false;
    }
    // UI 업데이트 요청
    PartyUI.instance.UpdateUI();
  }
  public void RemoveAllMembers()
  {
    members.Clear();
    memberCount = 0;

    // UI 업데이트 요청
    PartyUI.instance.isInParty = false;
    PartyUI.instance.UpdateUI();
  }


  public int GetMyPlayerId()
  {
    return GetMyPlayer()?.PlayerId ?? -1;
  }

  public string GetMyPlayerNickname()
  {
    return GetMyPlayer()?.nickname ?? null;
  }

  public MemberCardInfo GetMemberByNickname(string nickname)
  {
    return members.FirstOrDefault(m => m.Nickname == nickname);
  }


  private Player GetMyPlayer()
  {
    return FindObjectsOfType<Player>().FirstOrDefault(p => p.IsMine);
  }
  #endregion
}
