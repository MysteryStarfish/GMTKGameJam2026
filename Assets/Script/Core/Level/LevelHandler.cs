using System.Collections.Generic;
using DG.Tweening;
using Script.Core.Audio;
using Script.Core.Crystals;
using Script.Core.Ingredients;
using Script.Core.Pots;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] GoodEndSceneController _goodEndSceneController;
        [SerializeField] BadEndSceneController _badEndSceneController;
        [SerializeField] AudioHandler _audioHandler;
        [SerializeField] RestartAndHomeHandler _restartAndHomeHandler;
        [SerializeField] Image TransitionBackground;
        
        private List<Ingredient> Results;
        

        private void Start()
        {
            LoadLevel(0);
            Results = new List<Ingredient>();
        }

        private void LoadLevel(int idx)
        {
            TransitionBackground.DOColor(new Color(0, 0, 0, 0), 0.3f);
            DOVirtual.DelayedCall(0.3f, () =>
            {
                var levelData = _levelData[idx];
                _ingredientsHandler.Setup(new List<Ingredient>(levelData.ingredients), levelData.activePotsAmount);
                _potHandler.SetUp(levelData.activePotsAmount, levelData.pots, levelData.magicUseAmount);
                _audioHandler.SetUp();
                _crystalHandler.ActiveCrystal(levelData.magicUseAmount);
            });
        }

        private void OnEnable()
        {
            _potHandler.AllPotsFinishedEvent += OnAllPotsFinished;
            _potHandler.UseMagicPowerEvent += OnMagic;
            _player.MagicEvent += OnFinalMagic;
            _levelClearAnimation.OnCountDownStartEvent += _audioHandler.PlayMagicBGM;
            _crystalHandler.RunOutOfMagicPower += OnRunOutOfMagicPower;
            _player.BadEndChosenEvent += OnBadEndChosen;
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
            _crystalHandler.RunOutOfMagicPower -= OnRunOutOfMagicPower;
            _player.BadEndChosenEvent -= OnBadEndChosen;
            for (int i = 0; i < _potHandler.pots.Length; i++)
            {
                _potHandler.pots[i].SaveIngredientEvent -= OnGetResult;
            }
        }

        private void OnBadEndChosen(int obj)
        {
            if (obj <= 2)
            {
                _restartAndHomeHandler.Home();
            }
            else 
            {
                _restartAndHomeHandler.Restart();
            }
        }

        private void OnRunOutOfMagicPower()
        {
            if (_potHandler.AllPotsFinished) return;
            SwitchToBadEnd();
            _audioHandler.PlayBadEndBGM();
        }

        private void SwitchToBadEnd()
        {
            WhenEnding();
            _badEndSceneController.Show();
        }

        private void WhenEnding()
        {
            GameState.Instance.Finish();
        }

        private void OnGetResult(Ingredient ingredient)
        {
            Results.Add(ingredient);
        }
        private void OnAllPotsFinished()
        {
            _levelClearAnimation.PlayClearSequence();        
        }
        
        private void OnMagic()
        {
            _crystalHandler.TurnOffCrystal();
        }
        
        private void OnFinalMagic()
        {
            if (GameState.Instance.IsFinished) return;
            float duration = Time.time - _levelClearAnimation.startTime;
            float del = Mathf.Abs(5f - duration);
            Debug.Log("偏差" + (5f - duration));
            SwitchToGoodEndScene();
            _audioHandler.PlayGoodEndBGM();
        }

        private void SwitchToGoodEndScene()
        {
            WhenEnding();
            _goodEndSceneController.Show();
        }

        
    }
}