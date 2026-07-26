using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Core.Audio;
using Script.Core.Crystals;
using Script.Core.Ingredients;
using Script.Core.Pots;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Core
{
    public class LevelHandler : MonoBehaviour
    {
        [SerializeField] List<LevelData> _levelData;
        [SerializeField] Player _player;
        [SerializeField] PotsHandler _potHandler;
        [SerializeField] IngredientsHandler _ingredientsHandler;
        [SerializeField] CrystalHandler _crystalHandler;
        [SerializeField] LevelClearAnimation _levelClearAnimation;
        [SerializeField] BigPotAnimation _bigPotAnimation;
        [SerializeField] CookingEntranceAnimation _cookingEntranceAnimation;
        [SerializeField] AudioHandler _audioHandler;
        
        private List<Ingredient> Results;
        

        private void Start()
        {
            LoadLevel(0);
            Results = new List<Ingredient>();
        }

        private void LoadLevel(int idx)
        {
            var levelData = _levelData[idx];
            _ingredientsHandler.Setup(new List<Ingredient>(levelData.ingredients), levelData.activePotsAmount);
            _potHandler.SetUp(levelData.activePotsAmount, levelData.pots, levelData.magicUseAmount);
            _bigPotAnimation.Setup(levelData.ingredients);
            _audioHandler.SetUp();
            _crystalHandler.ActiveCrystal(levelData.magicUseAmount);
            _cookingEntranceAnimation.SetIngredientSprites(Results);
        }

        private void OnEnable()
        {
            _potHandler.AllPotsFinishedEvent += OnAllPotsFinished;
            _potHandler.UseMagicPowerEvent += OnMagic;
            _player.MagicEvent += OnFinalMagic;
            _levelClearAnimation.OnCountDownStartEvent += _audioHandler.PlayMagicBGM;
            for (int i = 0; i < _potHandler.pots.Length; i++)
            {
                _potHandler.pots[i].SaveIngredientEvent += OnGetResult;
            }
        }

        private void OnDisable()
        {
            _potHandler.AllPotsFinishedEvent -= OnAllPotsFinished;
            _potHandler.UseMagicPowerEvent -= OnMagic;
            _player.MagicEvent -= OnFinalMagic;
            _levelClearAnimation.OnCountDownStartEvent -= _audioHandler.PlayMagicBGM;
            for (int i = 0; i < _potHandler.pots.Length; i++)
            {
                _potHandler.pots[i].SaveIngredientEvent -= OnGetResult;
            }
        }

        private void OnRunOutOfMagicPower()
        {
            
        }

        private void OnGetResult(Ingredient ingredient)
        {
            Results.Add(ingredient);
        }
        private void OnAllPotsFinished()
        {
            _levelClearAnimation.PlayClearSequence(() => LoadLevel(1));        
        }
        
        private void OnMagic()
        {
            _crystalHandler.TurnOffCrystal();
        }
        
        private void OnFinalMagic()
        {
            float duration = Time.time - _levelClearAnimation.startTime;
            float del = Mathf.Abs(5f - duration);
            Debug.Log("偏差" + del);
            _levelClearAnimation.JumpOutDish();
        }
        
        public void RestartLevel()
        {
            DOTween.KillAll();

            Time.timeScale = 1f;

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        
        }
    }
}