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
    [SerializeField] CoinWinView coinWinView;

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

    private void Awake()
    {
        TotalWinValue_Txt.text = $"0";
        ComboTextEffect_Obj.SetActive(false);
        Spin_Btn.interactable = false;

        // 載入頭像
        StartCoroutine(Utils.I.ImageUrlToSprite(DataManager.I.UserImgUrl, (sprite) =>
        {
            if (sprite != null) Avatar_Img.sprite = sprite;
        }));
    }

    private void Start()
    {
        // 輪轉按鈕
        Spin_Btn.onClick.AddListener(() =>
        {
            // 下注值
            int betValue = changeBetValueView.BetValue;
            _gameMVC.game_Model.StartSpin(betValue);

            TotalWinValue_Txt.text = _gameMVC.game_Model.RecodeTotalWinValue.ToString("N0");
            Coin_Txt.text = _gameMVC.game_Model.RecodeUserCoin.ToString("N0");
            Spin_Btn.interactable = false;
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
    /// 初始更新介面
    /// </summary>
    public void InitUpdateUI()
    {
        Nickname_Txt.text = _gameMVC.game_Model.Nickname; ;
        Coin_Txt.text = _gameMVC.game_Model.RecodeUserCoin.ToString("N0");

        changeBetValueView.SetInitBetValue(_gameMVC.game_Model.PreBetValue);
        changeBetValueView.gameObject.SetActive(false);

        SetMultiplierText(_gameMVC.game_Model.RecodeFreeSpin);
    }

    /// <summary>
    /// 輪轉結束更新介面
    /// </summary>
    public void SpinCopleteUpdateUI()
    {
        Coin_Txt.text = _gameMVC.game_Model.RecodeUserCoin.ToString("N0");
        SetMultiplierText(_gameMVC.game_Model.RecodeFreeSpin);
    }

    /// <summary>
    /// 顯示贏分
    /// </summary>
    /// <param name="winValue">贏分</param>
    public void ShowWinValue(int winValue)
    {
        int tempUserCoin = _gameMVC.game_Model.RecodeUserCoin;

        if (winValue > 0)
        {
            Coin_Txt.text = (tempUserCoin + winValue).ToString("N0");
        }        

        TotalWinValue_Txt.text = _gameMVC.game_Model.RecodeTotalWinValue.ToString("N0");
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
    /// 獲得金幣中獎畫面開關
    /// </summary>
    /// <param name="isOpen"></param>
    public void GetFreeSpinAreaSwitch(bool isOpen)
    {
        coinWinView.GetFreeSpinAreaSwitch(isOpen);
    }

    /// <summary>
    /// 設置免費輪轉結束介面
    /// </summary>
    /// <param name="totalWon"></param>
    public void SetFreeSpinFinish()
    {
        int totalWon = _gameMVC.game_Model.RecodeFreeSpinTotalWinValue;
        coinWinView.SetFreeSpinFinish(totalWon);
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
