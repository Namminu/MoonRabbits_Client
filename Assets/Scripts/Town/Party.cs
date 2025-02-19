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

  public Party()
  {
    instance = this;
  }

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
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, IsMine = member.IsMine });
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
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, IsMine = member.IsMine });
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
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, IsMine = member.IsMine });
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
      members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, IsMine = member.IsMine });
    }

    // UI 업데이트 요청 (PartyUI에서 처리)
    PartyUI.instance.isInParty = true;
    PartyUI.instance.UpdateUI();
  }

  // public void InvitePartyData(S2CInviteParty partyData)
  // {
  //   partyId = partyData.PartyId;
  //   leaderId = partyData.LeaderId;
  //   memberCount = partyData.MemberCount;

  //   // 기존 멤버 리스트 초기화 후 새 데이터로 채우기
  //   members.Clear();
  //   foreach (var member in partyData.Members)
  //   {
  //     members.Add(new MemberCardInfo { Id = member.Id, Nickname = member.Nickname, IsMine = member.IsMine });
  //   }

  //   // UI 업데이트 요청 (PartyUI에서 처리)
  //   PartyUI.instance.CloseInvitePopUp();
  // }

  public void RemoveMember(int playerId)
  {
    members.RemoveAll(m => m.Id == playerId);
    memberCount = members.Count;
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
}
