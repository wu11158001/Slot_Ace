using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.Events;

public class MessageTipView : BasePopUpView
{
    [Space(30)]
    [Header("訊息提示")]
    [SerializeField] TextMeshProUGUI Msg_Txt;
    [SerializeField] Button Confirm_Btn;
    [SerializeField] Button Cancel_Btn;

    /// <summary>
    /// 設置訊息介面
    /// </summary>
    /// <param name="msg">訊息內容</param>
    /// <param name="isUsingCancelBtn">是否使用取消按鈕</param>
    /// <param name="confirmCallback">確認回傳</param>
    /// <param name="cancelCallback">取消回傳</param>
    /// <param name="isDirectlyClose">直接關閉介面</param>
    public void SetMessageTipView(string msg, bool isUsingCancelBtn, UnityAction confirmCallback, UnityAction cancelCallback = null, bool isDirectlyClose = false)
    {
        Msg_Txt.text = msg;

        // 確認按鈕
        Confirm_Btn.onClick.RemoveAllListeners();
        Confirm_Btn.onClick.AddListener(() =>
        {
            confirmCallback?.Invoke();

            if (isDirectlyClose) ViewManager.I.CloseCurrView();
            else StartCoroutine(ICloseView());
        });

        // 取消按鈕
        Cancel_Btn.gameObject.SetActive(isUsingCancelBtn);
        if (isUsingCancelBtn)
        {
            Cancel_Btn.onClick.RemoveAllListeners();
            Cancel_Btn.onClick.AddListener(() =>
            {
                cancelCallback?.Invoke();

                if (isDirectlyClose) ViewManager.I.CloseCurrView();
                else StartCoroutine(ICloseView());
            });
        }
    }
}
