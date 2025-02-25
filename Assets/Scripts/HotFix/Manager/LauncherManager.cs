using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotAceProtobuf;

public class LauncherManager : UnitySingleton<LauncherManager>
{
    private bool _isEditor;

    /// <summary>
    /// 遊戲啟動
    /// </summary>
    /// <param name="isEditor"></param>
    public void GameLauncher(bool isEditor)
    {
        Debug.Log("遊戲啟動");
        _isEditor = isEditor;

        new GameObject("UnityMainThreadDispatcher").AddComponent<UnityMainThreadDispatcher>();

        StartCoroutine(IProjectInitialize());
    }

    /// <summary>
    /// 專案初始化
    /// </summary>
    /// <returns></returns>
    private IEnumerator IProjectInitialize()
    {
        yield return ViewManager.I.ILoadViewAssets();

        // 連接服務器
        bool isConnectServer = ClientManager.I.InitSocket();
        if (!isConnectServer)
        {
            // 連接服務器失敗

            yield break;
        }

        if (_isEditor)
        {
            // 編輯器模式

            LoginPack loginPack = new()
            {
                UserId = "EditorUserId",
                NickName = "EditorPlayer",
            };
            SendLoginServer(loginPack);
        }
        else
        {
            // 非編輯器模式

            GPGSManager.I.LoginGoogle(SendLoginServer);
        }
    }

    /// <summary>
    /// 發送登入服務器
    /// </summary>
    /// <param name="loginPack"></param>
    private void SendLoginServer(LoginPack loginPack)
    {
        if (loginPack == null)
        {
            Debug.Log("登入資料為空");
            return;
        }

        RequestControl.LoginServerRequest(loginPack, OnLoginServerResult);
    }

    /// <summary>
    /// 登入服務器結果
    /// </summary>
    /// <param name="mainPack"></param>
    private void OnLoginServerResult(MainPack mainPack)
    {
        if (mainPack.ReturnCode != ReturnCode.Succeed)
        {
            // 登入服務器失敗

            return;
        }

        LoadSceneManager.I.LoadScene(SceneEnum.Lobby);
    }
}
