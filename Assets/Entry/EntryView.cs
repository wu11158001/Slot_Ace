using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Reflection;
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
    }

    private void Start()
    {
        entryErrorView.gameObject.SetActive(false);
        //Caching.ClearCache();
        
#if UNITY_EDITOR
        //StartCoroutine(IUpdateComplete());
        //StartCoroutine(IDoUpdateAddressable());
#else
        //StartCoroutine(IDoUpdateAddressable());
#endif
    }

    /// <summary>
    /// 熱更新錯誤
    /// </summary>
    public void OnHitFixError()
    {
        LanguageManager.I.GetString(LocalizationTableEnum.MessageTip_Table, "Hot update failed.", (text) =>
        {
            entryErrorView.gameObject.SetActive(true);
            entryErrorView.SetErrorMsg(text);
        });
    }

    /*
    /// <summary>
    /// 執行熱更新
    /// </summary>
    /// <returns></returns>
    public IEnumerator IDoUpdateAddressable()
    {
        LodingBar_Img.fillAmount = 0;

        AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        // 檢測更新
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;
        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            OnHitFixError();
            yield break;
        }

        Debug.Log($"更新數量 : {checkHandle.Result.Count}");
        if (checkHandle.Result.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, false);
            yield return updateHandle;
            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                OnHitFixError();
                yield break;
            }

            // 更新列表迭代器
            List<IResourceLocator> locators = updateHandle.Result;
            foreach (var locator in locators)
            {
                LodingBar_Img.fillAmount = 0;

                List<object> keys = new();
                keys.AddRange(locator.Keys);

                // 獲取待下載文件總大小
                var sizeHandle = Addressables.GetDownloadSizeAsync(keys.GetEnumerator());
                yield return sizeHandle;
                if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    OnHitFixError();
                    yield break;
                }

                long totalDownloadSize = sizeHandle.Result;
                LoadingProgress_Txt.text = $"Download Size : {totalDownloadSize}";
                Debug.Log($"Download Size : {totalDownloadSize}");
                if (totalDownloadSize > 0)
                {
                    // 下載
                    var downloadHandle = Addressables.DownloadDependenciesAsync(keys, false);
                    while (!downloadHandle.IsDone)
                    {
                        if (downloadHandle.Status == AsyncOperationStatus.Failed)
                        {
                            OnHitFixError();
                            yield break;
                        }

                        // 下載進度
                        float percentage = downloadHandle.PercentComplete * 100f;
                        Debug.Log($"已下載 : {percentage}%");

                        // 更新加載進度條與文本
                        LodingBar_Img.fillAmount = downloadHandle.PercentComplete;
                        LoadingProgress_Txt.text = $"{percentage:F1}%";
                        LodingBar_Img.fillAmount = downloadHandle.PercentComplete;

                        yield return null;
                    }

                    if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("下載完成");
                        LodingBar_Img.fillAmount = 1;
                    }

                    downloadHandle.Release();
                }
            }

            updateHandle.Release();
        }
        else
        {
            Debug.Log("沒有更新內容");
            LoadingProgress_Txt.text = $"Enter Game";
        }

        checkHandle.Release();

        StartCoroutine(IUpdateComplete());
    }

    /// <summary>
    /// 更新完成
    /// </summary>
    public IEnumerator IUpdateComplete()
    {
        LodingBar_Img.fillAmount = 1;
        bool isEditor = false;
        Assembly ass = null;

#if UNITY_EDITOR
        isEditor = true;
        ass = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotFix");
#else
        // 啟動腳本
        var handle = Addressables.LoadAssetAsync<TextAsset>("DLL/HotFix.dll.bytes");
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset dllAsset = handle.Result;
            byte[] loadDllData = dllAsset.bytes;
            ass = Assembly.Load(loadDllData);
            Debug.Log("DLL 加載成功");
        }
        else
        {
            Debug.LogError("DLL 加載失敗");
        }  
#endif


        Type type = ass.GetType("LauncherManager");
        GameObject launcherObj = new("LauncherManager");
        launcherObj.AddComponent(type);
        type.GetMethod("GameLauncher", BindingFlags.Public | BindingFlags.Instance).Invoke(launcherObj.GetComponent(type), new object[] { isEditor });

        yield return null;
    }*/
}
