using Script.Core.Pots.PotTypes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Script.Core.Ingredients
{
    public class Ingredient : MonoBehaviour
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Sprite _cookedGoodSprite;
        [SerializeField] private Sprite _cookedBadSprite;
        [SerializeField] private float _cookTime;
        [SerializeField] private int _extraExcutionTime;
        public Sprite ResultSprite;
        public PotType potType;

        public Sprite Sprite => _sprite;
        public Sprite CookedGoodSprite => _cookedGoodSprite;
        public Sprite CookedBadSprite => _cookedBadSprite;
        public float CookTime => _cookTime;
        public int ExtraExecutionTime => _extraExcutionTime;
    }
}