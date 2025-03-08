using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotAceProtobuf;
using System;
using System.Net.Sockets;
using UnityEngine.Events;

public class ClientManager : UnitySingleton<ClientManager>
{
    private Socket socket;
    private Message message;
    private string ip;
    private const int port = 5503;

    public void OnDestroy()
    {
        message = null;

        //關閉連接        
        CloseSocket();
    }

    private void OnApplicationQuit()
    {
        //關閉連接        
        CloseSocket();
    }

    public override void Awake()
    {
        base.Awake();

        message = new();
    }

    /// <summary>
    /// 初始化連接
    /// </summary>
    public bool InitSocket()
    {
#if UNITY_EDITOR
        ip = "127.0.0.1";
#elif UNITY_ANDROID
    ip = "192.168.3.176";
#endif

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // 非同步連接
            IAsyncResult result = socket.BeginConnect(ip, port, null, null);

            // 設定超時時間
            bool success = result.AsyncWaitHandle.WaitOne(2000, true);

            if (!success || !socket.Connected)
            {
                Debug.LogWarning("連接超時或失敗");
                socket.Close();
                return false;
            }

            // 開始接收訊息
            StartReceive();
            Debug.Log("連接服務器成功");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("連接服務器失敗 :" + e);
            return false;
        }
    }

    /// <summary>
    /// 關閉連接
    /// </summary>
    private void CloseSocket()
    {
        if (socket != null && socket.Connected)
        {
            Debug.Log("關閉連接");
            socket.Close();
            socket = null;
        }
    }

    /// <summary>
    /// 開始接收訊息
    /// </summary>
    private void StartReceive()
    {
        socket.BeginReceive(message.GetBuffer, message.GetStartIndex, message.GetRemSize, SocketFlags.None, ReceiveCallBack, null);
    }

    /// <summary>
    /// 接收訊息CallBack
    /// </summary>
    /// <param name="iar"></param>
    private void ReceiveCallBack(IAsyncResult iar)
    {
        try
        {
            if (socket == null || !socket.Connected) return;

            int len = socket.EndReceive(iar);
            if (len == 0)
            {
                //關閉連接                
                CloseSocket();
                return;
            }

            //重新開始接收訊息
            message.ReadBuffer(len, HandleResponse);
            StartReceive();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 處理回覆
    /// </summary>
    /// <param name="pack"></param>
    private void HandleResponse(MainPack pack)
    {
        RequestManager.I.HandleResponse(pack);
    }

    /// <summary>
    /// 發送訊息
    /// </summary>
    /// <param name="pack"></param>
    /// <param name="callback"></param> 
    public void Send(MainPack pack, UnityAction<MainPack> callback)
    {
        RequestManager.I.AddRequest(pack.ActionCode, callback);
        socket.Send(Message.PackData(pack));
    }
}