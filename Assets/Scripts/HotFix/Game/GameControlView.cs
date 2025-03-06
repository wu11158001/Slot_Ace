using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SlotAceProtobuf;
using System.Collections;
using System.Collections.Generic;

public class GameControlView : MonoBehaviour
{
    [Header("用戶訊息")]
    [SerializeField] Image Avatar_Img;
    [SerializeField] TextMeshProUGUI Nickname_Txt;
    [SerializeField] TextMeshProUGUI Coin_Txt;

    [Space(30)]
    [Header("乘倍背景")]
    [SerializeField] Color MultiplierBgNormalColor;
    [SerializeField] Color MultiplierBgFreeGameColor;
    [SerializeField] Image MultiplierBg_Img;

    [Space(30)]
    [Header("乘倍文字")]
    [SerializeField] Color NormalMultiplierTextColor;
    [SerializeField] Color CurrMultiplierTextColor;
    [SerializeField] List<TextMeshProUGUI> MultiplierTextList;
    [SerializeField] GameObject ComboTextEffect_Obj;
    [SerializeField] TextMeshProUGUI Combo_Num;

    [Space(30)]
    [Header("免費遊戲")]
    [SerializeField] TextMeshProUGUI FreeSpinCount_Txt;

    [Space(30)]
    [Header("贏分")]
    [SerializeField] TextMeshProUGUI TotalWinValue_Txt;

    [Space(30)]
    [Header("金幣中獎")]
    [SerializeField] GameObject CoinWinArea;

    [Space(30)]
    [Header("遊戲操作")]
    [SerializeField] Button Spin_Btn;

    [Space(30)]
    [Header("下注值")]
    [SerializeField] Button ChangeBetValue_Btn;
    [SerializeField] TextMeshProUGUI CurrBetValue_Txt;
    [SerializeField] ChangeBetValueView changeBetValueView;

    private GameMVC _gameMVC;

    // 乘倍變化
    private List<int> _multiplierNormalNumList = new() { 1, 2, 3, 5 };
    private List<int> _multiplierFreeSpinNumList = new() { 2, 4, 6, 10 };

    // 當前總贏分
    private int _currTotalWinValue;
    // 用戶籌碼
    private int _userCoin;
    // 免費輪轉
    private int _freeSpin;

    private void Awake()
    {
        TotalWinValue_Txt.text = $"0";
        ComboTextEffect_Obj.SetActive(false);
        Spin_Btn.interactable = false;
        SwitchCoinWinArea(false);

        // 載入頭像
        StartCoroutine(Utils.I.ImageUrlToSprite(DataManager.I.UserImgUrl, (sprite) =>
        {
            if (sprite != null) Avatar_Img.sprite = sprite;
        }));

        // 獲取用戶訊息
        LoginPack loginPack = new()
        {
            UserId = DataManager.I.UserId,
        };
        RequestControl.GetUserInfoRequest(loginPack, UpdateUserInfo);
    }

    private void Start()
    {
        // 輪轉按鈕
        Spin_Btn.onClick.AddListener(() =>
        {
            // 下注值
            int betValue = changeBetValueView.BetValue;

            _currTotalWinValue = 0;
            TotalWinValue_Txt.text = _currTotalWinValue.ToString("N0");

            if (_freeSpin == 0)
            {
                Coin_Txt.text = (_userCoin - betValue).ToString("N0");
            }

            Spin_Btn.interactable = false;
            _gameMVC.game_Contriller.SendSpinRequest(betValue);
        });

        // 更換下注值按鈕
        ChangeBetValue_Btn.onClick.AddListener(() =>
        {
            changeBetValueView.gameObject.SetActive(true);
        });
    }

    /// <summary>
    /// 設置遊戲MVC
    /// </summary>
    /// <param name="gameMVC"></param>
    public void SetGameMVC(GameMVC gameMVC)
    {
        _gameMVC = gameMVC;
    }

