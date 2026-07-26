using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Script.Core.Ingredients;
using Random = UnityEngine.Random;

public class CookingEntranceAnimation : MonoBehaviour
{
    [Header("UI 主體元素")]
    [SerializeField] private RectTransform potOuterRect;     // 外鍋
    [SerializeField] private RectTransform potInnerRect;     // 內層 (鍋內)
    [SerializeField] private RectTransform lidRect;          // 鍋蓋

    private Image[] _ingredientImages;        // 6 個食材 Image 元件

    [Header("落點與高度設定")]
    [SerializeField] private float dropHeight = 800f;
    [SerializeField] private float randomDropRadius = 80f; // 🌟 改成單一數字 (隨機半徑)
    [SerializeField] private Vector2 potTargetPos;           // 鍋子目標 AnchoredPosition
    [SerializeField] private Vector2 lidTargetPos;           // 鍋蓋目標 AnchoredPosition

    [Header("動畫時間設定")]
    [SerializeField] private float potDropDuration = 0.5f;
    [SerializeField] private float ingredientDropDuration = 0.3f;
    [SerializeField] private float ingredientInterval = 0.07f; // 食材間的掉落時間差
    [SerializeField] private float lidDropDuration = 0.35f;

    [Header("音效 (可選)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip potSFX;
    [SerializeField] private AudioClip ingredientSFX;
    [SerializeField] private AudioClip lidSFX;

    private Sequence _entranceSequence;
    private Vector2[] _originalIngredientPositions;

    private void Awake()
    {
        
    }
    
    private void SetUp(Image[] ingredientImages)
    {
        _ingredientImages = ingredientImages;
        // 記錄 Inspector 排版時的原始食材相對位置
        if (_ingredientImages != null && _ingredientImages.Length > 0)
        {
            _originalIngredientPositions = new Vector2[_ingredientImages.Length];
            for (int i = 0; i < _ingredientImages.Length; i++)
            {
                if (_ingredientImages[i] != null)
                {
                    _originalIngredientPositions[i] = _ingredientImages[i].rectTransform.anchoredPosition;
                }
            }
        }
    }

    /// <summary>
    /// 設置 6 個食材的 Sprite
    /// </summary>
    public void SetIngredientSprites(List<Ingredient> ingredients)
    {
        if (ingredients == null) return;

        for (int i = 0; i < _ingredientImages.Length; i++)
        {
            var sprite = ingredients[i].ResultSprite;
            if (_ingredientImages[i] == null) continue;

            if (sprite != null)
            {
                _ingredientImages[i].sprite = sprite;
                _ingredientImages[i].gameObject.SetActive(true);
            }
            else
            {
                _ingredientImages[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 播放完整登場動畫
    /// </summary>
    [ContextMenu("Play Animation")]
    public void PlayEntranceAnimation(Action callback = null)
    {
        // 1. 清理舊 Tween 防錯
        KillAllTweens();

        _entranceSequence = DOTween.Sequence();

        // --------------------------------------------------
        // 第一階段：外鍋 + 內層 一起從天而降至指定位置
        // --------------------------------------------------
        // 將外鍋與內層歸位至指定座標
        potOuterRect.anchoredPosition = potTargetPos;
        potInnerRect.anchoredPosition = potTargetPos;

        // 兩物件同步下落
        _entranceSequence.Append(
            potOuterRect.DOAnchorPosY(potTargetPos.y + dropHeight, potDropDuration)
                        .From()
                        .SetEase(Ease.OutBounce)
                        .OnComplete(() => PlaySFX(potSFX))
        );
        _entranceSequence.Join(
            potInnerRect.DOAnchorPosY(potTargetPos.y + dropHeight, potDropDuration)
                        .From()
                        .SetEase(Ease.OutBounce)
        );

        // 落地後的果凍 Q 彈感 (兩層同步擠壓)
        _entranceSequence.Append(
            potOuterRect.DOScale(new Vector3(1.08f, 0.88f, 1f), 0.08f).SetLoops(2, LoopType.Yoyo)
        );
        _entranceSequence.Join(
            potInnerRect.DOScale(new Vector3(1.08f, 0.88f, 1f), 0.08f).SetLoops(2, LoopType.Yoyo)
        );

        // --------------------------------------------------
        // 第二階段：6 個食材散落進隨機範圍位置
        // --------------------------------------------------
        Sequence ingredientSeq = DOTween.Sequence();

        for (int i = 0; i < _ingredientImages.Length; i++)
        {
            if (_ingredientImages[i] == null || !_ingredientImages[i].gameObject.activeSelf) continue;

            RectTransform ingRect = _ingredientImages[i].rectTransform;

            // 計算隨機落點 (在原本位置的基礎上做 random offset)
            Vector2 randomOffset = Random.insideUnitCircle * randomDropRadius;
            Vector2 targetLandingPos = _originalIngredientPositions[i] + randomOffset;
            ingRect.anchoredPosition = targetLandingPos;

            float delay = i * ingredientInterval;

            // 用 Insert 創造連續流暢的交錯掉落
            ingredientSeq.Insert(delay,
                ingRect.DOAnchorPosY(targetLandingPos.y + dropHeight, ingredientDropDuration)
                       .From()
                       .SetEase(Ease.InQuad)
                       .OnComplete(() =>
                       {
                           PlaySFX(ingredientSFX);
                           // 食材砸中時外鍋微震
                           potOuterRect.DOShakePosition(0.06f, new Vector3(0, 6f, 0), vibrato: 5);
                       })
            );
        }

        _entranceSequence.Append(ingredientSeq);

        // --------------------------------------------------
        // 第三階段：鍋蓋掉落至指定位置
        // --------------------------------------------------
        lidRect.anchoredPosition = lidTargetPos;

        _entranceSequence.Append(
            lidRect.DOAnchorPosY(lidTargetPos.y + dropHeight, lidDropDuration)
                   .From()
                   .SetEase(Ease.OutBack)
                   .OnComplete(() => PlaySFX(lidSFX))
        );

        // 鍋蓋砸上後的最終整體大晃動
        _entranceSequence.Append(
            potOuterRect.DOShakePosition(0.2f, new Vector3(10f, 10f, 0), vibrato: 15)
        );
        
        _entranceSequence.OnComplete(() => callback?.Invoke());
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void KillAllTweens()
    {
        // 1. 殺掉 Sequence
        _entranceSequence?.Kill();

        // 2. RectTransform 防呆清理
        if (potOuterRect != null) potOuterRect.DOKill();
        if (potInnerRect != null) potInnerRect.DOKill();
        if (lidRect != null) lidRect.DOKill();

        // 3. 陣列內部元素與 RectTransform 防呆清理 (🌟 最容易在此爆 Null)
        if (_ingredientImages != null)
        {
            foreach (var img in _ingredientImages)
            {
                // 必須先判斷 img 不是 null，才能存取它的 rectTransform！
                if (img != null && img.rectTransform != null)
                {
                    img.rectTransform.DOKill();
                }
            }
        }
    }

    private void OnDisable()
    {
        KillAllTweens();
    }
    
    [Header("Gizmos 偵錯設定")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0.5f, 0.35f); // 預設半透明青綠色

    // --------------------------------------------------
    // 🎨 Gizmos 視覺化繪製
    // --------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // 決定 Gizmos 的中心點（優先使用外鍋位置，若未指定則使用自身物件位置）
        Transform targetTransform = potOuterRect != null ? potOuterRect.transform : transform;

        // 保存當前的 Gizmos 矩陣與顏色
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Color oldColor = Gizmos.color;

        // 將 Gizmos 座標系變換至目標 UI 物件的世界變換矩陣 (支援旋轉與縮放)
        Gizmos.matrix = targetTransform.localToWorldMatrix;
        
        // 1. 繪製半透明填充圓形（半徑範圍）
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(Vector3.zero, randomDropRadius);

        // 2. 繪製外框線，讓邊界更清晰
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.9f);
        Gizmos.DrawWireSphere(Vector3.zero, randomDropRadius);

        /* 💡 如果你使用的是 Vector2 randomDropRange (橢圓形範圍)，請用這段替換上面的 DrawSphere：

        Vector3 rangeSize = new Vector3(randomDropRange.x * 2f, randomDropRange.y * 2f, 1f);
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(Vector3.zero, rangeSize);

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.9f);
        Gizmos.DrawWireCube(Vector3.zero, rangeSize);
        */

        // 還原 Gizmos 狀態
        Gizmos.matrix = oldMatrix;
        Gizmos.color = oldColor;
    }
}