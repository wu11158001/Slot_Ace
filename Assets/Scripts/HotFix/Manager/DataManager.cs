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
    // 黃金牌資料
    public List<GoldCardData> GoldCardDataList;
    // 中獎牌位置
    public List<List<int>> WinCardPosList;
}

/// <summary>
/// 黃金牌資料
/// </summary>
public class GoldCardData
{
    // 位置
    public List<int> CardIndexList;
    // 類型(0=小鬼牌, 1=大鬼牌)
    public List<int> CardTypeLiist;
}

public class DataManager : UnitySingleton<DataManager>
{
    // 用戶ID
    public string UserId { get; set; }
    // 頭像URL
    public string UserImgUrl { get; set; }
}
