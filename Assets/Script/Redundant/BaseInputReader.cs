using System;
using UnityEngine;

namespace Script.Input
{
    public abstract class BaseInputReader : ScriptableObject, InputReader
    {
        private InputSystem_Actions _gameInput;  
    
        public abstract event Action OnAttack;
        public abstract event Action OnCharge;
        public abstract event Action OnCounter;
    }
}