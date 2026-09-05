using UnityEngine;
using UnityEngine.UI;

namespace Script.Core
{
    public class NumberShower : MonoBehaviour
    {
        [SerializeField] private Image showImage;
        
        [SerializeField] private Sprite[] numberSprites;

        public void ShowNumber(int number)
        {
            showImage.sprite = numberSprites[number];
        }
    }
}