using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Core.Pots.PotType
{
    public class PotType : ScriptableObject, IPotType
    {
        public Sprite potSprite;
        public event Action OnCook;
        public AudioClip SoundClip { get; set; }
    }
}