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
    [SerializeField] Image LodingBar_Img;
    [SerializeField] TextMeshProUGUI LoadingProgress_Txt;
    [SerializeField] Button Retry_Btn;

    private void Start()
    {
        Retry_Btn.gameObject.SetActive(false);
        Retry_Btn.onClick.AddListener(() =>
        {
            StartCoroutine(IDoUpdateAddressable());
            Retry_Btn.gameObject.SetActive(false);
        });

        Caching.ClearCache();
        
#if UNITY_EDITOR
        StartCoroutine(IUpdateComplete());
#else
        StartCoroutine(IDoUpdateAddressable());
#endif
    }

    /// <summary>
    /// 執行熱更新
    /// </summary>
    /// <returns></returns>
    private IEnumerator IDoUpdateAddressable()
    {
        LodingBar_Img.fillAmount = 0;

        // 檢測更新
        var checkHandle = Addressables.CheckForCatalogUpdates(true);
        yield return checkHandle;
        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            OnError("檢測更新錯誤");
            yield break;
        }

        if (checkHandle.Result.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, true);
            yield return updateHandle;
            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                OnError("更新錯誤");
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
                    OnError("獲取待下載文件總大小錯誤");
                    yield break;
                }

                long totalDownloadSize = sizeHandle.Result;
                LoadingProgress_Txt.text = $"Download Size : {totalDownloadSize}";
                Debug.Log($"Download Size : {totalDownloadSize}");
                if (totalDownloadSize > 0)
                {
                    // 下載
                    var downloadHandle = Addressables.DownloadDependenciesAsync(keys, true);
                    while (!downloadHandle.IsDone)
                    {
                        if (downloadHandle.Status == AsyncOperationStatus.Failed)
                        {
                            OnError($"下載錯誤 : {downloadHandle.OperationException}");
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
                }
            }
        }
        else
        {
            Debug.Log("沒有更新內容");
            LoadingProgress_Txt.text = $"Enter Game";
        }

        LodingBar_Img.fillAmount = 1;
        StartCoroutine(IUpdateComplete());
    }

    /// <summary>
    /// 錯誤
    /// </summary>
    /// <param name="msg"></param>
    private void OnError(string msg)
    {
        Debug.LogError(msg);
        Retry_Btn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 更新完成
    /// </summary>
    private IEnumerator IUpdateComplete()
    {
        yield return null;

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
        GameObject launcherObj = new GameObject("LauncherManager");
        launcherObj.AddComponent(type);
        type.GetMethod("GameLauncher").Invoke(launcherObj.GetComponent(type), new object[] { isEditor });
    }
}
