using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using TMPro;

using DG.Tweening;

namespace Jam3
{
    public class LoadingSectionUI : MonoBehaviour
    {
        public Action OnComplete = null;

        public float enterAnimationDelay = 0.5f;
        public float exitAnimationDelay = 0.8f;
        public float animationDuration = 0.75f;

        public RectTransform titleTransform;

        public TextMeshProUGUI titleText;

        public Image titleLogo;

        public VideoPlayer LogoVideo = null;
        public RawImage LogoVideoTexture = null;

        private Color titleTextInitialColor;
        private Color titleLogoInitialColor;

        private bool isVideoPlaying;

        private void Update()
        {
            if (!LogoVideo.isPlaying && !isVideoPlaying)
                LogoVideoTexture.color = new Color(1, 1, 1, 0);
            else
            {
                if (!isVideoPlaying)
                    StartCoroutine(WaitAndShowVideo());
            }
        }

        private IEnumerator WaitAndShowVideo()
        {
            isVideoPlaying = true;
            yield return new WaitForSeconds(0.5f);

            AudioManager.Instance.PlayAudioClip("Logo");
            LogoVideoTexture.color = new Color(1, 1, 1, 1);
        }

        public void StartFlow(Action onComplete)
        {
            OnComplete = onComplete;

            gameObject.SetActive(true);
            StartCoroutine("Flow");
        }

        private IEnumerator Flow()
        {
            titleTextInitialColor = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0);
            titleText.color = titleTextInitialColor;

            titleLogoInitialColor = new Color(titleLogo.color.r, titleLogo.color.g, titleLogo.color.b, 0);
            titleLogo.color = titleLogoInitialColor;

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(enterAnimationDelay);

            AnimateEnterTitle();
        }

        private void AnimateEnterTitle()
        {
            titleTransform.DOAnchorPosY(340.0f, animationDuration).SetEase(Ease.OutCirc).OnComplete(AnimateExitTitle).SetDelay(0.5f);

            titleText.DOColor(new Color(titleTextInitialColor.r, titleTextInitialColor.g, titleTextInitialColor.b, 0.568f), animationDuration).SetDelay(0.5f);
            titleLogo.DOColor(new Color(titleLogo.color.r, titleLogo.color.g, titleLogo.color.b, 1.0f), animationDuration).SetDelay(0.5f);
        }

        private void AnimateExitTitle()
        {
            titleTransform.DOAnchorPosY(340.0f, animationDuration).OnComplete(CompleteFlow).SetDelay(exitAnimationDelay);
        }

        private void CompleteFlow()
        {
            OnComplete();
        }
    }
}
