using System;
using System.Collections.Generic;
using Script.Core.Ingredients;
using Script.Tool.Timers;
using UnityEngine;

namespace Script.Core.Pots
{
    public class Pot : MonoBehaviour
    {
        private int _currentExtraAmount;
        [SerializeField] private int extraAmount;
        
        private CountdownTimer _timer;
        private float _startTime;
        private float _endTime; 
        private float _duration;
        
        private bool _isFinished;
        public bool IsFinished =>  _isFinished;
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
            if  (_ingredient == null) throw new Exception("Already taking ingredient");
            _isFinished = true;
            PotFinishedEvent?.Invoke();
            _endTime = Time.time;
            _duration = _endTime - _startTime;
            _timer.Stop();
            Debug.Log("Take" + _ingredient.name + " 偏差" + Math.Abs(_duration - _ingredient.time));
            _ingredient = null;
            return _ingredient;
        }

        public void RemovePot()
        {
            throw new NotImplementedException();
        }
    }
}