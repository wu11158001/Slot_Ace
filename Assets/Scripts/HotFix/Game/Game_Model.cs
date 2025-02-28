using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotAceProtobuf;
using Google.Protobuf.Collections;

public class Game_Model : MonoBehaviour
{
    private GameMVC _gameMVC;

    private void Awake()
    {
        _gameMVC = GetComponent<GameMVC>();
    }

    /// <summary>
    /// 處理輪轉協議
    /// </summary>
    /// <param name="mainPack"></param>
    public void HandleSlotRequest(MainPack mainPack)
    {
        List<List<int>> slotResoultList = new();
        List<GoldCardData> goldCardDataList = new();
        List<List<int>> winCardPosList = new();

        foreach (var resultList in mainPack.SlotResultListPack.SlotResultList)
        {
            // 盤面結果
            RepeatedField<int> resultRepeate = resultList.SlotResult;
            slotResoultList.Add(new(resultRepeate));

            // 黃金牌資料
            foreach (var goldData in resultList.GoldCardDataList)
            {
                GoldCardData goldCardData = new();
                RepeatedField<int> cardIndexRepeate = goldData.CardIndexList;
                RepeatedField<int> cardTypeRepeate = goldData.GoldCardTypeList;
                goldCardData.CardIndexList = new(cardIndexRepeate);
                goldCardData.CardTypeLiist = new(cardTypeRepeate);
                goldCardDataList.Add(goldCardData);
            }

            // 中獎牌位置
            RepeatedField<int> winPosRepeate = resultList.WinIndexList;
            winCardPosList.Add(new(winPosRepeate));
        }

        // 輪轉結果
        SlotResultData slotResultData = new()
        {
            SlotCardNumList = slotResoultList,
            GoldCardDataList = goldCardDataList,
            WinCardPosList = winCardPosList,
        };

        _gameMVC.game_View.StartSlot(slotResultData);
    }
}
