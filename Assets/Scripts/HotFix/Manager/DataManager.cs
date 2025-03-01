using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 輪轉結果
/// </summary>
public class SlotResultData
{
    // 盤面結果
    public List<List<int>> SlotCardNumList;
    // 黃金牌位置
    public List<List<int>> GoldCardIndexList;
    // 中獎牌位置
    public List<List<int>> WinCardPosList;
    // 大鬼牌資料
    public List<BigWildData> bigWildDataList;
}

/// <summary>
/// 大鬼牌資料
/// </summary>
public class BigWildData
{
    // 主卡位置
    public int MainIndex;
    // 複製位置
    public List<int> CopyIndexList;
    // 複製撲克
    public List<Poker> CopyPokerList;
}


public class DataManager : UnitySingleton<DataManager>
{
    // 用戶ID
    public string UserId { get; set; }
    // 頭像URL
    public string UserImgUrl { get; set; }
}
