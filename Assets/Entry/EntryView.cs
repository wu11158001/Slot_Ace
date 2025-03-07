using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class EntryView : MonoBehaviour
{
    [SerializeField] Slider LodingBar_Sli;
    [SerializeField] TextMeshProUGUI LoadingProgress_Txt;
    [SerializeField] EntryErrorView entryErrorView;

    private static EntryView _instance;
    public static EntryView I { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null) _instance = this;

        entryErrorView.gameObject.SetActive(false);
    }

    /// <summary>
    /// 顯示熱更新錯誤訊息
    /// </summary>
    public void OnHitFixError()
    {
        LanguageManager.I.GetString(LocalizationTableEnum.MessageTip_Table, "Hot update failed.", (text) =>
        {
            entryErrorView.gameObject.SetActive(true);
            entryErrorView.SetErrorMsg(text);
        });
    }
}
