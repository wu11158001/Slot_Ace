using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotAceProtobuf;

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

        foreach (var resultList in mainPack.SlotResultListPack.SlotResultList)
        {
            // 盤面結果
            List<int> list = new();
            foreach (var slotResult in resultList.SlotResult)
            {
                list.Add(slotResult);
            }
            slotResoultList.Add(list);

            // 黃金牌資料
            foreach (var goldData in resultList.GoldCardDataList)
            {
                GoldCardData goldCardData = new();
                goldCardData.CardIndexList = new();
                goldCardData.CardTypeLiist = new();
                for (int i = 0; i < goldData.CardIndexList.Count; i++)
                {
                    goldCardData.CardIndexList.Add(goldData.CardIndexList[i]);
                    goldCardData.CardTypeLiist.Add(goldData.GoldCardTypeList[i]);
                }
                goldCardDataList.Add(goldCardData);
            }            
        }

        // 輪轉結果
        SlotResultData slotResultData = new()
        {
            SlotCardNumList = slotResoultList,
            GoldCardDataList = goldCardDataList,
        };

        _gameMVC.game_View.StartSlot(slotResultData);
    }
}