    /// <summary>
    /// 更新用戶訊息
    /// </summary>
    /// <param name="mainPack"></param>
    private void UpdateUserInfo(MainPack mainPack)
    {
        if (mainPack == null)
        {
            Debug.LogError("更新用戶訊息錯誤");
            return;
        }

        // 暱稱
        string nickname = mainPack.LoginPack.Nickname;
        // 用戶籌碼
        int coin = mainPack.UserInfoPack.Coin;
        // 免費輪轉次數
        int freeSpin = mainPack.UserInfoPack.FreeSpin;
        // 前個下注值
        int preBetValue = mainPack.UserInfoPack.PreBetValue;

        _userCoin = coin;
        _freeSpin = freeSpin;

        Nickname_Txt.text = nickname;
        Coin_Txt.text = coin.ToString("N0");

        changeBetValueView.SetInitBetValue(preBetValue);
        changeBetValueView.gameObject.SetActive(false);

        SetMultiplierText(freeSpin);
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

        _userCoin = coin;
        _freeSpin = freeSpin;

        Coin_Txt.text = coin.ToString("N0");

        SetMultiplierText(freeSpin);
    }

    /// <summary>
    /// 設置贏分
    /// </summary>
    /// <param name="winValue">贏分</param>
    public void SetWinValue(int winValue)
    {
        int tempUserCoin = _userCoin;

        if (winValue > 0)
        {
            Coin_Txt.text = (tempUserCoin + winValue).ToString("N0");
        }        

        _currTotalWinValue += winValue;
        TotalWinValue_Txt.text = _currTotalWinValue.ToString("N0");
    }

    /// <summary>
    /// 設置免費輪轉相關UI
    /// </summary>
    /// <param name="freeSpin"></param>
    private void SetMultiplierText(int freeSpin)
    {
        ChangeBetValue_Btn.interactable = freeSpin == 0;
        FreeSpinCount_Txt.gameObject.SetActive(freeSpin > 0);
        FreeSpinCount_Txt.text = $"{freeSpin}";

        MultiplierBg_Img.color =
            freeSpin > 0 ?
            MultiplierBgFreeGameColor :
            NormalMultiplierTextColor;

        if (freeSpin > 0)
        {
            for (int i = 0; i < MultiplierTextList.Count; i++)
            {
                MultiplierTextList[i].text = $"X{_multiplierFreeSpinNumList[i]}";
            }
        }
        else
        {
            for (int i = 0; i < MultiplierTextList.Count; i++)
            {
                MultiplierTextList[i].text = $"X{_multiplierNormalNumList[i]}";
            }
        }
    }

    /// <summary>
    /// 開放操作
    /// </summary>
    public void OpenOperation()
    {
        SetComboPanelText(-1);
        Spin_Btn.interactable = true;
    }

    /// <summary>
    /// 設置連擊面板文字
    /// </summary>
    /// <param name="currIndex">當前連擊數</param>
    public void SetComboPanelText(int combo)
    {
        SetComboEffectText(combo + 1);

        int currIndex = combo;
        if (currIndex >= MultiplierTextList.Count)
        {
            currIndex = MultiplierTextList.Count - 1;
        }
        else if (currIndex <= 0)
        {
            currIndex = 0;
        }

        for (int i = 0; i < MultiplierTextList.Count; i++)
        {
            if (currIndex == i)
            {
                MultiplierTextList[i].color = CurrMultiplierTextColor;
            }
            else
            {
                MultiplierTextList[i].color = NormalMultiplierTextColor;
            }
        }
    }

    /// <summary>
    /// 設置連擊文字
    /// </summary>
    /// <param name="combo"></param>
    public void SetComboEffectText(int combo)
    {
        if (combo <= 0)
        {
            ComboTextEffect_Obj.SetActive(false);
            return;
        }

        ComboTextEffect_Obj.SetActive(false);
        ComboTextEffect_Obj.SetActive(true);
        Combo_Num.text = $"X{combo}";
    }

    /// <summary>
    /// 金幣中獎畫面開關
    /// </summary>
    /// <param name="isOpen"></param>
    public void SwitchCoinWinArea(bool isOpen)
    {
        CoinWinArea.SetActive(isOpen);
    }

    /// <summary>
    /// 設置當前下注值文字
    /// </summary>
    /// <param name="betValue"></param>
    public void SetBetValueText(int betValue)
    {
        LanguageManager.I.GetString(LocalizationTableEnum.Game_Table, "Bet", (text) =>
        {
            CurrBetValue_Txt.text = $"{text} : {betValue}";
        });        
    }
}
