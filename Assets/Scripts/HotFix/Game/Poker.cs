using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Poker : MonoBehaviour
{
    [SerializeField] SpriteRenderer Poker_Sr;
    [SerializeField] Animator Poker_Ani;
    [SerializeField] Coin _coin;
    [SerializeField] GameObject BlockMask_Obj;
    [SerializeField] GameObject CopyEffect_Obj;

    // 動畫Hash_是否中獎
    private readonly int _IsWin_Hash = Animator.StringToHash("IsWin");
    // 複製大鬼牌移動時間
    private readonly float _CopyBigWildMoveTime = 0.4f;

    // 當前顯示編號
    public int CurrNum { get; private set; }
    // 位置
    public int PosIndex { get; private set; }
    // 移動目標位置
    public Vector3 TargetPos { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="posIndex"></param>
    /// <param name="targetPos"></param>
    public void Initialize(int posIndex, Vector3 targetPos)
    {
        PosIndex = posIndex;
        TargetPos = targetPos;
    }

    /// <summary>
    /// 重製撲克牌
    /// </summary>
    public void ResetPoker()
    {
        CurrNum = -1;
        CopyEffect_Obj.SetActive(false);
        BlockMask_Obj.SetActive(false);
        Poker_Ani.SetBool(_IsWin_Hash, false);
        Poker_Sr.enabled = true;

        _coin.ResetCoin();
    }

    /// <summary>
    /// 輪轉重製撲克牌
    /// </summary>
    public void SlotResetPoker()
    {
        CopyEffect_Obj.SetActive(false);
        BlockMask_Obj.SetActive(false);
        Poker_Ani.SetBool(_IsWin_Hash, false);
    }

    /// <summary>
    /// 設置撲克牌型
    /// </summary>
    /// <param name="num"></param>
    /// <param name="isGold"></param>
    public void SetPokerNum(int num, bool isGold)
    {
        CurrNum = num;
        BlockMask_Obj.SetActive(false);

        if (num == 10)
        {
            // 金幣

            Poker_Sr.enabled = false;
            _coin.OpenCoin();
        }
        else
        {
            // 撲克

            Poker_Sr.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[num];
            Poker_Sr.color =
                isGold ?
                Color.yellow :
                Color.white;
        }        
    }

    /// <summary>
    /// 設置撲克與黃金牌轉牌
    /// </summary>
    /// <param name="num">牌型編號</param>
    /// <param name="isGold">是否為黃金牌</param>
    /// <param name="bigWildData">大鬼牌資料</param>
    public void SetPokerAndTurn(int num, bool isGold, BigWildData bigWildData)
    {
        CurrNum = num;

        if (num == 10)
        {
            // 金幣

            Poker_Sr.enabled = false;
            _coin.OpenCoin();
        }
        else if (num == 8 || num == 9)
        {
            // 黃金牌

            StartCoroutine(IGoldTurn(num, bigWildData));
        }
        else
        {
            // 一般牌

            Poker_Sr.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[num];
        }

        Poker_Sr.color =
            isGold ?
            Color.yellow :
            Color.white;
    }

    /// <summary>
    /// 掉落完成
    /// </summary>
    /// <param name="num">牌型編號</param>
    public void OnDropOver(int num)
    {
        if (num == 10)
        {
            // 金幣

            _coin.OnDropOver();
        }
    }

    /// <summary>
    /// 開啟金幣閃爍背景效果
    /// </summary>
    public void OpenCoinShineEffect()
    {
        _coin.OpenShineEffect();
    }

    /// <summary>
    /// 中獎
    /// </summary>
    public void Winning()
    {
        BlockMask_Obj.SetActive(false);
        Poker_Ani.SetBool(_IsWin_Hash, true);
    }

    /// <summary>
    /// 開啟黑色遮罩
    /// </summary>
    public void OpenBlockMask()
    {
        BlockMask_Obj.SetActive(true);
    }

    /// <summary>
    /// 黃金牌轉牌
    /// </summary>
    /// <param name="wildNum">鬼牌編號(8=小鬼排, 9=大鬼排)</param>
    /// <param name="bigWildData">大鬼牌資料</param>
    /// <returns></returns>
    private IEnumerator IGoldTurn(int wildNum, BigWildData bigWildData)
    {
        // 是複製的大鬼牌
        if (wildNum == 9 && (bigWildData == null || bigWildData.MainIndex != PosIndex))
        {
            yield break;
        }

        // 翻轉時間
        float during = 0.25f;

        DateTime startTime = DateTime.Now;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        while ((DateTime.Now - startTime).TotalSeconds < during)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / during;
            float rotY = Mathf.Lerp(0, 90, progress);
            transform.rotation = Quaternion.Euler(0, rotY, 0);

            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 90, 0);
        Poker_Sr.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[wildNum];

        startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < during)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / during;
            float rotY = Mathf.Lerp(90, 0, progress);
            transform.rotation = Quaternion.Euler(0, rotY, 0);

            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // 大鬼牌主牌
        if (bigWildData != null && bigWildData.MainIndex == PosIndex && wildNum == 9)
        {
            // 複製大鬼牌
            for (int i = 0; i < bigWildData.CopyPokerList.Count; i++)
            {
                GameObject copyObj = Instantiate(gameObject);
                copyObj.transform.position = transform.position;
                SpriteRenderer spriteRenderer = copyObj.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 15;
                copyObj.GetComponent<Poker>().enabled = false;
                Poker targetPoker = bigWildData.CopyPokerList[i];
                StartCoroutine(ICopyBigWildMovement(targetPoker, copyObj));
            }
        }
    }

    /// <summary>
    /// 複製大鬼牌移動至目標
    /// </summary>
    /// <param name="targetPoker"></param>
    /// <param name="copyObj"></param>
    /// <returns></returns>
    private IEnumerator ICopyBigWildMovement(Poker targetPoker, GameObject copyObj)
    {
        // 移動時間
        Vector3 startPos = copyObj.transform.position;
        DateTime startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalSeconds < _CopyBigWildMoveTime)
        {
            float progress = (float)(DateTime.Now - startTime).TotalSeconds / _CopyBigWildMoveTime;
            Vector3 newPos = Vector3.Lerp(startPos, targetPoker.transform.position, progress);
            copyObj.transform.position = newPos;
            yield return null;
        }

        StartCoroutine(targetPoker.IBigWildCopyEffect());
        Destroy(copyObj);
    }

    /// <summary>
    /// 大鬼牌複製效果
    /// </summary>
    /// <returns></returns>
    public IEnumerator IBigWildCopyEffect()
    {
        Poker_Sr.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[9];
        Poker_Sr.color = Color.white;
        CopyEffect_Obj.SetActive(true);

        yield return new WaitForSeconds(_CopyBigWildMoveTime);

        CopyEffect_Obj.SetActive(false);
    }
}
