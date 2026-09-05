using UnityEngine;
using UnityEngine.UI;

namespace Script.Core.Ingredients
{
    public class IngredientSlot : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Ingredient ingredient;
        
        public RectTransform RectTransform => transform as RectTransform;

        public Ingredient GetIngredient()
        {
            return ingredient;
        }

        public void SetIngredient(Ingredient ingredient)
        {
            this.ingredient = ingredient;

            if (image == null) return;
            image.sprite = ingredient != null ? ingredient.Sprite : null;
            image.enabled = ingredient != null;
            image.SetNativeSize();
        }
        
        public void ClearSlotImage()
        {
            if (image == null) return;
            image.sprite = null;
            image.enabled = false;
        }

        public void ClearSlot()
        {
            ingredient = null;

            if (image == null) return;
            image.sprite = null;
            image.enabled = false;
        }

        public void SetCookedImage()
        {
            image.sprite = ingredient.ResultSprite;
            image.enabled = true;
        }
    }
}