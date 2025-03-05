using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game_View : MonoBehaviour
{
    [SerializeField] Transform PokerArea;
    [SerializeField] Transform TwoCoinColumnEffectArea;

    // 輪轉開始位置增加Y
    private float slotStartPositionAddY = 9.4f;

    private GameMVC _gameMVC;

    // 左上撲克牌起始位置
    private Vector3 pokerStartPosition = new(-3.2f, -2.8f, 0);
    // 撲克牌間距
    private Vector2 pokerSpace = new(1.65f, 1.9f);

    // 場上撲克牌
    private List<Poker> _pokerList;
    // 產生2枚金幣出現效果
    private List<GameObject> _twoCoinColumnEffectList;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize(GameMVC gameMVC)
    {
        _gameMVC = gameMVC;
        _pokerList = new();
        _twoCoinColumnEffectList = new();

        // 產生撲克牌
        Addressables.LoadAssetAsync<GameObject>("Prefab/Game/Poker.prefab").Completed += (handle) =>
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    float posX = pokerStartPosition.x + (row * pokerSpace.x);
                    float posY = pokerStartPosition.y + (col * pokerSpace.y);
                    Vector3 targetPos = new(posX, posY, 0);

                    GameObject pokerObj = Instantiate(handle.Result, PokerArea);
                    pokerObj.transform.position = targetPos;
                    Poker poker = pokerObj.GetComponent<Poker>();
                    poker.Initialize(
                        col + (row * 4),
                        targetPos);

                    _pokerList.Add(poker);
                }
            }

            FirstSlot();

            Addressables.Release(handle);
        };

        // 產生2枚金幣出現效果
        Addressables.LoadAssetAsync<GameObject>("Prefab/Game/TwoCoinColumnEffect.prefab").Completed += (handle) =>
        {
            for (int row = 0; row < 5; row++)
            {
                float posX = pokerStartPosition.x + (row * pokerSpace.x);

                GameObject twoCoinColumnEffect = Instantiate(handle.Result, TwoCoinColumnEffectArea);
                twoCoinColumnEffect.transform.position = new Vector3(posX, -0.3f, 0);
                twoCoinColumnEffect.SetActive(false);
                _twoCoinColumnEffectList.Add(twoCoinColumnEffect);
            }
        };
    }

    /// <summary>
    /// 首次輪轉(初始畫面)
    /// </summary>
    private void FirstSlot()
    {
        // 初始牌面
        SlotResultData slotResultData = new()
        {
            // 盤面結果
            SlotCardNumList = new()
            {
                new()
                {
                    7,7,7,7,
                    6,6,6,6,
                    5,5,5,5,
                    4,4,4,4,
                    3,3,3,3,
                },
            },

            // 黃金牌位置
            GoldCardIndexList = new()
            {
                new() { 7 },
            },
        };
        StartSlot(slotResultData, true);
    }

    /// <summary>
    /// 開始輪轉
    /// </summary>
    /// <param name="slotResultData"></param>
    /// <param name="isInit">初始輪轉</param>
    public void StartSlot(SlotResultData slotResultData, bool isInit = false)
    {
        // 首輪輪轉
        foreach (var poker in _pokerList)
        {
            float slotPosY = poker.TargetPos.y + slotStartPositionAddY;
            poker.gameObject.transform.position = new(poker.TargetPos.x, slotPosY, 0);
        }

        StartCoroutine(IStartSlotEffect(slotResultData, isInit));
    }

    /// <summary>
    /// 開始輪轉效果
    /// </summary>
    /// <param name="slotResultData"></param>
    /// <param name="isInit"></param>
    /// <returns></returns>
    private IEnumerator IStartSlotEffect(SlotResultData slotResultData, bool isInit)
    {
        // 已出現金幣數量
        int coinCount = 0;

        // 重製撲克牌
        foreach (var poker in _pokerList)
        {
            poker.ResetPoker();
        }

        for (int i = 0; i < slotResultData.SlotCardNumList.Count; i++)
        {
            // 一般輪轉撲克牌等待時間
            float normalYieldTime = 0.01f;
            // 2枚金幣出現輪轉撲克牌等待時間
            float twoCoinTieldTime = 0.25f;

            // 是否有大鬼牌
            bool hasBigWild = false;

            // 重製撲克牌
            foreach (var poker in _pokerList)
            {
                poker.SlotResetPoker();
            }

            // 輪轉效果
            int index = 0;
            int column = 0;
            foreach (var poker in _pokerList)
            {
                // 每列首個撲克掉落
                if (index > 0 && index % 4 == 0)
                {
                    column++;
                }

                // 設置撲克牌
                int pokerNum = slotResultData.SlotCardNumList[i][index];
                bool isGold = slotResultData.GoldCardIndexList[i].Contains(index);
                BigWildData bigWildData = new();
                if (pokerNum == 9)
                {
                    // 大鬼牌

                    bigWildData = slotResultData.bigWildDataList[i][index];
                    if (bigWildData != null && bigWildData.CopyIndexList != null)
                    {
                        hasBigWild = true;
                        bigWildData.CopyPokerList = new();
                        for (int copyIndex = 0; copyIndex < bigWildData.CopyIndexList.Count; copyIndex++)
                        {
                            bigWildData.CopyPokerList.Add(_pokerList[bigWildData.CopyIndexList[copyIndex]]);
                        }
                    }                                      
                }
                else if (poker.CurrNum != 10 && pokerNum == 10)
                {
                    // 金幣

                    coinCount++;                    
                }

                poker.SetPokerAndTurn(pokerNum, isGold, bigWildData);
                if (coinCount >= 2)
                {
                    foreach (var tempPoker in _pokerList)
                    {
                        if (tempPoker.CurrNum == 10)
                        {
                            tempPoker.OpenCoinShineEffect();
                        }
                    }
                }

                StartCoroutine(ISlotEffect(poker, pokerNum));

                if (i == 0 && coinCount == 2)
                {
                    // 金幣數量差1枚(首輪輪轉)

                    yield return new WaitForSeconds(twoCoinTieldTime);
                }
                else
                {
                    // 一般輪轉掉落

                    yield return new WaitForSeconds(normalYieldTime);
                }

                // 每列首個撲克掉落
                if (index > 0 && index % 4 == 0)
                {
                    // 首輪輪轉
                    if (i == 0)
                    {
                        // 重製兩枚金幣出現效果
                        ResetTwoCoinColumnEffect();

                        // 前幾列撲克牌開啟黑色遮罩
                        if (coinCount == 2)
                        {
                            int openBlockMaskCount = column * 4;
                            for (int blockIndex = 0; blockIndex < openBlockMaskCount; blockIndex++)
                            {
                                if (_pokerList[blockIndex].CurrNum != 10)
                                {
                                    _pokerList[blockIndex].OpenBlockMask();
                                }
                            }
                        }
                    }                                  
                }

                // 開啟兩枚金幣效果(首輪輪轉)
                if (i == 0 && coinCount == 2)
                {
                    _twoCoinColumnEffectList[column].SetActive(true);
                }

                index++;
            }

            // 等待輪轉效果結束
            bool isFinishSlot = _pokerList.All(x => x.gameObject.transform.position == x.TargetPos);
            while (!isFinishSlot)
            {
                isFinishSlot = _pokerList.All(x => x.gameObject.transform.position == x.TargetPos);
                yield return null;
            }

            // 有大鬼牌
            if (hasBigWild)
            {
                yield return new WaitForSeconds(1.0f);
            }

            // 重製兩枚金幣出現效果
            ResetTwoCoinColumnEffect();

            // 再次設置撲克牌牌面確保顯示資料正確
            index = 0;
            foreach (var poker in _pokerList)
            {
                // 設置撲克牌
                int pokerNum = slotResultData.SlotCardNumList[i][index];
                bool isGold = slotResultData.GoldCardIndexList[i].Contains(index);                
                poker.SetPokerNum(pokerNum, isGold);
                index++;
            }

            // 中獎牌效果
            if (slotResultData.WinCardPosList != null)
            {
                if (slotResultData.WinCardPosList[i].Count > 0)
                {
                    // 有中獎預設所有牌未中獎效果
                    foreach (var poker in _pokerList)
                    {
                        if (poker.CurrNum != 10)
                        {
                            poker.OpenBlockMask();
                        }                        
                    }

                    // 設置連擊數UI
                    _gameMVC.gameControlView.SetComboPanelText(i);
                }

                // 中獎牌效果
                foreach (var winPos in slotResultData.WinCardPosList[i])
                {
                    Poker winPoker = _pokerList.Where(x => x.PosIndex == winPos).FirstOrDefault();
                    if (winPoker) winPoker.Winning();
                }
            }

            if (!isInit)
            {
                if (slotResultData.WinCardPosList != null && slotResultData.WinCardPosList[i].Count > 0)
                {
                    yield return new WaitForSeconds(2.0f);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            // 中獎牌位置重製(非黃金牌)
            if (slotResultData.WinCardPosList != null)
            {
                foreach (var winPos in slotResultData.WinCardPosList[i])
                {
                    Poker winPoker = _pokerList.Where(x => x.PosIndex == winPos).FirstOrDefault();
                    if (winPoker)
                    {
                        if (!slotResultData.GoldCardIndexList[i].Contains(winPos))
                        {
                            // 一般牌
                            float slotPosY = winPoker.TargetPos.y + slotStartPositionAddY;
                            winPoker.gameObject.transform.position = new(winPoker.TargetPos.x, slotPosY, 0);
                        }                     
                    }
                }
            }
        }

        _gameMVC.gameControlView.OpenOperation();
    }

    /// <summary>
    /// 輪轉效果
    /// </summary>
    /// <param name="poker"></param>
    /// <param name="num">牌型編號</param>
    /// <returns></returns>
    private IEnumerator ISlotEffect(Poker poker, int num)
    {
        float slotMoveSpeed = 30;

        Transform tr = poker.gameObject.transform;

        while (tr.position.y > poker.TargetPos.y)
        {
            tr.Translate(Vector3.down * slotMoveSpeed * Time.deltaTime, Space.World);

            if (tr.position.y <= poker.TargetPos.y)
            {
                tr.position = poker.TargetPos;
            }

            yield return null;
        }

        tr.position = poker.TargetPos;
        poker.OnDropOver(num);
    }

    /// <summary>
    /// 重製兩枚金幣出現效果
    /// </summary>
    private void ResetTwoCoinColumnEffect()
    {        
        foreach (var twoCoinColumnEffect in _twoCoinColumnEffectList)
        {
            twoCoinColumnEffect.SetActive(false);
        }
    }
}
