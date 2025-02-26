using UnityEngine;
using UnityEngine.Events;
using SlotAceProtobuf;

public static class RequestControl
{
    /// <summary>
    /// 登入服務器請求
    /// </summary>
    /// <param name="loginPack"></param>
    /// <param name="callback"></param>
    public static void LoginServerRequest(LoginPack loginPack, UnityAction<MainPack> callback)
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.User,
            ActionCode = ActionCode.Login,

            LoginPack = loginPack,
        };

        ClientManager.I.Send(mainPack, callback);
    }

    /// <summary>
    /// 獲取用戶訊息請求
    /// </summary>
    /// <param name="loginPack"></param>
    /// <param name="callback"></param>
    public static void GetUserInfoRequest(LoginPack loginPack, UnityAction<MainPack> callback)
    {
        MainPack mainPack = new()
        {
            RequestCode = RequestCode.User,
            ActionCode = ActionCode.GetUserInfo,

            LoginPack = loginPack,
        };

        ClientManager.I.Send(mainPack, callback);
    }
}
