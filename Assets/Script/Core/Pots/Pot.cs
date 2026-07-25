using System;
using System.Collections.Generic;
using Script.Core.Ingredients;
using Script.Tool.Timers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.Core.Pots
{
    public class Pot : MonoBehaviour
    {
        private int _currentExtraAmount;
        [SerializeField] private Image potImage;
        [SerializeField] private Image ingredientImage;
        [SerializeField] private int extraAmount;
        [SerializeField] private PotAnimation potAnimation;
        
        private CountdownTimer _timer;
        private float _startTime;
        public float Duration => Time.time - _startTime;
        
        private bool _isFinished;
        public bool IsFinished =>  _isFinished;
        public bool IsStarted =>  _startTime > 0;
        public float TargetTime => _ingredient?.time ?? 0;
        public PotType type;
        private Ingredient _ingredient;

        public event Action PotChosenEvent;
        public event Action PotFinishedEvent;

        private void Awake()
        {
            _timer = new CountdownTimer(10);
            _currentExtraAmount = 0;
        }

        private void FixedUpdate()
        {
            _timer.Tick(Time.deltaTime);
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
            _ingredient = ingredient;
            _timer.Reset(_ingredient.time);
            potAnimation.PlayPutIngredientAnimation(_ingredient, ingredientImage, potImage);
            _timer.Start();
            _startTime = Time.time;
            Debug.Log("Put" + ingredient.name);
        }

        public void DoExtraAction()
        {
            if (IsSatisfied()) throw new Exception("Already satisfied condition");
            _currentExtraAmount++;
            Debug.Log("DoExtraAction");
        }
        
        public bool IsSatisfied() => _currentExtraAmount >= extraAmount;

        public Ingredient TakeIngredient()
        {
            if (_isFinished) return null;
            if  (_ingredient == null) throw new Exception("Already taking ingredient");
            _isFinished = true;
            PotFinishedEvent?.Invoke();
            potAnimation.PlayTakeIngredientAnimation(_ingredient, ingredientImage, potImage, AfterTakeIngredient);
            Debug.Log("Take" + _ingredient.name + " 偏差" + Math.Abs(Duration - _ingredient.time));
            _ingredient = null;
            return _ingredient;
        }

        private void AfterTakeIngredient()
        {
            _timer.Stop();
        }

        public void RemovePot()
        {
            potImage.enabled = false;
        }

        public void Finish()
        {
            TakeIngredient();
        }
    }
}