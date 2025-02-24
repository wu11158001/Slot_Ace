using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LauncherManager : UnitySingleton<LauncherManager>
{
    /// <summary>
    /// 遊戲啟動
    /// </summary>
    /// <param name="isEditor"></param>
    public void GameLauncher(bool isEditor)
    {
        Debug.Log("遊戲啟動");
        StartCoroutine(IProjectInitialize());
    }

    /// <summary>
    /// 專案初始化
    /// </summary>
    /// <returns></returns>
    private IEnumerator IProjectInitialize()
    {
        yield return ViewManager.I.ILoadViewAssets();

        LoadSceneManager.I.LoadScene(SceneEnum.Lobby);
    }
}
