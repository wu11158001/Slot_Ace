using UnityEngine;
using SlotAceProtobuf;

public class Game_Controller : MonoBehaviour
{
    private GameMVC _gameMVC;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="gameMVC"></param>
    public void Initialize(GameMVC gameMVC)
    {
        _gameMVC = gameMVC;
    }

    /// <summary>
    /// 發送輪轉請求
    /// </summary>
    public void SendSlotRequest()
    {
        LoginPack loginPack = new()
        {
            UserId = DataManager.I.UserId,
        };
        RequestControl.SlotRequest(loginPack, _gameMVC.game_Model.HandleSlotRequest);
    }
}
