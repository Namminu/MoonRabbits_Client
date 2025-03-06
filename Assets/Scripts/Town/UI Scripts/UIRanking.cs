using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRanking : MonoBehaviour
{
    // UI 요소 연결
    public Transform rankSheet; // Content Area > RankSheet
    public GameObject rankItemPrefab; // RankSheet의 개별 랭킹 항목 프리팹
    public Button allButton; // Down Area > ALL 버튼
    public Button top10Button; // Down Area > TOP10 버튼

    private void Start()
    {
        // 버튼 클릭 이벤트 등록
        allButton.onClick.AddListener(() => SendRankingListPacket("ALL"));
        top10Button.onClick.AddListener(() => SendRankingListPacket("TOP"));
    }

    // 서버에서 받은 랭킹 데이터를 처리하고 UI를 업데이트
    public void UpdateRanking(S2CUpdateRanking rankData)
    {
        if (rankData.Status == "success")
        {
            // 기존 UI 초기화
            foreach (Transform child in rankSheet)
            {
                Destroy(child.gameObject);
            }

            // 새로운 데이터를 기반으로 UI 생성
            foreach (var playerRank in rankData.Data.RankingList_)
            {
                GameObject item = Instantiate(rankItemPrefab, rankSheet);
                item.transform.Find("RankingNumber").GetComponentInChildren<TextMeshProUGUI>().text = playerRank.Rank.ToString();
                item.transform.Find("NickName").GetComponentInChildren<TextMeshProUGUI>().text = playerRank.Nickname;
                item.transform.Find("Exp").GetComponentInChildren<TextMeshProUGUI>().text = playerRank.Exp.ToString();
            }
        }
        else
        {
            Debug.LogError("랭킹 데이터를 가져오는 데 실패했습니다.");
        }
    }

    #region 패킷 전송 함수

    // 랭킹 요청 패킷 전송 (type은 "ALL" 또는 "TOP")
    public void SendRankingListPacket(string type)
    {
        // 멤버 변수와 혼동되지 않도록 RequestType에 값 저장
        var requestRankingList = new C2SRankingList { Type = type };
        GameManager.Network.Send(requestRankingList);
    }

    #endregion
}
