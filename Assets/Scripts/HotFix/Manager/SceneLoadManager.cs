using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoadManager : UnitySingleton<SceneLoadManager>
{
    private SceneEnum _activeScene;

    /// <summary>
    /// 載入場景
    /// </summary>
    /// <param name="sceneEnum"></param>
    public void LoadScene(SceneEnum sceneEnum)
    {
        Debug.Log("正在加載場景: " + sceneEnum);
        _activeScene = sceneEnum;
        Addressables.LoadSceneAsync($"Scenes/{sceneEnum}.unity", LoadSceneMode.Single).Completed += OnSceneLoaded;
    }

    /// <summary>
    /// 當場景加載完成回調
    /// </summary>
    /// <param name="handle"></param>
    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"進入場景 : {_activeScene}");
            ViewManager.I.ResetViewData();

            switch (_activeScene)
            {
                // 遊戲
                case SceneEnum.Game:
                    Addressables.LoadAssetAsync<GameObject>("Prefab/Game/GameMVC.prefab").Completed += (assets) =>
                    {
                        Instantiate(assets.Result);
                    };
                    break;
            }
        }
        else
        {
            Debug.LogError("場景加載失敗: " + handle.OperationException);
        }
    }
}
