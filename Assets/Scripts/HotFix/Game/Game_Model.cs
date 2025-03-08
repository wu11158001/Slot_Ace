using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotAceProtobuf;
using Google.Protobuf.Collections;

public class Game_Model : MonoBehaviour
{
    private GameMVC _gameMVC;

    // 暱稱
    public string Nickname { get; set; }
    // 前個下注值
    public int PreBetValue { get; set; }
    // 紀錄當前總贏分
    public int RecodeTotalWinValue { get; private set; }
    // 紀錄用戶籌碼
    public int RecodeUserCoin { get; private set; }
    // 紀錄免費輪轉
    public int RecodeFreeSpin { get; private set; }
    // 紀錄免費輪轉總贏分
    public int RecodeFreeSpinTotalWinValue { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="gameMVC"></param>
    public void Initialize(GameMVC gameMVC)
    {
        _gameMVC = gameMVC;

        // 獲取用戶訊息
        LoginPack loginPack = new()
        {
            UserId = DataManager.UserId,
        };
        RequestControl.GetUserInfoRequest(loginPack, (mainPack) =>
        {
            if (mainPack == null)
            {
                Debug.LogError("更新用戶訊息錯誤");
                return;
            }

            // 暱稱
            Nickname = mainPack.LoginPack.Nickname;
            // 用戶籌碼
            RecodeUserCoin = mainPack.UserInfoPack.Coin;
            // 免費輪轉次數
            RecodeFreeSpin = mainPack.UserInfoPack.FreeSpin;
            // 前個下注值
            PreBetValue = mainPack.UserInfoPack.PreBetValue;

            _gameMVC.gameControlView.InitUpdateUI();
        });
    }

    /// <summary>
    /// 處理輪轉協議
    /// </summary>
    /// <param name="mainPack"></param>
    public void HandleSlotRequest(MainPack mainPack)
    {
        // 金幣不足
        if (mainPack.ReturnCode == ReturnCode.Fail)
        {
            LanguageManager.I.GetString(LocalizationTableEnum.MessageTip_Table, "Not enough chips.", (text) =>
            {
                ViewManager.I.OpenView<MessageTipView>(ViewEnum.MessageTipView, (view) =>
                {
                    view.SetMessageTipView(
                        msg: text, 
                        isUsingCancelBtn: false, 
                        confirmCallback: () => { _gameMVC.gameControlView.OpenOperation(); }, 
                        cancelCallback: null, 
                        isDirectlyClose: false);
                });
            });
            return;
        }

        List<List<int>> spinResoultList = new();
        List<List<int>> goldCardDataList = new();
        List<List<int>> winCardPosList = new();
        List<List<BigWildData>> bigWildDataList = new();
        List<int> WinValueList = new();

        foreach (var resultList in mainPack.SpinResultListPack.SpinResultList)
        {
            // 盤面結果
            spinResoultList.Add(new(resultList.SpinResult));

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

            // 贏分
            WinValueList.Add(resultList.WinValue);
        }

        // 用戶訊息資料
        UserInfoData userInfoData = new()
        {
            UserCoin = mainPack.UserInfoPack.Coin,
            FreeSpin = mainPack.UserInfoPack.FreeSpin,
        };

        // 輪轉結果
        SlotResultData slotResultData = new()
        {
            SpinCardNumList = spinResoultList,
            GoldCardIndexList = goldCardDataList,
            WinCardPosList = winCardPosList,
            BigWildDataList = bigWildDataList,
            WinValueList = WinValueList,
            userInfoData = userInfoData,
        };

        _gameMVC.game_View.StartSlot(slotResultData);
    }

    /// <summary>
    /// 開始輪轉
    /// </summary>
    /// <param name="betValue">下注值</param>
    public void StartSpin(int betValue)
    {
        RecodeTotalWinValue = 0;
        
        if (RecodeFreeSpin == 0 &&
            RecodeUserCoin - betValue >= 0)
        {
            RecodeUserCoin = RecodeUserCoin - betValue;
        }

        _gameMVC.game_Contriller.SendSpinRequest(betValue);
    }

    /// <summary>
    /// 輪轉結束更新資料
    /// </summary>
    /// <param name="userInfoData"></param>
    public void SpinCopleteUpdateData(UserInfoData userInfoData)
    {
        if (userInfoData == null) return;

        // 用戶籌碼
        int coin = userInfoData.UserCoin;
        // 免費輪轉次數
        int freeSpin = userInfoData.FreeSpin;

        // 免費輪轉結束
        if (freeSpin == 0 && RecodeFreeSpin == 1)
        {
            _gameMVC.gameControlView.SetFreeSpinFinish();
        }

        RecodeUserCoin = coin;
        RecodeFreeSpin = freeSpin;

        if (RecodeFreeSpin == 0)
        {
            RecodeFreeSpinTotalWinValue = 0;
        }

        _gameMVC.gameControlView.SpinCopleteUpdateUI();
    }

    /// <summary>
    /// 設置贏分
    /// </summary>
    /// <param name="winValue">贏分</param>
    public void SetWinValue(int winValue)
    {
        RecodeTotalWinValue += winValue;
        _gameMVC.gameControlView.ShowWinValue(winValue);

        if (RecodeFreeSpin > 0)
        {
            RecodeFreeSpinTotalWinValue += winValue;
        }
    }
}
