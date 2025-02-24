using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
  public static PartyMemberUI instance { get; private set; }

  public Transform memberContainer; // MemberInfo를 담을 부모 오브젝트 (BG)
  public GameObject memberPrefab;  // MemberInfo 프리팹
  private List<GameObject> memberUIs = new List<GameObject>();

  private void Awake()
  {
    instance = this;
  }

  public void UpdateUI()
  {
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
      // if (member.IsMine)
      //   return;
      Debug.Log("멤버 UI 생성");

      GameObject newMember = Instantiate(memberPrefab, memberContainer);
      newMember.transform.Find("Nickname").GetComponent<TMP_Text>().text = member.Nickname;

      // 생성된 멤버 UI 저장
      memberUIs.Add(newMember);
    }
  }
}
