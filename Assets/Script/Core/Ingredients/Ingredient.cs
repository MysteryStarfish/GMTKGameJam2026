using Script.Core.Pots.PotTypes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Core.Ingredients
{
    public class Ingredient : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Sprite _cookedGoodSprite;
        [SerializeField] private Sprite _cookedBadSprite;
        [SerializeField] private float _cookTime;
        [SerializeField] private int _extraExcutionTime;
        public bool IsCookedGood;
        public Sprite ResultSprite => IsCookedGood ? _cookedGoodSprite : _cookedBadSprite;
        public PotType potType;

        public Sprite Sprite => _sprite;
        public Sprite CookedGoodSprite => _cookedGoodSprite;
        public Sprite CookedBadSprite => _cookedBadSprite;
        public float CookTime => _cookTime;
        public int ExtraExecutionTime => _extraExcutionTime;
        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}