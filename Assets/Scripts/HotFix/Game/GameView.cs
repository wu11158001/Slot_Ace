using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SlotAceProtobuf;

public class GameView : MonoBehaviour
{
    [Header("用戶訊息")]
    [SerializeField] Image Avatar_Img;
    [SerializeField] TextMeshProUGUI Nickname_Txt;
    [SerializeField] TextMeshProUGUI Coin_Txt;

    private void Awake()
    {
        // 載入頭像
        StartCoroutine(Utils.I.ImageUrlToSprite(DataManager.I.UserImgUrl, (sprite) =>
        {
            if (sprite != null) Avatar_Img.sprite = sprite;
        }));

        // 獲取用戶訊息
        LoginPack loginPack = new()
        {
            UserId = DataManager.I.UserId,
        };
        RequestControl.GetUserInfoRequest(loginPack, UpdateUserInfo);
    }

    /// <summary>
    /// 更新用戶訊息
    /// </summary>
    /// <param name="mainPack"></param>
    private void UpdateUserInfo(MainPack mainPack)
    {
        if (mainPack == null)
        {
            Debug.LogError("更新用戶訊息錯誤");
            return;
        }

        Nickname_Txt.text = mainPack.LoginPack.Nickname;
        Coin_Txt.text = mainPack.UserInfoPack.Coin.ToString("N0");
    }
}
