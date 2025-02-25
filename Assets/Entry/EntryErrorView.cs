using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

public class EntryErrorView : MonoBehaviour
{
    [Header("彈窗")]
    [SerializeField] Image Mask_Img;
    [SerializeField] RectTransform SlideArea;

    [Space(30)]
    [Header("錯誤訊息")]
    [SerializeField] TextMeshProUGUI Msg_Txt;
    [SerializeField] Button Retry_Btn;

    private void OnEnable()
    {
        StartCoroutine(IDisplayView());
    }

    /// <summary>
    /// 介面顯示
    /// </summary>
    /// <returns></returns>
    private IEnumerator IDisplayView()
    {
        float alphaMax = 200;                         // 遮罩透明值最大值
        float maskEffectTime = 0.2f;                  // 遮罩效果完成時間
        float slideViewMaxPosY = 50.0f;               // 滑動介面最頂部位置
        float slideViewReboundTime = 0.1f;            // 滑動介面回彈時間
        float slideViewPauseTime = 0.05f;             // 滑動介面停頓時間

        DateTime startTime = DateTime.Now;

        Color color = Mask_Img.color;
        while ((DateTime.Now - startTime).TotalSeconds <= maskEffectTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / maskEffectTime;

            // 遮罩淡入效果
            color.a = Mathf.Lerp(0, alphaMax / 255, progress);
            Mask_Img.color = color;

            // 介面向上滑動效果
            float posY = Mathf.Lerp(-Screen.height, slideViewMaxPosY, progress);
            SlideArea.anchoredPosition = new Vector2(0, posY);

            yield return null;
        }
        color.a = alphaMax / 255;
        Mask_Img.color = color;
        SlideArea.anchoredPosition = new Vector2(0, slideViewMaxPosY);

        yield return new WaitForSecondsRealtime(slideViewPauseTime);

        startTime = DateTime.Now;
        float currViewPosY = SlideArea.anchoredPosition.y;
        while ((DateTime.Now - startTime).TotalSeconds <= slideViewReboundTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / slideViewReboundTime;

            // 介面回彈滑動效果
            float posY = Mathf.Lerp(currViewPosY, 0, progress);
            SlideArea.anchoredPosition = new Vector2(0, posY);

            yield return null;
        }
        SlideArea.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// 介面關閉
    /// </summary>
    /// <returns></returns>
    private IEnumerator ICloseView()
    {
        float slideViewReboundPosY = 50.0f;             // 滑動介面回彈位置
        float slideViewReboundTime = 0.1f;              // 滑動介面回彈時間
        float waitTime = 0.05f;                         // 等待開始下滑時間
        float slideTime = 0.2f;                         // 下滑完成時間

        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds <= slideViewReboundTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / slideViewReboundTime;
            // 介面向上滑動效果
            float posY = Mathf.Lerp(0, slideViewReboundPosY, progress);
            SlideArea.anchoredPosition = new Vector2(0, posY);

            yield return null;
        }

        yield return new WaitForSeconds(waitTime);

        Color color = Mask_Img.color;
        float currMaskAlpha = color.a;
        float currViewPosY = SlideArea.anchoredPosition.y;
        startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds <= slideTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / slideTime;

            // 遮罩淡出效果
            color.a = Mathf.Lerp(currMaskAlpha, 0, progress);
            Mask_Img.color = color;

            // 介面向下滑動效果
            float posY = Mathf.Lerp(currViewPosY, -Screen.height, progress);
            SlideArea.anchoredPosition = new Vector2(0, posY);

            yield return null;
        }

        ViewManager.I.CloseCurrView();
    }

    /// <summary>
    /// 設置錯誤訊息
    /// </summary>
    /// <param name="msg">錯誤訊息</param>
    /// <param name="isReloadHotFix">重載熱更/重新連接服務器</param>
    public void SetErrorMsg(string msg, bool isReloadHotFix)
    {
        Msg_Txt.text = msg;
        Retry_Btn.onClick.RemoveAllListeners();
        Retry_Btn.onClick.AddListener(() =>
        {
            if (isReloadHotFix)
            {
                StartCoroutine(EntryView.I.IDoUpdateAddressable());
            }
            else
            {
                StartCoroutine(EntryView.I.IUpdateComplete());
            }

            gameObject.SetActive(false);
        });
    }
}
