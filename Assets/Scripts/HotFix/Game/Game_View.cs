using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;

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

    // 場上撲克牌(撲克牌, 移動目標位置)
    private Dictionary<Poker, Vector3> _poker_Dic;

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
        _poker_Dic = new();

        // 產生撲克牌
        Addressables.LoadAssetAsync<GameObject>("Prefab/Game/Poker.prefab").Completed += (assets) =>
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    float posX = pokerStartPosition.x + (row * pokerSpace.x);
                    float posY = pokerStartPosition.y + (col * pokerSpace.y);
                    Vector3 pos = new(posX, posY, 0);

                    GameObject pokerObj = Instantiate(assets.Result, PokerArea);
                    pokerObj.transform.position = pos;
                    Poker poker = pokerObj.AddComponent<Poker>();

                    _poker_Dic.Add(poker, pos);
                }
            }

            // 初始牌面
            List<List<int>> initCards = new()
            {
                new()
                {
                    7, 7, 7, 7,
                    6, 6, 6, 6,
                    5, 5, 5, 5,
                    4, 4, 4, 4,
                    3, 3, 3, 3,
                },
            };
            StartSlot(initCards);
        };
    }

    /// <summary>
    /// 開始輪轉
    /// </summary>
    /// <param name="slotResultList"></param>
    public void StartSlot(List<List<int>> slotResultList)
    {
        // 輪轉初始化
        int index = 0;
        foreach (var poker in _poker_Dic)
        {
            float slotPosY = poker.Value.y + slotStartPositionAddY;
            int pokerNum = slotResultList[0][index];

            poker.Key.gameObject.transform.position = new(poker.Value.x, slotPosY, 0);
            poker.Key.SetPoker(pokerNum, false);

            index++;
        }

        StartCoroutine(IStartSlotEffect());
    }

    /// <summary>
    /// 開始輪轉效果
    /// </summary>
    /// <returns></returns>
    private IEnumerator IStartSlotEffect()
    {
        foreach (var item in _poker_Dic)
        {
            StartCoroutine(ISlotEffect(item.Key.gameObject.transform, item.Value));
            yield return new WaitForSeconds(slotYieldTime);
        }
    }

    /// <summary>
    /// 輪轉效果
    /// </summary>
    /// <param name="tr">物件</param>
    /// <param name="targetPos">目標位置</param>
    /// <returns></returns>
    private IEnumerator ISlotEffect(Transform tr, Vector3 targetPos)
    {
        while (tr.position.y > targetPos.y)
        {
            tr.Translate(Vector3.down * slotMoveSpeed * Time.deltaTime, Space.World);

            if (tr.position.y <= targetPos.y)
            {
                tr.position = targetPos;
            }

            yield return null;
        }

        tr.position = targetPos;
    }
}
