using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core.Ingredients;
using Script.Core.Pots.PotTypes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Core.Pots
{
    public class PotsHandler : MonoBehaviour
    {
        private struct PotRuntimeState
        {
            public int CurrentUseMagicAmount;
            public int MagicLimit;
        }
        private int activePotsAmount;
        public event Action<int> PutIngredientEvent;
        public event Action<int> TakeIngredientEvent;
        public event Action AllPotsFinishedEvent;
        public event Action RunOutOfMagicPowerEvent;
        public event Action UseMagicPowerEvent;
        [SerializeField] public Pot[] pots;
        [SerializeField] public float chainAcceptableRange;
        [SerializeField] public float cookAcceptableRange;

        public List<int> ActivePots;
        public bool AllPotsFinished => ActivePots.Count > 0 && ActivePots.All(idx => pots[idx].IsFinished);

        private PotRuntimeState _state;
        private int _currentUseMagicAmount => _state.CurrentUseMagicAmount;
        private int magicLimit => _state.MagicLimit;
        private bool _hasFiredAllFinished = false; // 新增這行
        
        private bool IsActivePot(int idx)
        {
            return ActivePots.Contains(idx);
        }

        private int PotIndex(Pot pot)
        {
            return pots.ToList().IndexOf(pot);
        }
        public void SetUp(int amount, PotType[] pots, int magicLimit)
        {
            ActivePots = new List<int>();
            _state.MagicLimit = magicLimit;
            SetActivePotsAmount(amount);
            
            GeneratePots(pots);
        }

        private void SetActivePotsAmount(int amount)
        {
            activePotsAmount = amount;
        }

        private void GeneratePots(PotType[] potTypes)
        {
            int mid = pots.Length / 2;
            int left = mid-1; int right = mid;
            foreach (var pot in pots)
            {
                pot.gameObject.SetActive(true);
                pot.RemovePot();
            }

            for (int i = 0; i < activePotsAmount; i+=2)
            {
                pots[left].AddPot(potTypes[i]);
                ActivePots.Add(left);
                left--;
                
                if (i+1 >= activePotsAmount) break;
                pots[right].AddPot(potTypes[i+1]);
                ActivePots.Add(right);
                right++;
            }
        }

        public void OnPotChosen(int potIdx, Ingredient ingredient = null)
        {
            if (!IsActivePot(potIdx)) return;
            Pot pot = pots[potIdx];
            if (pot.IsFinished) return;
            if (pot.IsEmpty())
            {
                PutIngredientEvent?.Invoke(potIdx);
                pot.PutIngredient(ingredient);
            }
            else if (!pot.IsSatisfied()) pot.DoExtraAction();
            else
            {
                pot.TakeIngredient();
                TakeIngredientEvent?.Invoke(potIdx);
            }
        }

        public void OnOneTakeIngredient()
        {
            foreach (var pot in pots)
            {
                Debug.Log("pot IsStarted: " + pot.IsStarted + " Duration: " + pot.Duration + " TargetTime: "+ pot.TargetTime);
                if (!(pot.IsStarted && Math.Abs(pot.Duration - pot.TargetTime) < chainAcceptableRange)) continue;
                pot.ChainFinish();
                TakeIngredientEvent?.Invoke(PotIndex(pot));
            }
        }

        public void OnUseMana()
        {
            _state.CurrentUseMagicAmount++;
            UseMagicPowerEvent?.Invoke();
        }

        public void AfterOneTakeIngredient()
        {
            if (!AllPotsFinished && _state.CurrentUseMagicAmount >= magicLimit) RunOutOfMagicPowerEvent?.Invoke();
            if (!AllPotsFinished) return;
            if (_hasFiredAllFinished) return;
            AllPotsFinishedEvent?.Invoke();
            _hasFiredAllFinished = true;
        }
    }
}