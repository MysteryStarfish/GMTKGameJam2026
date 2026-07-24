using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    [CreateAssetMenu(fileName = "PlayerInputReader", menuName = "Input/PlayerInputReader")]
    public class PlayerInputReader : ScriptableObject, InputSystem_Actions.IPlayerActions
    {
        public event Action<int> PotChosenEvent;
        public event Action MagicEvent;
        private InputSystem_Actions _gameInput;
    
        private void OnEnable()  
        {  
            if (_gameInput == null)  
            {  
                _gameInput = new InputSystem_Actions();  
                _gameInput.Player.SetCallbacks(this);
            }  

            _gameInput.Player.Enable();  
        }  

        private void OnDisable()  
        {  
            if (_gameInput != null)
            {
                _gameInput.Player.Disable();  
            }
        }

        public void OnPot1(InputAction.CallbackContext context)
        {
            if (context.performed) PotChosenEvent?.Invoke(0);
        }

        public void OnPot2(InputAction.CallbackContext context)
        {
            if (context.performed) PotChosenEvent?.Invoke(1);
        }

        public void OnPot3(InputAction.CallbackContext context)
        {
            if (context.performed) PotChosenEvent?.Invoke(2);
        }

        public void OnPot4(InputAction.CallbackContext context)
        {
            if (context.performed) PotChosenEvent?.Invoke(3);
        }

        public void OnPot5(InputAction.CallbackContext context)
        {
            if (context.performed) PotChosenEvent?.Invoke(4);
        }

        public void OnPot6(InputAction.CallbackContext context)
        {
            if (context.performed) PotChosenEvent?.Invoke(5);
        }

        public void OnMagic(InputAction.CallbackContext context)
        {
            if (context.performed) MagicEvent?.Invoke();
        }
    }
}