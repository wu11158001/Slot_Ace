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
            ActionCode = ActionCode.Login,
            RequestCode = RequestCode.User,

            LoginPack = loginPack,
        };

        ClientManager.I.Send(mainPack, callback);
    }
}
