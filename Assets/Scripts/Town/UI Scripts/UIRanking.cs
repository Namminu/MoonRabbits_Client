using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class UIRanking : MonoBehaviour
{
    public string status { get; private set; }
    public string type { get; private set; }

    // void updateRanking(S2CUpdateRanking rankData)
    // {
    //     status = rankData.Status;
        

    //     if (rankData.status == "success")
    //     {
    //         foreach (var rank in rankData.data.rankingList)
    //         {
    //             // Unity UI 요소 업데이트 (예: RankSheet에 데이터 추가)
    //             UpdateRankUI(rank.rank, rank.nickname, rank.exp);
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("랭킹 데이터를 가져오는 데 실패했습니다.");
    //     }
    // }

    void UpdateRankUI(int rank, string nickname, int exp)
    {
        // RankSheet의 UI 요소 업데이트 로직 구현
    }
    #region 패킷 전송 함수
    void SendRankingListPacket(string type) // type은 "ALL" 또는 "TOP"
    {
        var requestRankingList = new C2SRankingList { Type = type };
        GameManager.Network.Send(requestRankingList);
    } 

    #endregion
}
