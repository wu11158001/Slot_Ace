using UnityEngine;

public class Poker : MonoBehaviour
{
    [SerializeField] GameObject NotWin_Obj;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    // 動畫Hash_是否中獎
    private readonly int _IsWin_Hash = Animator.StringToHash("IsWin");

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
}
