using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EntryPokerEventReceiver : MonoBehaviour
{
    [SerializeField] Image Poker0_Img;
    [SerializeField] Image Poker1_Img;
    [SerializeField] List<Sprite> PokerSpriteList;

    // 當前撲克牌動畫顯示牌面編號
    private int _currPokerAniIndex;

    private void Awake()
    {
        Poker1_Img.sprite = PokerSpriteList[0];
    }

    /// <summary>
    /// 撲克牌動畫更換牌
    /// </summary>
    public void PokerChangeAnimation()
    {
        _currPokerAniIndex = (_currPokerAniIndex + 1) % PokerSpriteList.Count;

        Poker0_Img.sprite = PokerSpriteList[_currPokerAniIndex];
        Poker1_Img.sprite = PokerSpriteList[_currPokerAniIndex];
    }
}
