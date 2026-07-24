using UnityEngine;
using UnityEngine.UI;

namespace Script.Core.Ingredients
{
    public class Ingredient : MonoBehaviour
    {
        private float _cookTime;
        [SerializeField] private Sprite _sprite;
        public float time;

        public Sprite Sprite => _sprite;
    }
}