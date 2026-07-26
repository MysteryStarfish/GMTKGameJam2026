using UnityEngine;
using UnityEngine.UI;

namespace Script.Core.Crystals
{
    public class Crystal : MonoBehaviour
    {
        [SerializeField] public Image crystalImage;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        public void TurnOff()
        {
            crystalImage.sprite = offSprite;
        }
        
        public void TurnOn()
        {
            crystalImage.sprite = onSprite;
        }
    }
}