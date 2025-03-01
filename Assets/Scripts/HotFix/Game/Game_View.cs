using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game_View : MonoBehaviour
{
    [SerializeField] Transform PokerArea;

    [Space(30)]
    [Header("輪轉效果參數")]
    [SerializeField] float slotStartPositionAddY;
    [SerializeField] float slotYieldTime;
    [SerializeField] float slotMoveSpeed;

    private GameMVC _gameMVC;

    // 左上撲克牌起始位置
    private Vector3 pokerStartPosition = new(-3.2f, -2.8f, 0);
    // 撲克牌間距
    private Vector2 pokerSpace = new(1.65f, 1.9f);

    // 場上撲克牌
    private List<Poker> _pokerList;

    private void Awake()
    {
        _gameMVC = GetComponent<GameMVC>();

        CreatePokers();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void CreatePokers()
    {
        _pokerList = new();

        // 產生撲克牌
        Addressables.LoadAssetAsync<GameObject>("Prefab/Game/Poker.prefab").Completed += (assets) =>
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    float posX = pokerStartPosition.x + (row * pokerSpace.x);
                    float posY = pokerStartPosition.y + (col * pokerSpace.y);
                    Vector3 targetPos = new(posX, posY, 0);

                    GameObject pokerObj = Instantiate(assets.Result, PokerArea);
                    pokerObj.transform.position = targetPos;
                    Poker poker = pokerObj.GetComponent<Poker>();
                    poker.Initialize(
                        col + (row * 4),
                        targetPos);

                    _pokerList.Add(poker);
                }
            }

            // 初始牌面
            SlotResultData slotResultData = new()
            {
                // 盤面結果
                SlotCardNumList = new()
                {
                    new()
                    {
                        7, 7, 7, 7,
                        6, 6, 6, 6,
                        5, 5, 5, 5,
                        4, 4, 4, 4,
                        3, 3, 3, 3,
                    },
                },

                // 黃金牌位置
                GoldCardIndexList = new()
                {
                    new() { 7 },
                },
            };
            StartSlot(slotResultData, true);
        };
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
        for (int i = 0; i < slotResultData.SlotCardNumList.Count; i++)
        {
            // 是否有大鬼牌
            bool hasBigWild = false;

            // 重製撲克牌
            foreach (var poker in _pokerList)
            {
                poker.ResetPoker();
            }

            // 輪轉效果
            int index = 0;
            foreach (var poker in _pokerList)
            {
                // 設置撲克牌
                int pokerNum = slotResultData.SlotCardNumList[i][index];
                bool isGold = slotResultData.GoldCardIndexList[i].Contains(index);
                BigWildData bigWildData = new();
                if (pokerNum == 9)
                {
                    // 大鬼牌
                    bigWildData = slotResultData.bigWildDataList.Where(x => x.MainIndex == index).FirstOrDefault();
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

                poker.SetPokerAndTurn(pokerNum, isGold, bigWildData);

                StartCoroutine(ISlotEffect(poker));
                yield return new WaitForSeconds(slotYieldTime);

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
                yield return new WaitForSeconds(0.45f);
            }

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
                        poker.NotWin();
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
                    yield return new WaitForSeconds(2);
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
    /// <param name="tr">物件</param>
    /// <param name="targetPos">目標位置</param>
    /// <returns></returns>
    private IEnumerator ISlotEffect(Poker poker)
    {
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
    }
}
