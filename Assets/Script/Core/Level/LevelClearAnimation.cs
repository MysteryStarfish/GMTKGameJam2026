using System;
using DG.Tweening;
using Script.Core;
using Script.Core.Crystals;
using Script.Core.Ingredients;
using Script.Tool.Timers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelClearAnimation : MonoBehaviour
{
    public event Action OnCountDownStartEvent;

    [SerializeField] private GameObject frontGround;
    
    [Header("材料們")]
    [SerializeField] private IngredientsHandler ingredientsHandler;

    [SerializeField] private IngredientSlot[] ingredients;
    [SerializeField] private Vector2 dropRangeStart;
    [SerializeField] private Vector2 dropRangeEnd;
    [SerializeField] private float dropDistance = 120f;

    [Header("小鍋子們")]
    [SerializeField] private RectTransform[] leftPots;  // 左半邊的鍋子
    [SerializeField] private RectTransform[] rightPots; // 右半邊的鍋子
    [SerializeField] private float exitDistance = 1200f; // 移出螢幕的距離
    
    [Header("Player")]
    [SerializeField] private Player player;
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private Vector2 playerFrom;
    [SerializeField] private Vector2 playerTo;
    private CountdownTimer _timer;
    public float startTime;
    
    [Header("超大鍋子")]
    [SerializeField] private GameObject bigPotScene;
    
    [SerializeField] private RectTransform bigPotRect;
    [SerializeField] private Vector2 bigPotFrom;
    [SerializeField] private Vector2 bigPotTo;
    [SerializeField] private float bigPotMoveDuration = 0.5f;
    
    [SerializeField] private RectTransform bigPotTopRect;
    [SerializeField] private Vector2 bigPotTopFrom;
    [SerializeField] private Vector2 bigPotTopTo;
    
    [Header("Timer")]
    [SerializeField] private NumberShower countDownText;
    
    private void Start()
    {
        if (bigPotScene != null) 
        {
            bigPotScene.SetActive(false);
        }
        if (bigPotRect != null)
        {
            bigPotRect.gameObject.SetActive(false);
        }
        if (countDownText != null)
        {
            countDownText.gameObject.SetActive(false);
        }

        foreach (var ingredient in ingredients)
        {
            ingredient.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        OnCountDownStartEvent += player.PlayMagicAnimation;
    }

    private void OnDisable()
    {
        OnCountDownStartEvent -= player.PlayMagicAnimation;
    }
    
    public void PlayClearSequence()
    {
        SetupBigPotScene();
        
        Sequence sequence = DOTween.Sequence();
        AppendLittlePotsFlyOut(sequence);
        
        JoinIngredientsFlyOut(sequence);
        
        AppendPlayerMoveRight(sequence);
        
        JoinBigPotFlyIn(sequence);

        AppendIngredientsFlyIn(sequence);
        
        AppendBigPotTopFlyIn(sequence);
        
        AppendStartCountDown(sequence);
    }

    private void SetupBigPotScene()
    {
        if (bigPotScene != null)
        {
            bigPotScene.SetActive(true); 
        }

        if (bigPotRect != null)
        {
            bigPotRect.gameObject.SetActive(true);
            bigPotRect.anchoredPosition = bigPotFrom;
        }
        
        if (bigPotTopRect != null)
        {
            bigPotTopRect.gameObject.SetActive(true);
            bigPotTopRect.anchoredPosition = bigPotTopFrom;
        }

        for (int i = 0; i <  ingredientsHandler.activeSlotsAmount; i++)
        {
            var randomPoint = GetRandomPointBetween(dropRangeStart, dropRangeEnd);
            ingredients[i].RectTransform.anchoredPosition = new Vector2(randomPoint.x, randomPoint.y + dropDistance);
            ingredients[i].gameObject.SetActive(true);
        }
    }

    private void AppendPlayerMoveRight(Sequence sequence)
    {
        sequence.Append(playerRect.DOAnchorPos(playerTo, 0.5f).SetEase(Ease.OutQuad));
    }

    private void AppendBigPotTopFlyIn(Sequence sequence)
    {
        sequence.Append(bigPotTopRect.DOAnchorPos(bigPotTopTo, 0.5f).SetEase(Ease.OutQuad));
    }

    private void AppendIngredientsFlyIn(Sequence sequence)
    {
        float interval = 0.1f;
        float duration = sequence.Duration();
        
        int index = 0;
        foreach (var ingredientSlotIdx in ingredientsHandler.ActiveSlots)
        {
            ingredients[index].SetIngredient(ingredientsHandler.GetIngredient(ingredientSlotIdx));
            ingredients[index].SetCookedImage();
    
            sequence.Insert(duration + index * interval, ingredients[index].RectTransform.DOAnchorPosY(-dropDistance, 0.7f).SetEase(Ease.OutQuad));
            index++;
        }
    }

    private void JoinIngredientsFlyOut(Sequence sequence)
    {
        foreach (var ingredientSlotIdx in ingredientsHandler.ActiveSlots)
        {
            var ingredient = ingredientsHandler.GetIngredientSlot(ingredientSlotIdx);
            
            sequence.Join(ingredient.RectTransform.DOAnchorPos(ingredient.RectTransform.anchoredPosition + new Vector2(0, 150), 0.5f).SetEase(Ease.InQuad));
        }

        sequence.AppendCallback(() =>
        {
            frontGround.SetActive(true);
        });
    }

    private void AppendStartCountDown(Sequence sequence)
    {
        _timer = new CountdownTimer(6f);
        sequence.AppendCallback(() => 
        {
            _timer.Start();
            startTime = Time.time;
            if (countDownText != null) countDownText.gameObject.SetActive(true);
            OnCountDownStartEvent?.Invoke();
        });
    }

    private void JoinBigPotFlyIn(Sequence sequence)
    {
        sequence.Join(bigPotRect.DOAnchorPos(bigPotTo, bigPotMoveDuration).SetEase(Ease.OutBounce));
    }

    private void AppendLittlePotsFlyOut(Sequence sequence)
    {
        foreach (var pot in leftPots)
        {
            if (pot == null || !pot.gameObject.activeSelf) continue;
            
            sequence.Join(pot.DOAnchorPosX(pot.anchoredPosition.x - exitDistance, 0.5f)
                .SetEase(Ease.InCubic));
        }

        foreach (var pot in rightPots)
        {
            if (pot == null || !pot.gameObject.activeSelf) continue;

            sequence.Join(pot.DOAnchorPosX(pot.anchoredPosition.x + exitDistance, 0.5f)
                .SetEase(Ease.InCubic));
        }
    }

    private int _lastDisplayedSecond = -1;
    private void Update()
    {
        if (_timer != null && _timer.IsRunning)
        {
            UpdateCountdownUI();
        }
    }
    private void FixedUpdate()
    {
        if (_timer is { IsRunning: true }) _timer.Tick(Time.fixedDeltaTime);
    }
    
    private void UpdateCountdownUI(bool forceUpdate = false)
    {
        // 使用 FloorToInt 並用 +0.99f 讓 5.0~4.01 秒都穩穩顯示 5
        // 或是直接 FloorToInt(_timer.Time + 0.01f) 防浮點數誤差
        int currentSecond = Mathf.Clamp(Mathf.FloorToInt(_timer.Time + 0.001f), 0, (int)5);

        if (forceUpdate || currentSecond != _lastDisplayedSecond)
        {
            _lastDisplayedSecond = currentSecond;
            countDownText.ShowNumber(currentSecond);
        }
    }
    
    public Vector2 GetRandomPointBetween(Vector2 a, Vector2 b)
    {
        // Random.value 會精準回傳一個 0.0f ~ 1.0f 之間的隨機浮點數
        float randomT = Random.value; 

        // 進行插值運算
        return Vector2.Lerp(a, b, randomT);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        Gizmos.DrawLine(dropRangeStart, dropRangeEnd);
    }

}