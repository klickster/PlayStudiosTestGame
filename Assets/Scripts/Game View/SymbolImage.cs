using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayStudios.ViewManagement
{
    [RequireComponent(typeof(Image))]
    public class SymbolImage : MonoBehaviour
    {
        [SerializeField] private float winAnimationDuration;
        [SerializeField] private AnimationCurve winAnimationCurve;
        [SerializeField] private Color startAnimationColor;
        [SerializeField] private Color endAnimationColor;

        private Image reelSymbolImage;
        private RectTransform rectTransform;
        private bool doingWinAnimation;
        private float currentTime;

        public Sprite GetSprite() => reelSymbolImage.sprite;

        private void Awake()
        {
            reelSymbolImage = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!doingWinAnimation) return;

            currentTime += Time.deltaTime;

            var t = currentTime / winAnimationDuration;
            reelSymbolImage.color = Color.Lerp(startAnimationColor, endAnimationColor, winAnimationCurve.Evaluate(Mathf.PingPong(t, 1)));
        }

        public void SetSymbolSprite(Sprite symbolSprite)
        {
            reelSymbolImage.sprite = symbolSprite;
        }

        public float GetYPosition()
        {
            return rectTransform.anchoredPosition.y;
        }

        /// <summary>
        /// Sets the position to the parameter and sets as first sibling so it doesn't break the layout
        /// </summary>
        /// <param name="yPosition">New Y position</param>
        public void ResetAnchoredPosition(float yPosition)
        {
            transform.SetAsFirstSibling();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPosition);
        }

        public void StartWinAnimation()
        {
            currentTime = 0;
            doingWinAnimation = true;
        }

        public void StopWinAnimation()
        {
            doingWinAnimation = false;
            reelSymbolImage.color = startAnimationColor;
        }
    }
}
