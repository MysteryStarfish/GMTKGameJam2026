using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.Core.Ingredients
{
    public class IngredientsHandler : MonoBehaviour
    {
        [SerializeField] private IngredientSlot[] ingredientSlots;
        private int activeSlotsAmount;
        
        List<int> ActiveSlots;
        List<Ingredient> _ingredients;

        private bool IsActivate(int idx)
        {
            return ActiveSlots.Contains(idx);
        }
        
        public void Setup(List<Ingredient> ingredients, int activePotsAmount)
        {
            ActiveSlots = new List<int>();
            _ingredients = new List<Ingredient>();
            
            SetIngredients(ingredients);
            SetActiveSlotsAmount(activePotsAmount);

            GenerateSlots();
            RefreshIngredientSlots();
        }

        public void SetIngredients(List<Ingredient> ingredients)
        {
            _ingredients = ingredients ?? new List<Ingredient>();
        }

        public void SetActiveSlotsAmount(int amount)
        {
            activeSlotsAmount = amount;
        }
        
        private void GenerateSlots()
        {
            int mid = ingredientSlots.Length / 2;
            int left = mid-1; int right = mid;
            foreach (var slot in ingredientSlots)
            {
                slot.gameObject.SetActive(false);
            }

            for (int i = 0; i < activeSlotsAmount; i+=2)
            {
                ingredientSlots[left].gameObject.SetActive(true);
                ActiveSlots.Add(left);
                left--;
                
                ingredientSlots[right].gameObject.SetActive(true);
                if (i+1 >= activeSlotsAmount) ingredientSlots[right].ClearSlot();
                else ActiveSlots.Add(right);
                right++;
            }
        }

        private void RefreshIngredientSlots()
        {
            for (int i = 0; i < ActiveSlots.Count; i++)
            {
                if (i < _ingredients.Count)
                {
                    ingredientSlots[ActiveSlots[i]].SetIngredient(_ingredients[i]);
                }
                else
                {
                    ingredientSlots[ActiveSlots[i]].ClearSlot();
                }
            }
        }

        public Ingredient GetIngredient(int idx)
        {
            if (idx < 0 || idx >= ingredientSlots.Length) return null;
            return ingredientSlots[idx].GetIngredient();
        }
        
        public void ClearSlot(int idx)
        {
            if (idx < 0 || idx >= ingredientSlots.Length) return;
            ingredientSlots[idx].ClearSlot();
        }
    }
}