using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] GameObject Coin_Obj;
    [SerializeField] SpriteRenderer Coin_Sr;
    [SerializeField] Animator Coin_Ani;
    [SerializeField] GameObject ShineEffect_Obj;

    // 動畫Hash_掉落完成
    private readonly int _DropOver_Hash = Animator.StringToHash("IsDropOver");
    // 動畫Hash_中獎
    private readonly int _IsWin_Hash = Animator.StringToHash("IsWin");

    /// <summary>
    /// 重製金幣
    /// </summary>
    public void ResetCoin()
    {
        Coin_Obj.SetActive(false);
        ShineEffect_Obj.SetActive(false);
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

    /// <summary>
    /// 開啟閃爍背景效果
    /// </summary>
    public void OpenShineEffect()
    {
        ShineEffect_Obj.SetActive(true);
    }
}
