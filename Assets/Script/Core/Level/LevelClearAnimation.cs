using System;
using DG.Tweening;
using Script.Core.Ingredients;
using Script.Tool.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelClearAnimation : MonoBehaviour
{
    public event Action OnCountDownStartEvent;
    [SerializeField] private LayoutGroup tableLayoutGroup;
    
    [Header("材料們")]
    [SerializeField] private RectTransform ingredients;

    [Header("小鍋子們")]
    [SerializeField] private RectTransform[] leftPots;  // 左半邊的鍋子
    [SerializeField] private RectTransform[] rightPots; // 右半邊的鍋子
    [SerializeField] private float exitDistance = 1200f; // 移出螢幕的距離

    [Header("超大鍋子")]
    [SerializeField] private RectTransform giantPotRect;
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private Vector2 giantPotTargetPos;
    
    [Header("成品")]
    [SerializeField] private RectTransform dishRect;
    [SerializeField] private Vector2 dishPotTargetPos;
    
    private CountdownTimer _timer;
    public float startTime;
    
    private void Start()
    {
        // 預設將超大鍋子隱藏/縮小/移到螢幕下方準備
        if (giantPotRect != null)
        {
            giantPotRect.localScale = Vector3.zero;
            giantPotRect.gameObject.SetActive(false);
        }
        if (countDownText != null)
        {
            countDownText.gameObject.SetActive(false);
        }
        _timer = new CountdownTimer(5f);
        _timer.OnStop += OnCountdownComplete;
        
        if (dishRect != null)
        {
            dishRect.gameObject.SetActive(false);
            dishRect.localScale = Vector3.zero;
            dishRect.anchoredPosition = dishPotTargetPos + new Vector2(0f, -500f);
        }
    }

    public void OnCountdownComplete()
    {
        if (!dishRect) return;

        dishRect.gameObject.SetActive(true);
        countDownText.text = "0";
    }

    public void JumpOutDish()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(dishRect.DOAnchorPos(dishPotTargetPos, 0.8f).SetEase(Ease.OutCubic));
        seq.Join(dishRect.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack));
    }

    /// <summary>
    /// 觸發通關動畫
    /// </summary>
    public void PlayClearSequence(Action callback)
    {
        if (tableLayoutGroup != null)
        {
            tableLayoutGroup.enabled = false; 
        }
        // 建立 DOTween 序列
        Sequence sequence = DOTween.Sequence();

        // ----------------------------------------------------
        // 步驟 1：桌子對齊底部 & 鍋子左右飛出 (同時進行 Join)
        // ----------------------------------------------------
        
        // 1. 桌子往下移
        // sequence.Append(tableRect.DOAnchorPos(tableTargetPos, 0.6f).SetEase(Ease.OutCubic));

        // 2. 左邊鍋子往左移出
        foreach (var pot in leftPots)
        {
            if (pot == null || !pot.gameObject.activeSelf) continue;
            
            sequence.Join(pot.DOAnchorPosX(pot.anchoredPosition.x - exitDistance, 0.5f)
               .SetEase(Ease.InCubic));
        }

        // 3. 右邊鍋子往右移出
        foreach (var pot in rightPots)
        {
            if (pot == null || !pot.gameObject.activeSelf) continue;

            sequence.Join(pot.DOAnchorPosX(pot.anchoredPosition.x + exitDistance, 0.5f)
               .SetEase(Ease.InCubic));
        }
        
        sequence.Join(ingredients.DOAnchorPosY(ingredients.anchoredPosition.y - exitDistance, 0.5f)
            .SetEase(Ease.InCubic));

        // ----------------------------------------------------
        // 步驟 2：出現超大鍋子 (Append 銜接在移出之後)
        // ----------------------------------------------------
        sequence.AppendCallback(() => 
        {
            if (giantPotRect != null) giantPotRect.gameObject.SetActive(true);
        });

        // 效果：從 0 彈跳放大出現 (Ease.OutBack)
        sequence.Append(giantPotRect.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));
        sequence.Append(countDownText.rectTransform.DOScale(Vector3.one, 0.05f).SetEase(Ease.OutBack));
        
        // 如果想同時從下方升起：
        // sequence.Join(giantPotRect.DOAnchorPos(giantPotTargetPos, 0.6f).SetEase(Ease.OutBack));
        sequence.AppendCallback(() => 
        {
            _timer.Start();
            startTime = Time.time;
            if (countDownText != null) countDownText.gameObject.SetActive(true);
            OnCountDownStartEvent?.Invoke();
        });
        // 動態結束時的回呼
        sequence.OnComplete(() => 
        {
            callback?.Invoke();
        });
    }
    private int _lastDisplayedSecond = -1;
    private void Update()
    {
        // 取出當前剩餘的整數秒數 (例如 29.8 秒 -> 29 秒)
        if (_timer != null && _timer.IsRunning)
        {
            UpdateCountdownUI();
        }
    }
    
    private void UpdateCountdownUI(bool forceUpdate = false)
    {
        int currentSecond = Mathf.CeilToInt(_timer.Time);

        if (forceUpdate || currentSecond != _lastDisplayedSecond)
        {
            _lastDisplayedSecond = currentSecond;
            countDownText.text = currentSecond.ToString();
        }
    }

    private void FixedUpdate()
    {
        if (_timer.IsRunning) _timer.Tick(Time.fixedDeltaTime);
    }
}