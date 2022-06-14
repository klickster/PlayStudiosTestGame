using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace PlayStudios.ViewManagement
{
    public class ReelView : MonoBehaviour
    {
        [SerializeField] private SymbolImage symbolImagePrefab;
        [SerializeField] private VerticalLayoutGroup symbolsLayoutGroup;

        private List<SymbolImage> symbolImages = new List<SymbolImage>();
        private float layoutHeight;
        private float symbolStartingPosition;
        private float symbolResetPosition;

        private float spinningSpeed;
        private bool spinning;
        private bool calculatedPositions;
        private SymbolImage middleSymbol;

        public event Action<SymbolImage, ReelView> OnSymbolReachBottom;

        private void Update()
        {
            if (!spinning) return;

            Spin();
        }

        /// <summary>
        /// Calculates the starting and resetting positions for the symbols
        /// </summary>
        private void CalculateResetAndStartingPositions()
        {
            calculatedPositions = true;
            //Position of the top-most symbol
            symbolStartingPosition = symbolImages[0].GetYPosition();
            //Position of the bottom-most symbol, adding its height and the spacing
            symbolResetPosition = Mathf.Abs(symbolImages[symbolImages.Count - 1].GetYPosition()) + symbolImages[0].GetSprite().rect.height + symbolsLayoutGroup.spacing;
        }

        private void Stop()
        {
            spinning = false;
        }

        private void Spin()
        {
            for (int i = 0; i < symbolImages.Count; i++)
            {
                symbolImages[i].transform.position += Vector3.down * spinningSpeed * Time.deltaTime;

                //If the Y position goes below the reset position, it invokes the symbol reach bottom event
                //This event, eventually, updates the reel symbol index and updates this symbol's sprite
                if (Mathf.Abs(symbolImages[i].GetYPosition()) >= symbolResetPosition)
                {
                    OnSymbolReachBottom?.Invoke(symbolImages[i], this);
                }
            }
        }

        /// <summary>
        /// Sets the Layout container height according to the amount of symbols per reel
        /// </summary>
        private void SetLayoutHeight()
        {
            var layoutGroupRectTransform = symbolsLayoutGroup.GetComponent<RectTransform>();
            layoutHeight = symbolImages.Count * symbolImages[0].GetSprite().rect.height + symbolsLayoutGroup.spacing * (symbolImages.Count - 1);
            layoutGroupRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, layoutHeight);
        }

        public void InitReel(int symbolAmount, SlotsGameViewController viewController)
        {
            //Instantiates and sets starting symbol sprites
            for (int i = 0; i < symbolAmount; i++)
            {
                var symbolImage = Instantiate(symbolImagePrefab, symbolsLayoutGroup.transform);
                symbolImage.SetSymbolSprite(viewController.GetSymbolSprite(i, this));
                symbolImages.Add(symbolImage);
            }

            SetLayoutHeight();
        }

        public void StartSpinning(float movingSpeed)
        {
            //Calculate symbol positions
            if (!calculatedPositions)
            {
                CalculateResetAndStartingPositions();  
            }

            spinning = true;
            this.spinningSpeed = movingSpeed;
        }

        public void StopSpinning()
        {
            Stop();
        }

        public void ResetSymbolImage(SymbolImage symbolImage, Sprite newSprite)
        {
            symbolImage.ResetAnchoredPosition(symbolStartingPosition);
            symbolImage.SetSymbolSprite(newSprite);
        }

        public void DoWinAnimation()
        {
            var closestSymbolImageToZero = symbolImages[0];

            //Gets the symbol closest to the center of the screen, which would end up being the one in the middle
            for (int i = 1; i < symbolImages.Count; i++)
            {
                if(Mathf.Abs(symbolImages[i].transform.localPosition.y) < Mathf.Abs(closestSymbolImageToZero.transform.localPosition.y))
                {
                    closestSymbolImageToZero = symbolImages[i];
                }
            }
            middleSymbol = closestSymbolImageToZero;
            //Animates the middle symbol
            middleSymbol.StartWinAnimation();
        }

        public void StopWinAnimation()
        {
            if (middleSymbol != null)
            {
                middleSymbol.StopWinAnimation();
                middleSymbol = null;
            }
        }
    }
}
