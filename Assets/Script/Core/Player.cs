using System;
using System.Collections;
using Script.Core.Ingredients;
using Script.Input;
using Script.Tool.Cursors;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Core
{
    public class Player : MonoBehaviour
    {
        [SerializeField] PlayerInputReader inputReader;
        [SerializeField] private Animator _animator;
        
        private Ingredient _holdingIngredient;
        CursorHandler cursorHandler;
        public event Action<int> PotChosenEvent;
        public event Action<int> BadEndChosenEvent;
        public event Action MagicEvent;

        private Image img;

        private void Start()
        {
            img = GetComponent<Image>();
            StartCoroutine(WaitAndResize());
        }
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
            if (GameState.Instance.IsFinished)
            {
                BadEndChosenEvent?.Invoke(index);
                return;
            }
            PotChosenEvent?.Invoke(index);
        }
        
        public void PlayMagicAnimation()
        {
            _animator.Play("Magic");
            StartCoroutine(WaitAndResize());
        }

        private IEnumerator WaitAndResize()
        {
            // 等待 Animator 應用新的 Sprite
            yield return new WaitForEndOfFrame(); 
            if (img)
            {
                img.SetNativeSize();
            }
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

        public void PlayFallDownAnimation()
        {
            _animator.Play("FallDown");
            StartCoroutine(WaitAndResize());
        }
    }
}