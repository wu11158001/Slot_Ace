using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BigWinView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WinType_Txt;
    [SerializeField] TextMeshProUGUI TotalWinValue_Txt;
    [SerializeField] Animator TotalWinValue_Animator;

    // 贏分文字大小效果Hash
    private int _isSizeEffect_Hash = Animator.StringToHash("IsSizeEffect");

    /// <summary>
    /// 顯示大獎
    /// </summary>
    /// <param name="game_Model"></param>
    public IEnumerator IShowBigWin(Game_Model game_Model)
    {
        int totalWon = game_Model.RecodeTotalWinValue;
        int betValue = game_Model.PreBetValue;

        BigWinData bigWinData = null;
        List<BigWinData> bigWinDatas = DataManager.BigWinDataList;

        for (int i = 0; i < bigWinDatas.Count; i++)
        {
            int multiple = totalWon / betValue;
            if (multiple >= bigWinDatas[i].Multiple)
            {
                bigWinData = bigWinDatas[i];
                break;
            }
        }

        if (bigWinData == null)
        {
            Debug.LogError($"顯示大獎倍數不足 : {totalWon / betValue}");
            yield break;
        }

        // 產生效果
        GameObject effectObj = AssetsManager.I.SOManager.Effect_SO.GameObjectList[1];
        Instantiate(effectObj).transform.position = Vector3.zero;

        WinType_Txt.text = $"{bigWinData.TypeString}";
        yield return IWinEffect(totalWon);
    }

    /// <summary>
    /// 贏分效果
    /// </summary>
    /// <param name="totalWon">總贏分</param>
    /// <returns></returns>
    private IEnumerator IWinEffect(int totalWon)
    {
        // 贏分增加效果
        yield return Utils.I.ICoinTextIncreaseEffect(
            start: 0,
            target: totalWon,
            during: 3,
            TotalWinValue_Txt);

        // 大小變化效果
        TotalWinValue_Animator.SetBool(_isSizeEffect_Hash, true);

        // 關閉
        yield return new WaitForSeconds(2);
        ViewManager.I.CloseCurrView();
    }
}
