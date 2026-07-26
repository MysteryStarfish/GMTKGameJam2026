using UnityEngine;
using DG.Tweening;

public class PotDropSequence : MonoBehaviour
{
    [Header("Anim Target Objects")]
    [SerializeField] private Transform potTransform;
    [SerializeField] private Transform ingredientTransform;
    [SerializeField] private Transform lidTransform;

    [Header("Settings")]
    [SerializeField] private float dropHeight = 10f; // 從多高掉下來
    [SerializeField] private float dropDuration = 0.6f;

    // 紀錄原本的位置（落地後的目標位置）
    private Vector3 _potTargetPos;
    private Vector3 _ingredientTargetPos;
    private Vector3 _lidTargetPos;

    private void Awake()
    {
        // 1. 記錄 Inspector 中排好的初始位置
        _potTargetPos = potTransform.position;
        _ingredientTargetPos = ingredientTransform.position;
        _lidTargetPos = lidTransform.position;
    }

    [ContextMenu("Play Drop Animation")] // 可以在 Editor 模式下右鍵測試
    public void PlayAnimation()
    {
        // 防呆與初始化：先停止舊動畫，並將物件推到上方天空
        DOTween.KillAll();
        ResetPositions();

        // 建立主序列
        Sequence seq = DOTween.Sequence();

        // ------------------------------------
        // Step 1: 大鍋子從天而降 (Bounce 彈跳落下來)
        // ------------------------------------
        seq.Append(potTransform.DOMoveY(_potTargetPos.y, dropDuration).SetEase(Ease.OutBounce));
        
        // 鍋子落地時，加一個微小的落地擠壓動畫（扁一下再恢復，增強打擊感）
        seq.Append(potTransform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        seq.Append(potTransform.DOScale(Vector3.one, 0.1f));

        // ------------------------------------
        // Step 2: 食材掉落入鍋
        // ------------------------------------
        // 連接下一個動作，並稍微延遲一點點
        seq.AppendInterval(0.1f); 
        seq.AppendCallback(() => ingredientTransform.gameObject.SetActive(true));
        
        // 食材掉落（使用 Quad/Cubic 快落速度）
        seq.Append(ingredientTransform.DOMoveY(_ingredientTargetPos.y, 0.4f).SetEase(Ease.InQuad));
        
        // 食材進鍋瞬間：讓鍋子稍微晃動一下 (Punch Scale)
        seq.Append(potTransform.DOPunchScale(new Vector3(0.1f, -0.1f, 0f), 0.15f));

        // ------------------------------------
        // Step 3: 鍋蓋掉落蓋上
        // ------------------------------------
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() => lidTransform.gameObject.SetActive(true));
        
        // 鍋盖掉落
        seq.Append(lidTransform.DOMoveY(_lidTargetPos.y, 0.35f).SetEase(Ease.InCubic));
        
        // 蓋上瞬間：整個鍋子組小幅地震動，完美收尾
        seq.Append(potTransform.DOShakePosition(0.2f, strength: 0.2f, vibrato: 10));

        // 動畫完成回呼 (可在此接續遊戲邏輯，例如開始倒數或允許玩家操作)
        seq.OnComplete(() =>
        {
            Debug.Log("[Animation] 鍋子入場動畫播放完畢！");
        });
    }

    private void ResetPositions()
    {
        // 隱藏食材與鍋蓋，並把所有物件推高到天空預備
        potTransform.position = _potTargetPos + Vector3.up * dropHeight;
        
        ingredientTransform.position = _ingredientTargetPos + Vector3.up * dropHeight;
        ingredientTransform.gameObject.SetActive(false);

        lidTransform.position = _lidTargetPos + Vector3.up * dropHeight;
        lidTransform.gameObject.SetActive(false);

        potTransform.localScale = Vector3.one;
    }
}