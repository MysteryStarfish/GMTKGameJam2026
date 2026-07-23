using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Player1InputReader", menuName = "Input/Player1InputReader")]
public class Player1InputReader : BaseInputReader, InputSystem_Actions.IPlayer1Actions
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
            _gameInput.Player1.SetCallbacks(this);
        }  

        _gameInput.Player1.Enable();  
    }  

    private void OnDisable()  
    {  
        _gameInput.Player1.Disable();  
    }  

    void InputSystem_Actions.IPlayer1Actions.OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnAttack?.Invoke();
    }

    void InputSystem_Actions.IPlayer1Actions.OnCharge(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnCharge?.Invoke();
    }

    void InputSystem_Actions.IPlayer1Actions.OnCounter(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnCounter?.Invoke();
    }
}