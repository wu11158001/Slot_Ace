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
    [Header("連擊")]
    [SerializeField] Color NormalCombosTextColor;
    [SerializeField] Color CurrCombosTextColor;
    [SerializeField] List<TextMeshProUGUI> CombosTextList;
    [SerializeField] GameObject ComboTextEffect_Obj;
    [SerializeField] TextMeshProUGUI Combo_Num;

    [Space(30)]
    [Header("遊戲操作")]
    [SerializeField] Button Slot_Btn;

    private GameMVC _gameMVC;

    private void Awake()
    {
        ComboTextEffect_Obj.SetActive(false);
        Slot_Btn.interactable = false;

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
            Slot_Btn.interactable = false;
            _gameMVC.game_Contriller.SendSlotRequest();            
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

        Nickname_Txt.text = mainPack.LoginPack.Nickname;
        Coin_Txt.text = mainPack.UserInfoPack.Coin.ToString("N0");
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
        if (currIndex >= CombosTextList.Count)
        {
            currIndex = CombosTextList.Count - 1;
        }
        else if (currIndex <= 0)
        {
            currIndex = 0;
        }

        for (int i = 0; i < CombosTextList.Count; i++)
        {
            if (currIndex == i)
            {
                CombosTextList[i].color = CurrCombosTextColor;
            }
            else
            {
                CombosTextList[i].color = NormalCombosTextColor;
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
        Combo_Num.text = $"{combo}";
    }
}
