using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotAceProtobuf;
using UnityEngine.Events;
using SlotAceProtobuf;

public class RequestManager : UnitySingleton<RequestManager>
{
    private static Dictionary<ActionCode, UnityAction<MainPack>> _requsetDic = new();

    /// <summary>
    /// 添加請求
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="callback"></param>
    public void AddRequest(ActionCode actionCode, UnityAction<MainPack> callback)
    {
        _requsetDic.Add(actionCode, callback);
    }

    /// <summary>
    /// 移除請求
    /// </summary>
    /// <param name="actionCode"></param>
    private void RemoveRequest(ActionCode actionCode)
    {
        _requsetDic.Remove(actionCode);
    }

    /// <summary>
    /// 處理回覆
    /// </summary>
    /// <param name="mainPack"></param>
    public void HandleResponse(MainPack mainPack)
    {
        if (_requsetDic.ContainsKey(mainPack.ActionCode))
        {
            UnityMainThreadDispatcher.I.Enqueue(() =>
            {
                _requsetDic[mainPack.ActionCode].Invoke(mainPack);
                RemoveRequest(mainPack.ActionCode);
            });
        }
        else
        {
            Debug.LogError("不能找到對應的處理:" + mainPack.ActionCode);
        }
    }
}
