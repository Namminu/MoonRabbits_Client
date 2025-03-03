using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    public static PartyMemberUI instance { get; private set; }

    public Transform memberContainer; // MemberInfo를 담을 부모 오브젝트 (BG)
    public GameObject memberPrefab; // MemberInfo 프리팹
    private List<GameObject> memberUIs = new List<GameObject>();

    public Image hand;
    public Image axe;
    public Image pickaxe;


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

            hand = newMember.transform.Find("MemberImage/WorkingOn/Hand").GetComponent<Image>();
            axe = newMember.transform.Find("MemberImage/WorkingOn/Axe").GetComponent<Image>();
            pickaxe = newMember.transform.Find("MemberImage/WorkingOn/Pickaxe").GetComponent<Image>();

            switch (member.CurrentSector)
            {
                case 100:
                    newMember.transform.Find("Level/LevelText").GetComponent<TMP_Text>().text = $"{TownManager.Instance.GetPlayer(member.Id).level}";
                    Player townPlayer = TownManager.Instance.GetPlayer(member.Id);

                    if (townPlayer.ActiveEquipObj == townPlayer.axe)
                    {
                        hand.gameObject.SetActive(false);
                        axe.gameObject.SetActive(true);
                        pickaxe.gameObject.SetActive(false);
                    }
                    else if (townPlayer.ActiveEquipObj == townPlayer.pickAxe)
                    {
                        hand.gameObject.SetActive(false);
                        axe.gameObject.SetActive(false);
                        pickaxe.gameObject.SetActive(true);
                    }
                    else
                    {
                        hand.gameObject.SetActive(true);
                        axe.gameObject.SetActive(false);
                        pickaxe.gameObject.SetActive(false);
                    }

                    break;
                case 101:
                    newMember.transform.Find("Level/LevelText").GetComponent<TMP_Text>().text = $"{S1Manager.Instance.GetPlayer(member.Id).level}";
                    Player S1Player = S1Manager.Instance.GetPlayer(member.Id);
                    if (S1Player.ActiveEquipObj == S1Player.axe)
                    {
                        hand.gameObject.SetActive(false);
                        axe.gameObject.SetActive(true);
                        pickaxe.gameObject.SetActive(false);
                    }
                    else if (S1Player.ActiveEquipObj == S1Player.pickAxe)
                    {
                        hand.gameObject.SetActive(false);
                        axe.gameObject.SetActive(false);
                        pickaxe.gameObject.SetActive(true);
                    }
                    else
                    {
                        hand.gameObject.SetActive(true);
                        axe.gameObject.SetActive(false);
                        pickaxe.gameObject.SetActive(false);
                    }
                    break;
                case 102:
                    newMember.transform.Find("Level/LevelText").GetComponent<TMP_Text>().text = $"{S2Manager.Instance.GetPlayer(member.Id).level}";
                    Player S2Player = S2Manager.Instance.GetPlayer(member.Id);
                    if (S2Player.ActiveEquipObj == S2Player.axe)
                    {
                        hand.gameObject.SetActive(false);
                        axe.gameObject.SetActive(true);
                        pickaxe.gameObject.SetActive(false);
                    }
                    else if (S2Player.ActiveEquipObj == S2Player.pickAxe)
                    {
                        hand.gameObject.SetActive(false);
                        axe.gameObject.SetActive(false);
                        pickaxe.gameObject.SetActive(true);
                    }
                    else
                    {
                        hand.gameObject.SetActive(true);
                        axe.gameObject.SetActive(false);
                        pickaxe.gameObject.SetActive(false);
                    }
                    break;
            }



            // 생성된 멤버 UI 저장
            memberUIs.Add(newMember);
        }
    }
}
