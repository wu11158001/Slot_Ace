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
        foreach (var resultList in mainPack.SlotResultListPack.SlotResultList)
        {
            List<int> list = new();
            foreach (var slotResult in resultList.SlotResult)
            {
                list.Add(slotResult);
            }

            slotResoultList.Add(list);
        }

        _gameMVC.game_View.StartSlot(slotResoultList);
    }
}
