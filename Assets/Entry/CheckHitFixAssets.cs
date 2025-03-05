using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Reflection;
using System;
using System.Linq;

public class CheckHitFixAssets : MonoBehaviour
{
    private static CheckHitFixAssets _instance;
    public static CheckHitFixAssets I { get { return _instance; } }

    [SerializeField] EntryView entryView;

    private AsyncOperationHandle<IResourceLocator> _initHandle;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        Caching.ClearCache();
        InitializeAddressable();
    }

    /// <summary>
    /// 初始化Addressable
    /// </summary>
    public async void InitializeAddressable()
    {
        _initHandle = Addressables.InitializeAsync(false);
        await _initHandle.Task;
        if (_initHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("初始化Addressable失敗!");
            entryView.OnHitFixError();
            return;
        }

        ChechUpdateAssets();
    }

    /// <summary>
    /// 檢查是否有更新資源
    /// </summary>
    private async void ChechUpdateAssets()
    {
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        await checkHandle.Task;
        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("檢查是否有更新資源失敗!");
            entryView.OnHitFixError();
            return;
        }

        if (checkHandle.Result.Count <= 0)
        {
            Debug.Log("沒有更新資源");
            UpdateComplete();
            return;
        }

        UpdateAssets(checkHandle.Result);
        checkHandle.Release();
    }

    /// <summary>
    /// 獲取下載資源總大小
    /// </summary>
    /// <param name="updateAssetsKeys">更新資源目錄</param>
    private async void UpdateAssets(List<string> updateAssetsKeys)
    {
        // 下載資源
        var updateHandle = Addressables.UpdateCatalogs(true, updateAssetsKeys, false);
        await updateHandle.Task;
        if (updateHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("獲取下載資源失敗!");
            entryView.OnHitFixError();
            return;
        }

        // 獲取下載資源總大小
        long totalSize = 0;
        foreach (var locator in updateHandle.Result)
        {
            // 獲取下載資源大小
            var sizeHandle = Addressables.GetDownloadSizeAsync(locator.Keys);
            await sizeHandle.Task;
            if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"{locator.Keys} : 獲取下載資源大小失敗!");
                entryView.OnHitFixError();
                return;
            }

            totalSize += sizeHandle.Result;
            sizeHandle.Release();
        }

        Debug.Log($"獲取下載資源總大小 : {totalSize / 1048579}M");

        if (totalSize <= 0)
        {
            UpdateComplete();
            return;
        }

        DownloadAssets(updateHandle.Result);
        updateHandle.Release();
    }

    /// <summary>
    /// 下載資源
    /// </summary>
    /// <param name="locators"></param>
    private async void DownloadAssets(List<IResourceLocator> locators)
    {
        foreach (var locator in locators)
        {
            var downloadHandle = Addressables.DownloadDependenciesAsync(locator, false);
            await downloadHandle.Task;
            if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"{locator.Keys} : 下載資源失敗!");
                entryView.OnHitFixError();
                return;
            }

            // 下載進度百分比(0~1)
            float progressPrcent = downloadHandle.PercentComplete * 100;
            downloadHandle.Release();
        }

        Debug.Log("熱更新下載完成。");
        UpdateComplete();
    }

    /// <summary>
    /// 更新完成
    /// </summary>
    private async void UpdateComplete()
    {
        _initHandle.Release();

        bool isEditor = false;
        Assembly ass = null;

#if UNITY_EDITOR
        isEditor = true;
        ass = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotFix");
#else
        var dllHandle = Addressables.LoadAssetAsync<TextAsset>("DLL/HotFix.dll.bytes");
        await dllHandle.Task;
        if (dllHandle.Status == AsyncOperationStatus.Succeeded)
        {
            ass = Assembly.Load(dllHandle.Result.bytes);
            Debug.Log("DLL 加載成功");
        }
        else
        {
            Debug.LogError("DLL 加載失敗");
        }
#endif

        await Task.Yield();

        Type type = ass.GetType("LauncherManager");
        GameObject launcherObj = new("LauncherManager");
        launcherObj.AddComponent(type);
        type.GetMethod("GameLauncher", BindingFlags.Public | BindingFlags.Instance).Invoke(launcherObj.GetComponent(type), new object[] { isEditor });
    }
}
