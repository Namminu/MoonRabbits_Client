using System.Collections.Generic;
using System.Data.Common;
using Google.Protobuf.Protocol;
using UnityEngine;
public class Party : MonoBehaviour
{
  public int partyId { get; private set; }
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
  public void SetPartyData(S2CCreateParty partyData)
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

  public void RemoveMember(int playerId)
  {
    members.RemoveAll(m => m.Id == playerId);
    memberCount = members.Count;
  }

}
