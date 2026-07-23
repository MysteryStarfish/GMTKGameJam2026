using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    [CreateAssetMenu(fileName = "PlayerInputReader", menuName = "Input/PlayerInputReader")]
    public class PlayerInputReader : ScriptableObject, InputSystem_Actions.IPlayerActions
    {
        public event Action SwitchPutGetEvent;
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
            _gameInput.Player.Disable();  
        }

        public void OnSwitchPutGet(InputAction.CallbackContext context)
        {
            if (context.performed) SwitchPutGetEvent?.Invoke();
        }
    }
}