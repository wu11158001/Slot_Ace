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
    /// <param name="betValue"></param>
    public void SendSpinRequest(int betValue)
    {
        SpinRequestPack spinRequestPack = new()
        {
            BetValue = betValue,
        };
        RequestControl.SlotRequest(spinRequestPack, _gameMVC.game_Model.HandleSlotRequest);
    }
}