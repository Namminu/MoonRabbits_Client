using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
  public static PartyMemberUI instance { get; private set; }

  public Transform memberContainer; // MemberInfo를 담을 부모 오브젝트 (BG)
  public GameObject memberPrefab;  // MemberInfo 프리팹
  private List<GameObject> memberUIs = new List<GameObject>();

  public bool isInParty = false;

  private void Awake()
  {
    instance = this;
  }

  public void UpdateUI()
  {
    if (!isInParty || Party.instance == null) return;

    // 기존 UI 삭제
    foreach (GameObject memberUI in memberUIs)
    {
      Destroy(memberUI);
    }
    memberUIs.Clear();

    // `Party.cs`에서 멤버 리스트 가져오기
    List<MemberCardInfo> members = Party.instance.members;

    // 새로운 멤버 UI 동적 생성
    foreach (var member in members)
    {
      // 자기 자신이면 return
      if (member.IsMine)
        return;

      GameObject newMember = Instantiate(memberPrefab, memberContainer);
      newMember.transform.Find("NicknameText").GetComponent<Text>().text = member.Nickname;

      // 생성된 멤버 UI 저장
      memberUIs.Add(newMember);
    }
  }
}
