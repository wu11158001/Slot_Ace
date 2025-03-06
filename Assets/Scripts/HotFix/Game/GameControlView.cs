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
    [SerializeField] Button Slot_Btn;

    private GameMVC _gameMVC;

    // 乘倍變化
    private List<int> _multiplierNormalNumList = new() { 1, 2, 3, 5 };
    private List<int> _multiplierFreeSpinNumList = new() { 2, 4, 6, 10 };

    private void Awake()
    {
        ComboTextEffect_Obj.SetActive(false);
        Slot_Btn.interactable = false;
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
        Slot_Btn.onClick.AddListener(() =>
        {
            // 下注值
            int betValue = 0;

            Slot_Btn.interactable = false;
            _gameMVC.game_Contriller.SendSpinRequest(betValue);
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

        Nickname_Txt.text = nickname;
        Coin_Txt.text = coin.ToString("N0");

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

        Coin_Txt.text = coin.ToString("N0");

        SetMultiplierText(freeSpin);
    }

    /// <summary>
    /// 設置免費輪轉相關UI
    /// </summary>
    /// <param name="freeSpin"></param>
    private void SetMultiplierText(int freeSpin)
    {
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
        Slot_Btn.interactable = true;
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
}
