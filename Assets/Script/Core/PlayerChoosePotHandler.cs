using System;
using System.Collections.Generic;
using Script.Core.Ingredients;
using Script.Core.Pots;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Core
{
    public class PlayerChoosePotHandler : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private PotsHandler potHandler;
        [SerializeField] private IngredientsHandler ingredientHandler;
        
        private readonly List<Action> _cachedPotDelegates = new List<Action>();

        private void OnEnable()
        {
            player.PotChosenEvent += OnPotChosen;
            potHandler.PutIngredientEvent += ingredientHandler.ClearSlot;
            _cachedPotDelegates.Clear();

            for (int i = 0; i < potHandler.pots.Length; i++)
            {
                int currentIndex = i;
                Action potAction = () => OnPotChosen(currentIndex);
        
                _cachedPotDelegates.Add(potAction);
                potHandler.pots[i].PotChosenEvent += potAction;
                potHandler.pots[i].PotFinishedEvent += potHandler.OnOnePotFinished;
            }
        }

        private void OnDisable()
        {
            player.PotChosenEvent -= OnPotChosen;

            for (int i = 0; i < potHandler.pots.Length; i++)
            {
                if (i < _cachedPotDelegates.Count && potHandler.pots[i] != null)
                {
                    potHandler.pots[i].PotChosenEvent -= _cachedPotDelegates[i];
                    potHandler.pots[i].PotFinishedEvent -= potHandler.OnOnePotFinished;
                }
            }
            _cachedPotDelegates.Clear();
        }

        private void OnPotChosen(int idx)
        {
            potHandler.OnPotChosen(idx, ingredientHandler.GetIngredient(idx));
        }
    }
}