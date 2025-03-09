using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ChangeBetValueView : BasePopUpView
{
    [Space(30)]
    [Header("遊戲介面")]
    [SerializeField] GameControlView _gameControlView;

    [Space(30)]
    [Header("更換下注值")]
    [SerializeField] Color NormalBtnColor;
    [SerializeField] Color SelectBtnColor;
    [SerializeField] List<Button> BtnList;    

    // 下注值更改值
    private List<int> _changeBetValueList = new()
    {
        10, 15, 20,
        50, 100, 200,
        500, 1000, 5000,
    };

    // 當前選擇按鈕編號
    private int _currBtnIndex;

    //當前下注值
    public int BetValue { get; private set; }

    private void Start()
    {
        // 設置下注值更改按鈕
        for (int i = 0; i < BtnList.Count; i++)
        {
            int index = i;

            TextMeshProUGUI betBtnTxt = BtnList[i].GetComponentInChildren<TextMeshProUGUI>();
            betBtnTxt.text = $"{_changeBetValueList[i]}";

            BtnList[i].onClick.AddListener(() =>
            {
                AudioManager.I.PlaySound(SoundEnum.ButtonClick);
                SetBetValueData(index);
                StartCoroutine(ICloseView());
            });
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SetBtnColor();
    }

    /// <summary>
    /// 設置初始下注值
    /// </summary>
    /// <param name="betValue"></param>
    public void SetInitBetValue(int betValue)
    {
        for (int i = 0; i < _changeBetValueList.Count; i++)
        {
            if (betValue == _changeBetValueList[i])
            {
                SetBetValueData(i);
                return;
            }
        }

        // 預設下注值
        SetBetValueData(4);
    }

    /// <summary>
    /// 設置下注值資料
    /// </summary>
    /// <param name="index"></param>
    private void SetBetValueData(int index)
    {
        _currBtnIndex = index;
        BetValue = _changeBetValueList[index];
        SetBtnColor();
        _gameControlView.SetBetValueText(BetValue);
    }

    /// <summary>
    /// 設置按鈕顏色
    /// </summary>
    private void SetBtnColor()
    {
        for (int i = 0; i < BtnList.Count; i++)
        {
            if (_currBtnIndex == i)
            {
                BtnList[i].image.color = SelectBtnColor;
            }
            else
            {
                BtnList[i].image.color = NormalBtnColor;
            }
        }
    }
}
