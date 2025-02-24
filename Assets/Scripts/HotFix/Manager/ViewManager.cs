using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Events;

public class ViewManager : UnitySingleton<ViewManager>
{
    // 已開啟介面
    private Stack<RectTransform> _openedView = new();

    // 當前場景Canvas RectTransform
    public RectTransform CurrSceneCanvasRt { get; private set; }

    // 所有介面物件
    private Dictionary<ViewEnum, GameObject> _viewObjDic;
    // 已創建介面
    private Dictionary<ViewEnum, RectTransform> _createdViewDic = new();

    /// <summary>
    /// 載入所有介面
    /// </summary>
    /// <returns></returns>
    public IEnumerator ILoadViewAssets()
    {
        _viewObjDic = new();

        foreach (var viewEnum in Enum.GetValues(typeof(ViewEnum)))
        {
            var handle = Addressables.LoadAssetAsync<GameObject>($"Prefab/View/{(ViewEnum)viewEnum}.prefab");
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject viewObj = handle.Result;
                _viewObjDic.Add((ViewEnum)viewEnum, viewObj);
            }
            else
            {
                Debug.LogError($"{viewEnum} 加載失敗");
            }
        }
    }

    /// <summary>
    /// 設置當前場景Canvas
    /// </summary>
    private void SetCurrSceneCanvas()
    {
        CurrSceneCanvasRt = GameObject.Find("Canvas").GetComponent<RectTransform>();
        _createdViewDic.Clear();
        _openedView.Clear();
    }

    /// <summary>
    /// 重製介面資料
    /// </summary>
    public void ResetViewData()
    {
        SetCurrSceneCanvas();
    }

    /// <summary>
    /// 開啟介面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="openView"></param>
    /// <param name="callback"></param>
    public void OpenView<T>(ViewEnum openView, UnityAction<T> callback = null) where T : Component
    {
        if (CurrSceneCanvasRt == null)
        {
            SetCurrSceneCanvas();
        }

        if (!_viewObjDic.ContainsKey(openView))
        {
            Debug.LogError($"無法找到介面 : {openView}");
            return;
        }

        if (_createdViewDic.ContainsKey(openView))
        {
            // 介面已創建

            RectTransform view = _createdViewDic[openView];
            CreateViewHandle(view, CurrSceneCanvasRt, callback);
            _openedView.Push(view);
        }
        else
        {
            // 介面未創建

            RectTransform newView = Instantiate(_viewObjDic[openView], CurrSceneCanvasRt).GetComponent<RectTransform>();
            CreateViewHandle(newView, CurrSceneCanvasRt, callback);
            _createdViewDic.Add(openView, newView);
            _openedView.Push(newView);
        }

        
    }

    /// <summary>
    /// 關閉當前介面
    /// </summary>
    public void CloseCurrView()
    {
        _openedView.Pop().gameObject.SetActive(false);
    }

    /// <summary>
    /// 產生介面處理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="view"></param>
    /// <param name="canvasRt"></param>
    /// <param name="callback"></param>
    public void CreateViewHandle<T>(RectTransform view, RectTransform canvasRt, UnityAction<T> callback = null) where T : Component
    {
        view.gameObject.SetActive(true);
        view.offsetMax = Vector2.zero;
        view.offsetMin = Vector2.zero;
        view.anchoredPosition = Vector2.zero;
        view.eulerAngles = Vector3.zero;
        view.localScale = Vector3.one;
        view.name = view.name.Replace("(Clone)", "");
        view.SetSiblingIndex(canvasRt.childCount + 1);

        ActionCallback(view, callback);
    }

    /// <summary>
    /// 執行回傳方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="view"></param>
    /// <param name="callback"></param>
    private void ActionCallback<T>(RectTransform view, UnityAction<T> callback) where T : Component
    {
        if (callback != null)
        {
            T component = view.GetComponent<T>();
            if (component != null)
            {
                callback?.Invoke(component);
            }
            else
            {
                Debug.LogError($"{view.name}: 介面不存在 Component");
            }
        }
    }
}
