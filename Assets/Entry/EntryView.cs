using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    /// 更新下載進度
    /// </summary>
    /// <param name="progress"></param>
    public void UpdateProgress(float progress)
    {
        LoadingProgress_Txt.text = $"{progress} %";
        LodingBar_Sli.value = progress;
    }

    /// <summary>
    /// 顯示熱更新錯誤訊息
    /// </summary>
    public void OnHotFixError()
    {
        StartCoroutine(IOnHotFixError());
    }

    private IEnumerator IOnHotFixError()
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("MessageTip_Table");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            var stringTable = loadingOperation.Result;
            entryErrorView.gameObject.SetActive(true);
            entryErrorView.SetErrorMsg(stringTable.GetEntry("Hot update failed.").GetLocalizedString());
        }
        else
        {
            Debug.LogError($"無法載入語言表 : MessageTip_Table , 錯誤:{loadingOperation.OperationException}");
        }
    }
}
