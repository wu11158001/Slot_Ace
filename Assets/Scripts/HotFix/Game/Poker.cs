using UnityEngine;

public class Poker : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 設置撲克
    /// </summary>
    /// <param name="num">牌型編號</param>
    /// <param name="isGold">是否為黃金牌</param>
    public void SetPoker(int num, bool isGold)
    {
        _spriteRenderer.sprite = AssetsManager.I.SOManager.PokerSprite_SO.SpriteList[num];
        _spriteRenderer.color =
            isGold ?
            Color.yellow :
            Color.white;
    }
}
