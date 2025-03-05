using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class AssetsManager : UnitySingleton<AssetsManager>
{
    // 介面物件
    public Dictionary<ViewEnum, GameObject> ViewObjDic { get; private set; }

    // SO管理中心
    public SOManager SOManager;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    public IEnumerator Initialize()
    {
        // 載入SO管理中心
        var soHandle = Addressables.LoadAssetAsync<GameObject>("Prefab/Manager/SOManager.prefab");
        yield return soHandle;

        if (soHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject soObj = soHandle.Result;
            SOManager = soObj.GetComponent<SOManager>();
        }
        else
        {
            Debug.LogError($"載入SO管理中心 加載失敗");
        }

        // 載入介面物件
        ViewObjDic = new();
        foreach (var viewEnum in Enum.GetValues(typeof(ViewEnum)))
        {
            var viewHandle = Addressables.LoadAssetAsync<GameObject>($"Prefab/View/{(ViewEnum)viewEnum}.prefab");
            yield return viewHandle;

            if (viewHandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject viewObj = viewHandle.Result;
                ViewObjDic.Add((ViewEnum)viewEnum, viewObj);
            }
            else
            {
                Debug.LogError($"{viewEnum} 加載失敗");
            }
        }

        Debug.Log("資源管理中心 初始化完成。");
        yield return null;
    }
}
