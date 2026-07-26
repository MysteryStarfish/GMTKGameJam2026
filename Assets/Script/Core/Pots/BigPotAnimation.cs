using DG.Tweening;
using Script.Core.Ingredients;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Core.Pots
{
    public class BigPotAnimation : MonoBehaviour
    {
        [Header("食材 UI 列表")]
        private Ingredient[] ingredients;
        [SerializeField] private Image[] ingredientRects;

        [Header("隨機範圍設定")]
        [SerializeField] private float spawnRadius = 80f; // 圓形範圍半徑 (像素)
        [SerializeField] private float maxRotationAngle = 30f;
        [SerializeField] private bool useAnimation = true; // 是否播放落入動畫
        [SerializeField] private float dropHeight = 200f; // 動畫落下的高度
        [SerializeField] private float dropDuration = 0.5f;

        public void Setup(Ingredient[] ingredients)
        {
            this.ingredients = ingredients; 
        }

        public void Reset()
        {
            for (int i = 0; i < ingredientRects.Length; i++)
            {
                var image = ingredientRects[i];
                image.sprite = null;
                image.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 將所有食材隨機散落在鍋子內
        /// </summary>
        [ContextMenu("▶ 測試隨機散落食材")]
        public void RandomizeIngredientPositions()
        {
            if (ingredientRects == null || ingredientRects.Length == 0) return;

            Sequence seq = DOTween.Sequence();
            
            int index = -1;
            for (int i = 0; i < ingredientRects.Length; i++)
            {
                var image = ingredientRects[i];
                index++;
                if (index >= ingredients.Length) break;
                if (image == null) continue;
                var rect = image.rectTransform;
                
                image.sprite = ingredients[index].Sprite;
                rect.gameObject.SetActive(true);
                
                rect.DOKill();
                
                // 1. 🌟 均勻圓形隨機座標 (避免擠在一起)
                Vector2 randomPos = GetUniformRandomInCircle(spawnRadius);

                // 2. 🌟 限制輕微旋轉 (只在 -15 ~ +15 度之間變化)
                float randomZAngle = Random.Range(-maxRotationAngle, maxRotationAngle);

                // 3. 🌟 自然層次感：隨機加上輕微 Scale 變化 (例如 0.28 ~ 0.32)
                float randomScale = Random.Range(0.28f, 0.32f);

                // 設定起始狀態 (從上方落下)
                Vector2 startPos = randomPos + new Vector2(0f, dropHeight);
                rect.anchoredPosition = startPos;
                rect.localRotation = Quaternion.Euler(0, 0, randomZAngle);
                rect.localScale = Vector3.one * (randomScale * 1.2f); // 初始略大

                // 播放落入動畫 (使用 AppendInterval 讓 5 個食材有些許時間差落入，更有動態感！)
                float delay = i * 0.05f; // 每個食材間隔 0.05 秒
            
                seq.Insert(delay, rect.DOAnchorPos(randomPos, dropDuration).SetEase(Ease.OutBounce));
                seq.Insert(delay, rect.DOScale(Vector3.one * randomScale, dropDuration).SetEase(Ease.OutQuad));
            }
        }

        /// <summary>
        /// 算得出平均分布在圓形裡面的隨機座標
        /// </summary>
        private Vector2 GetUniformRandomInCircle(float radius)
        { Rose:
            float angle = Random.Range(0f, Mathf.PI * 2f);
            // 使用 Sqrt 讓分佈不會全部堆在中間或極端邊緣
            float r = Mathf.Sqrt(Random.Range(0.2f, 1f)) * radius; 
            return new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}