using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Player2InputReader", menuName = "Input/Player2InputReader")]
public class Player2InputReader : BaseInputReader, InputSystem_Actions.IPlayer2Actions
{
    public override event Action OnAttack;
    public override event Action OnCharge;
    public override event Action OnCounter;
    private InputSystem_Actions _gameInput;
    
    private void OnEnable()  
    {  
        if (_gameInput == null)  
        {  
            _gameInput = new InputSystem_Actions();  
            _gameInput.Player2.SetCallbacks(this);
        }  

        _gameInput.Player2.Enable();  
    }  

    private void OnDisable()  
    {  
        _gameInput.Player2.Disable();  
    }  

    void InputSystem_Actions.IPlayer2Actions.OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnAttack?.Invoke();
    }

    void InputSystem_Actions.IPlayer2Actions.OnCharge(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnCharge?.Invoke();
    }

    void InputSystem_Actions.IPlayer2Actions.OnCounter(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnCounter?.Invoke();
    }
}