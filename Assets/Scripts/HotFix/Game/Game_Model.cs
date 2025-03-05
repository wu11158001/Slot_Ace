using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotAceProtobuf;
using Google.Protobuf.Collections;

public class Game_Model : MonoBehaviour
{
    private GameMVC _gameMVC;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="gameMVC"></param>
    public void Initialize(GameMVC gameMVC)
    {
        _gameMVC = gameMVC;
    }

    /// <summary>
    /// 處理輪轉協議
    /// </summary>
    /// <param name="mainPack"></param>
    public void HandleSlotRequest(MainPack mainPack)
    {
        List<List<int>> slotResoultList = new();
        List<List<int>> goldCardDataList = new();
        List<List<int>> winCardPosList = new();
        List<List<BigWildData>> bigWildDataList = new();

        foreach (var resultList in mainPack.SlotResultListPack.SlotResultList)
        {
            // 盤面結果
            slotResoultList.Add(new(resultList.SlotResult));

            // 黃金牌資料
            goldCardDataList.Add(new(resultList.GoldIndexList));

            // 中獎牌位置
            winCardPosList.Add(new(resultList.WinIndexList));

            // 大鬼牌資料
            if (resultList.BigWildDataPackList != null && resultList.BigWildDataPackList.Count > 0)
            {
                List<BigWildData> bigWildList = new();
                for (int i = 0; i < 20; i++)
                {
                    bigWildList.Add(new BigWildData());
                }

                foreach (var bigWild in resultList.BigWildDataPackList)
                {
                    BigWildData bigWildData = new()
                    {
                        MainIndex = bigWild.MainIndex,
                        CopyIndexList = new(bigWild.CopyIndexList),
                    };
                    bigWildList[bigWild.MainIndex] = bigWildData;
                }
                bigWildDataList.Add(bigWildList);
            }
            else
            {
                List<BigWildData> tempBigWildData = new();
                bigWildDataList.Add(tempBigWildData);
            }
        }

        // 輪轉結果
        SlotResultData slotResultData = new()
        {
            SlotCardNumList = slotResoultList,
            GoldCardIndexList = goldCardDataList,
            WinCardPosList = winCardPosList,
            bigWildDataList = bigWildDataList,
        };

        _gameMVC.game_View.StartSlot(slotResultData);
    }
}
