using System;
using System.Collections;
using DG.Tweening;
using Script.Core.Ingredients;
using Script.Core.Pots.PotTypes;
using Script.Tool.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Core.Pots
{
    public class Pot : MonoBehaviour
    {
        private struct PotRuntimeState
        {
            public Ingredient Ingredient;
            public int CurrentExtraAmount;
            public float StartTime;
            public float EndTime;
            public float TotalDelayTime;
            public bool IsFinished;
        }
        [SerializeField] private Image potImage;
        [SerializeField] private Image ingredientImage;
        [SerializeField] public PotType potType;
        [SerializeField] private Animator _animator;
        private int extraAmount => _ingredient?.ExtraExecutionTime ?? 0;
        
        private PotRuntimeState _state;
        private CountdownTimer _timer;
        private float _startTime => _state.StartTime;
        private float _endTime => _state.EndTime;
        public float Duration => _isFinished ? _endTime - _startTime : Time.time - _startTime;
        
        private bool _isFinished => _state.IsFinished;
        public bool IsStarted => _startTime > 0;
        public bool IsFinished => _isFinished;
        public float TargetTime => _ingredient?.CookTime ?? 0;
        private Ingredient _ingredient => _state.Ingredient;
        private int _currentExtraAmount =>  _state.CurrentExtraAmount;
        private float CookTime => _ingredient?.CookTime ?? 0;
        public float TotalDelayTime => _state.TotalDelayTime;

        public event Action PotChosenEvent;
        public event Action PutIngredientEvent;
        public event Action ExtraExecutionEvent;
        public event Action TakeIngredientEvent;
        public event Action UseManaEvent;
        public event Action<Ingredient> SaveIngredientEvent;
        public event Action AfterTakeIngredientEvent;

        private void Awake()
        {
            Setup();
        }

        private void OnEnable()
        {
            PutIngredientEvent += OnPutIngredient;
            ExtraExecutionEvent += OnExtraExecution;
            TakeIngredientEvent += OnTakeIngredient;
        }

        private void OnDisable()
        {
            PutIngredientEvent -= OnPutIngredient;
            ExtraExecutionEvent -= OnExtraExecution;
            TakeIngredientEvent -= OnTakeIngredient;
        }

        private void Setup()
        {
            _timer = new CountdownTimer(0);
            _state.CurrentExtraAmount = 0;
        }

        private void FixedUpdate()
        {
            if (_timer is { IsRunning: true }) _timer.Tick(Time.deltaTime);
        }
        
        public void OnPotChosen()
        {
            PotChosenEvent?.Invoke();
        }

        public bool IsEmpty()
        {
            return _ingredient == null;
        }
        public void PutIngredient(Ingredient ingredient)
        {
            if (_ingredient != null) throw new Exception("Already putting ingredient");
            if (_isFinished) return;
            SetupIngredient(ingredient);
            PutIngredientEvent?.Invoke();
            PotCountDownStart();
            // Debug.Log("Put" + ingredient.name);
        }

        private void PotCountDownStart()
        {
            _timer.Start();
            _state.StartTime = Time.time;
        }

        private void SetupIngredient(Ingredient ingredient)
        {
            _state.Ingredient = ingredient;
            _timer.Reset(_ingredient.CookTime);
        }

        public void DoExtraAction()
        {
            if (IsSatisfied()) throw new Exception("Already satisfied condition");
            _state.CurrentExtraAmount++;
            ExtraExecutionEvent?.Invoke();
            _state.TotalDelayTime += Mathf.Abs(Duration - CookTime / (extraAmount + 1) * (_state.CurrentExtraAmount + 1));
            // Debug.Log("DoExtraAction");
        }
        
        public bool IsSatisfied() => _currentExtraAmount >= extraAmount;

        public void ChainTakeIngredient()
        {
            if (_isFinished) return;
            if  (_ingredient == null) throw new Exception("Already taking ingredient");
            _state.IsFinished = true;
            PotCountDownStop();
            _state.TotalDelayTime += Math.Abs(Duration - _ingredient.CookTime);
            Debug.Log("Take" + _ingredient.name + " 偏差" + Math.Abs(Duration - _ingredient.CookTime));
            StartCoroutine(Co_ChainTakeIngredientProcess());
        }

        public void TakeIngredient()
        {
            if (_isFinished) return;
            if  (_ingredient == null) throw new Exception("Already taking ingredient");
            _state.IsFinished = true;
            PotCountDownStop();
            _state.TotalDelayTime += Math.Abs(Duration - _ingredient.CookTime);
            Debug.Log("Take" + _ingredient.name + " 偏差" + Math.Abs(Duration - _ingredient.CookTime));
            StartCoroutine(Co_TakeIngredientProcess());
        }

        private void PotCountDownStop()
        {
            _timer.Stop();
            _state.EndTime = Time.time;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator Co_ChainTakeIngredientProcess()
        {
            AnimationClip clip = potType.takeIngredientAnimation;
            PlayTakeIngredientAnimation();
            ShowIngredient();
            TakeIngredientEvent?.Invoke();
            SaveIngredientEvent?.Invoke(_ingredient);

            if (clip)
            {
                yield return new WaitForSeconds(clip.length + 1);
            }
            Debug.Log(clip.name);
            AfterTakeIngredientEvent?.Invoke();
            _state.Ingredient = null;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator Co_TakeIngredientProcess()
        {
            AnimationClip clip = potType.takeIngredientAnimation;
            PlayTakeIngredientAnimation();
            ShowIngredient();
            TakeIngredientEvent?.Invoke();
            UseManaEvent?.Invoke();
            SaveIngredientEvent?.Invoke(_ingredient);

            if (clip)
            {
                yield return new WaitForSeconds(clip.length + 1);
            }
            Debug.Log(clip.name);
            AfterTakeIngredientEvent?.Invoke();
            _state.Ingredient = null;
        }

        public void RemovePot()
        {
            potImage.enabled = false;
        }

        public void Finish()
        {
            TakeIngredient();
        }
        
        public void Reset()
        {
            _state = default;
            _timer.Stop();
            potImage.enabled = true;  
            ingredientImage.sprite = null;  
        }

        public void AddPot(PotType potType)
        {
            if (potType == null)
            {
                Debug.LogError($"[{nameof(Pot)}] 傳入的 potType 為 null！", this);
                return;
            }

            // 2. 檢查 UI Image 元件是否存在
            if (potImage == null)
            {
                Debug.LogError($"[{nameof(Pot)}] potImage 未在 Inspector 指派！GameObject: {gameObject.name}", this);
                return;
            }

            this.potType = potType;

            // 3. 賦值 Sprite 並啟用
            if (potType.potSprite != null)
            {
                potImage.sprite = potType.potSprite;
                potImage.enabled = true;
            }
            else
            {
                Debug.LogWarning($"[{nameof(Pot)}] {potType.name} 的 potSprite 尚未設定！", this);
                potImage.enabled = false;
            }

            PlayInitAnimation();
        }
        
        private void OnPutIngredient()
        {
            PlayPutIngredientAnimation();
        }

        private void OnExtraExecution()
        {
            PlayPutIngredientAnimation();
        }

        private void OnTakeIngredient()
        {
            // ShowIngredient();
        }

        public void PlayInitAnimation()
        {
            PlayClip(potType.initAnimation);
        }

        public void PlayPutIngredientAnimation()
        {
            Debug.Log("PlayPutIngredientAnimation");
            PlayClip(potType.putIngredientAnimation);
        }

        public void PlayExtraExecutionAnimation()
        {
            PlayClip(potType.extraExecutionAnimation);
        }

        public void PlayTakeIngredientAnimation()
        {
            PlayClip(potType.takeIngredientAnimation);
        }
        
        private void PlayClip(AnimationClip clip)
        {
            _animator.Play(clip.name);
        }

        private void ShowIngredient()
        {
            _ingredient.IsCookedGood = Math.Abs(Duration - _ingredient.CookTime) < 0.5f;
            ingredientImage.sprite = _ingredient.ResultSprite;
            ingredientImage.gameObject.SetActive(true);
            ingredientImage.enabled = true;
            FadeInOut(ingredientImage, 0.5f);
        }
        
        public void FadeIn(Image targetImage, float fadeDuration)
        {
            targetImage.DOFade(1f, fadeDuration);
        }

        public void FadeOut(Image targetImage, float fadeDuration)
        {
            targetImage.DOFade(0f, fadeDuration);
        }
        
        public void FadeInOut(Image targetImage, float fadeDuration)
        {
            ingredientImage.DOKill();

            Sequence seq = DOTween.Sequence();
            seq.Append(ingredientImage.DOFade(1f, 0.3f).From(0f));
            seq.AppendInterval(0.4f); 
            seq.Append(ingredientImage.DOFade(0f, 0.3f));
        }

        public void ChainFinish()
        {
            ChainTakeIngredient();
        }
    }
}