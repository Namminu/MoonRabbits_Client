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

    [SerializeField]
    private GameObject[] hearts;


    public Image hand;
    public Image axe;
    public Image pickaxe;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateUI()
    {

        foreach (GameObject memberUI in memberUIs)
        {
            Destroy(memberUI);
        }
        memberUIs.Clear();

        foreach (Transform memberInfo in memberContainer)
        {
            Destroy(memberInfo.gameObject);
        }


        // `Party.cs`에서 멤버 리스트 가져오기
        List<MemberCardInfo> members = Party.instance.members;

        // 새로운 멤버 UI 동적 생성
        foreach (var member in members)
        {
            Debug.Log($"멤버 UI 생성 : {member}");

            GameObject newMember = Instantiate(memberPrefab, memberContainer);
            newMember.transform.Find("Nickname").GetComponent<TMP_Text>().text = member.Nickname;

            hand = newMember.transform.Find("MemberImage/WorkingOn/Hand").GetComponent<Image>();
            axe = newMember.transform.Find("MemberImage/WorkingOn/Axe").GetComponent<Image>();
            pickaxe = newMember.transform.Find("MemberImage/WorkingOn/Pickaxe").GetComponent<Image>();

            if (!PlayerManager.playerSaveData.ContainsKey(member.Id))
            {
                PlayerManager.playerSaveData[member.Id] = new SavePlayerData();
            }

            var player = PlayerManager.playerSaveData[member.Id];

            // 레벨 업데이트
            newMember.transform.Find("Level/LevelText").GetComponent<TMP_Text>().text = $"{player.Level}";

            // 체력 업데이트
            // 하트 찾아서 배열에 넣어주기
            hearts = new GameObject[3];

            hearts[0] = newMember.transform.Find("Heart1").gameObject;
            hearts[1] = newMember.transform.Find("Heart2").gameObject;
            hearts[2] = newMember.transform.Find("Heart3").gameObject;

            foreach (var heart in hearts)
            {
                heart.SetActive(false);
            }

            for (int i = 0; i < player.CurHp; i++)
            {
                hearts[i].SetActive(true);
            }

            // 사용 중인 도구 업데이트
            if (player.CurrentEquip == 1)
            {
                hand.gameObject.SetActive(false);
                axe.gameObject.SetActive(true);
                pickaxe.gameObject.SetActive(false);
            }
            else if (player.CurrentEquip == 2)
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

            // 생성된 멤버 UI 저장
            memberUIs.Add(newMember);
        }
    }
}
