using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Core
{
    public class GoodEndSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject _flash;
        [SerializeField] private GameObject _goodEndScene;
        [SerializeField] private GameObject _goodEnd;
        [SerializeField] private RectTransform potTop;
        
        [SerializeField] private float fadeInTime;
        [SerializeField] private float durationTime;
        [SerializeField] private float fadeOutTime;

        void Start()
        {
            GoodEndSetup();
        }
        
        public void Show()
        {
            GoodEndReady();
            Sequence sequence = DOTween.Sequence();
            
            sequence.Append(potTop.DOAnchorPosY(40, 0.5f+fadeInTime).SetEase(Ease.OutCirc));
            sequence.Insert(0.5f, _flash.GetComponent<Image>().DOFade(1f, fadeInTime).SetEase(Ease.OutQuart));
            sequence.AppendInterval(durationTime);
            sequence.AppendCallback(() => 
            {
                _goodEnd.SetActive(true);
            });
            sequence.Append(_flash.GetComponent<Image>().DOFade(0f, fadeOutTime).SetEase(Ease.OutQuart));
        }

        private void GoodEndReady()
        {
            _goodEnd.SetActive(false);
            _flash.SetActive(true);
            _flash.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            _goodEndScene.SetActive(true);
        }
        
        private void GoodEndSetup()
        {
            _goodEndScene.SetActive(false);
        }
    }
}