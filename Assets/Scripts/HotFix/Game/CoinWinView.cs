using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class CoinWinView : MonoBehaviour
{
    [SerializeField] GameObject GetFreeSpinArea;
    [SerializeField] GameObject FreeSpinFinishArea;
    [SerializeField] TextMeshProUGUI FreeSpinTotalWon_Txt;

    /// <summary>1
    /// 獲得免費輪轉介面開關
    /// </summary>
    /// <param name="isOpen"></param>
    public void GetFreeSpinAreaSwitch(bool isOpen)
    {
        GetFreeSpinArea.SetActive(isOpen);
        FreeSpinFinishArea.SetActive(false);
    }

    /// <summary>
    /// 設置免費輪轉結束介面
    /// </summary>
    /// <param name="totalWon">免費輪轉總贏分</param>
    public void SetFreeSpinFinish(int totalWon)
    {
        GetFreeSpinArea.SetActive(false);
        FreeSpinFinishArea.SetActive(totalWon > 0);
        if (totalWon > 0)
        {
            StartCoroutine(IFreeSpinFinishEffect(totalWon));
        }
    }

    /// <summary>
    /// 免費輪轉結束介面效果
    /// </summary>
    /// <param name="totalWon">免費輪轉總贏分</param>
    /// <returns></returns>
    private IEnumerator IFreeSpinFinishEffect(int totalWon)
    {
        // 贏分增加效果
        yield return Utils.I.ICoinTextIncreaseEffect(
            start: 0,
            target: totalWon,
            during: 3,
            FreeSpinTotalWon_Txt);

        // 大小變化效果
        float initFontSize = FreeSpinTotalWon_Txt.fontSize;
        float targetFontSize = initFontSize + 50;

        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < 0.5f)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / 0.5f;
            float size = Mathf.Lerp(initFontSize, targetFontSize, progress);
            FreeSpinTotalWon_Txt.fontSize = size;
            yield return null;
        }

        startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < 0.5f)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / 0.5f;
            float size = Mathf.Lerp(targetFontSize, initFontSize, progress);
            FreeSpinTotalWon_Txt.fontSize = size;
            yield return null;
        }
        FreeSpinTotalWon_Txt.fontSize = initFontSize;

        // 關閉
        yield return new WaitForSeconds(2);
        FreeSpinFinishArea.SetActive(false);
    }
}
