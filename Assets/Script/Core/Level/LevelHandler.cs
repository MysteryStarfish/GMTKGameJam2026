using System;
using System.Collections.Generic;
using Script.Core.Ingredients;
using Script.Core.Pots;
using UnityEngine;

namespace Script.Core
{
    public class LevelHandler : MonoBehaviour
    {
        [SerializeField] LevelData _levelData;
        [SerializeField] Player _player;
        [SerializeField] PotsHandler _potHandler;
        [SerializeField] IngredientsHandler _ingredientsHandler;
        [SerializeField] LevelClearAnimation _levelClearAnimation;
        [SerializeField] BigPotAnimation _bigPotAnimation;

        private void Awake()
        {
            _ingredientsHandler.Setup(new List<Ingredient>(_levelData.ingredients), _levelData.activePotsAmount);
            _potHandler.SetUp(_levelData.activePotsAmount);
            _bigPotAnimation.Setup(_levelData.ingredients);
        }

        private void OnEnable()
        {
            _potHandler.AllPotsFinishedEvent += OnAllPotsFinished;
            _player.MagicEvent += OnMagic;
        }

        private void OnDisable()
        {
            _potHandler.AllPotsFinishedEvent -= OnAllPotsFinished;
            _player.MagicEvent -= OnMagic;
        }

        private void OnAllPotsFinished()
        {
            _levelClearAnimation.PlayClearSequence();
        }
        
        private void OnMagic()
        {
            float duration = Time.time - _levelClearAnimation.startTime;
            float del = Mathf.Abs(5f - duration);
            Debug.Log("偏差" + del);
            _levelClearAnimation.JumpOutDish();
        }
    }
}