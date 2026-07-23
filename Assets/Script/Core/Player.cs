using System;
using UnityEngine;

namespace Script.Core
{
    public class Player : MonoBehaviour
    {
        [SerializeField] BaseInputReader inputReader;

        void Awake()
        {
            inputReader.OnAttack += OnAttack;
            inputReader.OnCharge += OnCharge;
            inputReader.OnCounter += OnCounter;
        }
        
        void OnAttack()
        {
            Debug.Log("Attack");
        }
        
        void OnCharge()
        {
            Debug.Log("Charge");
        }

        void OnCounter()
        {
            Debug.Log("Counter");
        }
    }
}