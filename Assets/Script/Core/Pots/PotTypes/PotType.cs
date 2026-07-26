using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Core.Pots.PotTypes
{
    [CreateAssetMenu(fileName = "PotType", menuName = "ScriptableObjects/Pot")]
    public class PotType : ScriptableObject
    {
        public Sprite potSprite;
        public AnimationClip initAnimation;
        public AnimationClip putIngredientAnimation;
        public AnimationClip extraExecutionAnimation;
        public AnimationClip takeIngredientAnimation;
        public AudioClip soundClip;
    }
}