using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.Core
{
    public class RestartAndHomeHandler : MonoBehaviour
    {
        [SerializeField] private AnimationClip[] pressedAnimations;
        [SerializeField] private Animator[] pressedAnimators;
        [SerializeField] private Image[] backgrounds;
        [SerializeField] private Image transitionImage;
        
        private void DoRestart()
        {
            DOTween.KillAll();
            
            DoTransition(() =>
            {
                Time.timeScale = 1f;

                GameState.Instance.Reset();
            
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex);
            });
        }
        
        private void DoHome()
        {
            throw new NotImplementedException("DoHome method is not implemented yet.");
            DoTransition();
        }
        
        private void DoTransition(Action onComplete = null)
        {
            transitionImage.DOColor(new Color(0, 0, 0, 1), 0.3f).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
        
        private void PressProcess(int idx)
        {
            AnimationClip clip = pressedAnimations[idx];
            PlayTakeIngredientAnimation(idx);
            ShowBackground(idx);
            
            float waitTime = clip != null ? clip.length + 1f : 0f;

            DOVirtual.DelayedCall(waitTime, () =>
            {
                if (idx == 0) DoHome();
                if (idx == 1) DoRestart();
                else throw new Exception("Invalid index for PressProcess. Expected 0 or 1, got: " + idx);
            });
        }

        private void PlayTakeIngredientAnimation(int idx)
        {
            pressedAnimators[idx].Play(pressedAnimations[idx].name);
        }

        private void ShowBackground(int idx)
        {
            FadeInOut(backgrounds[idx], 0.4f);
        }
        
        public void FadeInOut(Image targetImage, float fadeDuration)
        {
            targetImage.DOKill();

            Sequence seq = DOTween.Sequence();
            seq.Append(targetImage.DOFade(1f, 0.3f).From(0f));
            seq.AppendInterval(fadeDuration); 
            seq.Append(targetImage.DOFade(0f, 0.3f));
        }

        public void Home()
        {
            PressProcess(0);
        }
        
        public void Restart()
        {
            PressProcess(1);
        }
    }
}