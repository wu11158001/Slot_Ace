using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotAceProtobuf;

public static class GPGSManager
{
    /// <summary>
    /// 登入Google
    /// </summary>
    /// <param name="callback"></param>
    public static void LoginGoogle(UnityAction<LoginPack> callback)
    {     
        PlayGamesPlatform.Activate().Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                string userId = PlayGamesPlatform.Instance.GetUserId();
                string nickname = PlayGamesPlatform.Instance.GetUserDisplayName();
                string imgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();

                DataManager.UserImgUrl = imgUrl;

                Debug.Log($"用戶登入 : {nickname} ID : {userId}");

                LoginPack loginPack = new()
                {
                    UserId = userId,
                    Nickname = nickname,
                };
                callback.Invoke(loginPack);
            }
            else
            {
                Debug.LogError("Google 登入失敗!!!");
                callback.Invoke(null);
            }
        });
    }
}
