using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core.Ingredients;
using UnityEngine;

namespace Script.Core.Pots
{
    public class PotsHandler : MonoBehaviour
    {
        private int activePotsAmount;
        public event Action<int> PutIngredientEvent;
        public event Action AllPotsFinishedEvent;
        [SerializeField] public Pot[] pots;

        public List<int> ActivePots;
        public bool AllPotsFinished => ActivePots.Count > 0 && ActivePots.All(idx => pots[idx].IsFinished);

        private bool IsActivePots(int idx)
        {
            return ActivePots.Contains(idx);
        }
        public void SetUp(int amount)
        {
            ActivePots = new List<int>();
            SetActivePotsAmount(amount);
            
            GeneratePots();
        }

        private void SetActivePotsAmount(int amount)
        {
            activePotsAmount = amount;
        }

        private void GeneratePots()
        {
            int mid = pots.Length / 2;
            int left = mid-1; int right = mid;
            foreach (var pot in pots)
            {
                pot.gameObject.SetActive(false);
            }

            for (int i = 0; i < activePotsAmount; i+=2)
            {
                pots[left].gameObject.SetActive(true);
                ActivePots.Add(left);
                left--;
                
                pots[right].gameObject.SetActive(true);
                if (i+1 >= activePotsAmount) pots[right].RemovePot();
                else ActivePots.Add(right);
                right++;
            }
        }

        public void OnPotChosen(int potIdx, Ingredient ingredient = null)
        {
            if (!IsActivePots(potIdx)) return;
            Pot pot = pots[potIdx];
            if (pot.IsEmpty())
            {
                PutIngredientEvent?.Invoke(potIdx);
                pot.PutIngredient(ingredient);
            }
            else if (!pot.IsSatisfied()) pot.DoExtraAction();
            else pot.TakeIngredient();
        }

        public void OnOnePotFinished()
        {
            if (!AllPotsFinished) return;
            AllPotsFinishedEvent?.Invoke();
        }
    }
}