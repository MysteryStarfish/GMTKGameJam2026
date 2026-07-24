using System;
using Script.Core.Ingredients;
using Script.Input;
using Script.Tool.Cursors;
using UnityEngine;

namespace Script.Core
{
    public class Player : MonoBehaviour
    {
        [SerializeField] PlayerInputReader inputReader;
        
        private Ingredient _holdingIngredient;
        CursorHandler cursorHandler;
        public event Action<int> PotChosenEvent;
        public event Action MagicEvent;
        private void OnEnable()
        {
            if (inputReader == null) return;
            inputReader.PotChosenEvent += HandlePotChosen;
            inputReader.MagicEvent += OnMagic;
        }

        private void OnDisable()
        {
            if (inputReader == null) return;
            inputReader.PotChosenEvent -= HandlePotChosen;
            inputReader.MagicEvent -= OnMagic;
        }

        private void HandlePotChosen(int index)
        {
            PotChosenEvent?.Invoke(index);
        }

        public void SetCursorIngredient(Ingredient getIngredient)
        {
            _holdingIngredient = getIngredient;
            cursorHandler.SetNewCursor(_holdingIngredient.Sprite.texture);
        }
        private void OnMagic()
        {
            MagicEvent?.Invoke();
        }
    }
}