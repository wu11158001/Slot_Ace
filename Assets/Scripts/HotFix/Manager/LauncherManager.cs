using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotAceProtobuf;
using System.Reflection;
using System;
using System.Linq;

public class LauncherManager : MonoBehaviour
{
    private bool _isEditor;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// 遊戲啟動
    /// </summary>
    /// <param name="isEditor"></param>
    public void GameLauncher(bool isEditor)
    {
        Debug.Log("遊戲啟動...");
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
        yield return AssetsManager.I.Initialize();

        // 連接服務器
        bool isConnectServer = ClientManager.I.InitSocket();
        if (!isConnectServer)
        {
            // 連接服務器失敗
            OnError("Failed to connect to the server.");
            yield break;
        }

        if (_isEditor)
        {
            // 編輯器模式
            LoginPack loginPack = new()
            {
                UserId = "EditorUserId",
                Nickname = "EditorPlayer",
            };
            SendLoginServer(loginPack);
        }
        else
        {
            // 非編輯器模式
            GPGSManager.LoginGoogle(SendLoginServer);
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
            Debug.Log("Google登入失敗");
            OnError("Google login failed.");
            return;
        }

        DataManager.I.UserId = loginPack.UserId;

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
            OnError("Failed to connect to the server.");
            return;
        }

        EnterLobby();        
    }

    /// <summary>
    /// 錯誤
    /// </summary>
    /// <param name="msg"></param>
    private void OnError(string msg)
    {
        LanguageManager.I.GetString(LocalizationTableEnum.MessageTip_Table, msg, (text) =>
        {
            ViewManager.I.OpenView<MessageTipView>(ViewEnum.MessageTipView, (view) =>
            {
                view.SetMessageTipView(
                    msg: text,
                    isUsingCancelBtn: false,
                    confirmCallback: ReGameLauncher,
                    cancelCallback: null,
                    isDirectlyClose: true);
            });
        });
    }

    /// <summary>
    /// 重新啟動遊戲
    /// </summary>
    public void ReGameLauncher()
    {
        GameLauncher(_isEditor);
    }

    /// <summary>
    /// 進入大廳
    /// </summary>
    private void EnterLobby()
    {
        SceneLoadManager.I.LoadScene(SceneEnum.Game);
    }
}
