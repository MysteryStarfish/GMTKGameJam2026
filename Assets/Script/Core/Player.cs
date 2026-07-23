using System;
using Script.Input;
using UnityEngine;

namespace Script.Core
{
    public class Player : MonoBehaviour
    {
        public enum Stat
        {
            Idle,
            Attack,
            Charge,
            Counter
        }
        [SerializeField] PlayerInputReader inputReader;
        public float chargeLevel;
        public Stat stat { get; private set; }

        void Awake()
        {
            inputReader.SwitchPutGetEvent += OnSwitchPutGet;
        }
        
        void OnSwitchPutGet()
        {
            Debug.Log("OnSwitchPutGet");
        }
    }
}