using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 輪轉結果
/// </summary>
public class SlotResultData
{
    // 盤面結果
    public List<List<int>> SpinCardNumList;
    // 黃金牌位置
    public List<List<int>> GoldCardIndexList;
    // 中獎牌位置
    public List<List<int>> WinCardPosList;
    // 大鬼牌資料
    public List<List<BigWildData>> BigWildDataList;
    // 每輪贏分
    public List<int> WinValueList;
    // 用戶訊息資料
    public UserInfoData userInfoData;
}

/// <summary>
/// 用戶訊息資料
/// </summary>
public class UserInfoData
{
    // 用戶籌碼
    public int UserCoin;
    // 免費輪轉次數
    public int FreeSpin;
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

/// <summary>
/// 大獎資料
/// </summary>
public class BigWinData
{
    // 大獎名稱
    public string TypeString;
    // 中獎所需倍數
    public int Multiple;
}

public static class DataManager
{
    // 用戶ID
    public static string UserId { get; set; }
    // 頭像URL
    public static string UserImgUrl { get; set; }

    // 大獎資料
    public static List<BigWinData> BigWinDataList = new()
    {
        // Mega Win
        new() { TypeString = "Mega\nWin", Multiple = 30 },
        // Super Win
        new() { TypeString = "Super\nWin", Multiple = 20 },
        // Big Win
        new() { TypeString = "Big\nWin", Multiple = 10 },
    };
}
