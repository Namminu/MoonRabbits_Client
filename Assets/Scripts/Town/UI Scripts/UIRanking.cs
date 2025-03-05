using Google.Protobuf.Protocol;
using UnityEngine;

public class UIRanking : MonoBehaviour
{
    // public Transform rankingListParent;
    // public GameObject rankingItemPrefab;

    // 서버에서 랭킹 패킷을 받았을 때 호출되는 콜백 함수
    public void UpdateRanking(S2CUpdateRanking rankData)
    {
        if (rankData.Status == "success")
        {
            // 랭킹 정보를 UI에 반영합니다.
            foreach (var playerRank in rankData.Data.RankingList_)
            {
                UpdateRankUI(playerRank.Rank, playerRank.Nickname, playerRank.Exp);
            }
            // 필요 시 타임스탬프 등 다른 정보도 업데이트
            // e.g., timestampText.text = rankData.Data.Timestamp;
        }
        else
        {
            Debug.LogError("랭킹 데이터를 가져오는 데 실패했습니다.");
        }
    }

    // 개별 랭킹 항목을 UI에 업데이트하는 함수 (UI용 로직 직접 구현)
    private void UpdateRankUI(int rank, string nickname, int exp)
    {
        // 예시: Prefab을 인스턴스화하여 자식으로 추가하고 텍스트 컴포넌트를 업데이트
        // GameObject item = Instantiate(rankingItemPrefab, rankingListParent);
        // item.transform.Find("RankText").GetComponent<Text>().text = rank.ToString();
        // item.transform.Find("NicknameText").GetComponent<Text>().text = nickname;
        // item.transform.Find("ExpText").GetComponent<Text>().text = exp.ToString();
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
