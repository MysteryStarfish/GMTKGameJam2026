using System;
using DG.Tweening;
using Script.Core.Ingredients;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace Script.Core.Pots
{
    public class PotAnimation : MonoBehaviour
    {
        [SerializeField] private float dropHeight = 150f; // 注意：UI RectTransform 的位移通常是 100~200 px，1.2f 太小了！
        [SerializeField] private float dropDuration = 0.3f;
        [SerializeField] private Ease dropEase = Ease.OutQuad;

        public void PlayPutIngredientAnimation(Ingredient ingredient, Image ingredientImage, Image potImage)
        {
            if (ingredientImage == null || ingredient == null) return;

            ingredientImage.sprite = ingredient.Sprite;
            RectTransform rect = ingredientImage.rectTransform;

            rect.DOKill();
            ingredientImage.DOKill();

            Vector2 targetPos = Vector2.zero;
            rect.anchoredPosition = targetPos + new Vector2(0f, dropHeight);

            ingredientImage.gameObject.SetActive(true);
            ingredientImage.color = new Color(1f, 1f, 1f, 0f); // 初始全透明
            rect.localScale = Vector3.one * 1.2f;

            Sequence seq = DOTween.Sequence();

            // 淡入 + 下落 + 縮放 同時進行
            seq.Append(ingredientImage.DOFade(1f, dropDuration * 0.4f));
            seq.Join(rect.DOAnchorPos(targetPos, dropDuration).SetEase(dropEase));
            seq.Join(rect.DOScale(Vector3.one, dropDuration).SetEase(Ease.OutQuad));

            if (potImage != null)
            {
                seq.Append(potImage.rectTransform.DOPunchScale(new Vector3(0.1f, -0.1f, 0f), 0.15f));
            }
        }
        
        public void PlayTakeIngredientAnimation(Ingredient ingredient, Image ingredientImage, Image potImage, Action after)
        {
            if (ingredientImage == null)
            {
                after?.Invoke();
                return;
            }

            ingredientImage.sprite = ingredient.Sprite;
            RectTransform rect = ingredientImage.rectTransform;

            // 1. 防呆：殺掉舊動畫並重置狀態
            rect.DOKill();
            ingredientImage.DOKill();

            rect.anchoredPosition = Vector2.zero;
            ingredientImage.color = Color.white;
            rect.localScale = Vector3.one;
            ingredientImage.gameObject.SetActive(true);

            // 目標：往上飛高 dropHeight 的距離
            Vector2 targetPos = new Vector2(0f, dropHeight); 
            float takeDuration = 0.35f; // 取出動畫乾淨俐落，建議 0.3~0.35s 即可

            Sequence seq = DOTween.Sequence();

            // 🌟 鍋子壓一下反饋
            if (potImage != null)
            {
                seq.Append(potImage.rectTransform.DOPunchScale(new Vector3(0.1f, -0.1f, 0f), 0.15f));
            }
            
            // 🌟 核心修正：將「往上飛」、「淡出 (Fade Out)」、「放大」全部用 Join 綁在一起同時發生！
            seq.Join(rect.DOAnchorPos(targetPos, takeDuration).SetEase(Ease.OutCubic));
            seq.Join(ingredientImage.DOFade(0f, takeDuration).SetEase(Ease.InQuad));
            seq.Join(rect.DOScale(Vector3.one * 1.2f, takeDuration).SetEase(Ease.OutQuad));

            // 動畫播完後關閉 Image 並觸發完成事件
            seq.OnComplete(() =>
            {
                ingredientImage.gameObject.SetActive(false);
                rect.anchoredPosition = Vector2.zero; // 歸位供下次使用
                after?.Invoke();
            });
        }
    }
}