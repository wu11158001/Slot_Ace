using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] GameObject Coin_Obj;
    [SerializeField] SpriteRenderer Coin_Sr;
    [SerializeField] Animator Coin_Ani;

    // 動畫Hash_掉落完成
    private readonly int _DropOver_Hash = Animator.StringToHash("DropOver");

    /// <summary>
    /// 重製金幣
    /// </summary>
    public void ResetCoin()
    {
        Coin_Obj.SetActive(false);
    }

    /// <summary>
    /// 開啟金幣
    /// </summary>
    public void OpenCoin()
    {
        Coin_Obj.SetActive(true);
    }

    /// <summary>
    /// 掉落完成
    /// </summary>
    public void OnDropOver()
    {
        Coin_Ani.SetBool(_DropOver_Hash, true);
    }
}
