using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Core
{
    public class BadEndSceneController : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private GameObject _badEndScene;
        [SerializeField] private GameObject _blackFront;
        [SerializeField] private GameObject _blackBack;
        [SerializeField] private GameObject _UI;

        private void Start()
        {
            _badEndScene.SetActive(false);
        }

        public void Show()
        {
            BadEndSceneSetup();
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                MovePlayerAboveBackground();
                _player.PlayFallDownAnimation();
            });
            sequence.AppendInterval(0.1f);
            sequence.Append(_blackFront.GetComponent<Image>().DOFade(1, 1.5f));
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() =>
            {
                _blackBack.SetActive(true);
                _UI.SetActive(true);
            });
            sequence.Append(_blackFront.GetComponent<Image>().DOFade(0, 1.5f));
        }

        private void BadEndSceneSetup()
        {
            _badEndScene.SetActive(true);
            _blackBack.SetActive(false);
            _UI.SetActive(false);
            _blackFront.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        private void MovePlayerAboveBackground()
        {
            _player.transform.SetParent(_blackFront.transform.parent);
            
            // 取得背景當前的索引值
            int bgIndex = _blackFront.transform.GetSiblingIndex();
    
            // 把 Player 設在背景的下一個位置，這樣 Player 就會剛好蓋在該背景之上
            // 並且不會影響到排在更後面的其他前景 UI
            _player.transform.SetSiblingIndex(bgIndex + 1); 
        }
    }
}