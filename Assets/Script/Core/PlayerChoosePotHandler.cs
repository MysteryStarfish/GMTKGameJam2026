using System;
using System.Collections.Generic;
using Script.Core.Audio;
using Script.Core.Ingredients;
using Script.Core.Pots;
using UnityEngine;

namespace Script.Core
{
    public class PlayerChoosePotHandler : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private PotsHandler potHandler;
        [SerializeField] private IngredientsHandler ingredientHandler;
        [SerializeField] private AudioHandler audioHandler;
        
        private readonly List<Action> _cachedPotDelegates = new List<Action>();

        private void OnEnable()
        {
            player.PotChosenEvent += OnPotChosen;
            potHandler.PutIngredientEvent += ingredientHandler.ClearSlotImage;
            potHandler.TakeIngredientEvent += ingredientHandler.SetCookedImage;
            potHandler.PutIngredientEvent += PlayPotCookingBGM;
            _cachedPotDelegates.Clear();

            for (int i = 0; i < potHandler.pots.Length; i++)
            {
                int currentIndex = i;
                Action potAction = () => OnPotChosen(currentIndex);
        
                _cachedPotDelegates.Add(potAction);
                potHandler.pots[i].PotChosenEvent += potAction;
                potHandler.pots[i].TakeIngredientEvent += potHandler.OnOneTakeIngredient;
                potHandler.pots[i].AfterTakeIngredientEvent += potHandler.AfterOneTakeIngredient;
                potHandler.pots[i].UseManaEvent += potHandler.OnUseMana;
            }
        }

        private void OnDisable()
        {
            player.PotChosenEvent -= OnPotChosen;
            potHandler.PutIngredientEvent -= ingredientHandler.ClearSlotImage;
            potHandler.TakeIngredientEvent -= ingredientHandler.SetCookedImage;
            potHandler.PutIngredientEvent -= PlayPotCookingBGM;
            for (int i = 0; i < potHandler.pots.Length; i++)
            {
                if (i < _cachedPotDelegates.Count && potHandler.pots[i] != null)
                {
                    potHandler.pots[i].PotChosenEvent -= _cachedPotDelegates[i];
                    potHandler.pots[i].TakeIngredientEvent -= potHandler.OnOneTakeIngredient;
                    potHandler.pots[i].AfterTakeIngredientEvent -= potHandler.AfterOneTakeIngredient;
                    potHandler.pots[i].UseManaEvent -= potHandler.OnUseMana;
                }
            }
            _cachedPotDelegates.Clear();
        }

        private void OnPotChosen(int idx)
        {
            if (GameState.Instance.IsFinished) return;
            potHandler.OnPotChosen(idx, ingredientHandler.GetIngredient(idx));
        }

        private void PlayPotCookingBGM(int idx)
        {
            if (GameState.Instance.IsFinished) return;
            audioHandler.AddPotAccompaniment(potHandler.pots[idx].potType.soundClip);
        }
        
        private void RemovePotCookingBGM(int idx)
        {
            audioHandler.RemovePotAccompaniment(potHandler.pots[idx].potType.soundClip);
        }
    }
}