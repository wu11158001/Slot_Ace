using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Poker : MonoBehaviour
{
    [SerializeField] GameObject NotWin_Obj;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    // 動畫Hash_是否中獎
    private readonly int _IsWin_Hash = Animator.StringToHash("IsWin");
    // 複製大鬼牌移動時間
    private readonly float _CopyBigWildMoveTime = 0.4f;

    // 位置
    public int PosIndex { get; private set; }
    // 移動目標位置
    public Vector3 TargetPos { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

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
    /// 設置撲克牌型
    /// </summary>
    /// <param name="num"></param>
    /// <param name="isGold"></param>
    public void SetPokerNum(int num, bool isGold)
    {
        _spriteRenderer.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[num];
        _spriteRenderer.color =
            isGold ?
            Color.yellow :
            Color.white;
    }

    /// <summary>
    /// 設置撲克與黃金牌轉牌
    /// </summary>
    /// <param name="num">牌型編號</param>
    /// <param name="isGold">是否為黃金牌</param>
    /// <param name="bigWildData">大鬼牌資料</param>
    public void SetPokerAndTurn(int num, bool isGold, BigWildData bigWildData)
    {       
        if (num == 8 || num == 9)
        {
            // 黃金牌
            StartCoroutine(IGoldTurn(num, bigWildData));
        }
        else
        {
            // 一般牌

            _spriteRenderer.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[num];
        }

        _spriteRenderer.color =
            isGold ?
            Color.yellow :
            Color.white;
    }

    /// <summary>
    /// 重製撲克牌
    /// </summary>
    public void ResetPoker()
    {
        NotWin_Obj.SetActive(false);
        _animator.SetBool(_IsWin_Hash, false);
    }

    /// <summary>
    /// 中獎
    /// </summary>
    public void Winning()
    {
        NotWin_Obj.SetActive(false);
        _animator.SetBool(_IsWin_Hash, true);
    }

    /// <summary>
    /// 未中獎
    /// </summary>
    public void NotWin()
    {
        NotWin_Obj.SetActive(true);
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
        _spriteRenderer.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[wildNum];

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
            IBigWildTurnEffect();

            // 複製大鬼牌
            for (int i = 0; i < bigWildData.CopyPokerList.Count; i++)
            {
                GameObject copyObj = Instantiate(gameObject);
                copyObj.transform.position = transform.position;
                SpriteRenderer spriteRenderer = copyObj.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 10;
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

        targetPoker.IBigWildTurnEffect();
        Destroy(copyObj);
    }

    /// <summary>
    /// 大鬼牌轉牌
    /// </summary>
    /// <returns></returns>
    public void IBigWildTurnEffect()
    {
        _spriteRenderer.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[9];
    }
}
