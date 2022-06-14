using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using PlayStudios.GameManagement;
using TMPro;

namespace PlayStudios.ViewManagement
{
    public class SlotsGameViewController : MonoBehaviour
    {
        [SerializeField] private SymbolDataCollection symbolCollection;
        [SerializeField] private Transform reelsContainer;
        [SerializeField] private ReelView reelViewPrefab;
        [SerializeField] private Button spinButton;
        [SerializeField] private TMP_Text winText; 

        private SlotsGamePresenter slotsGamePresenter;
        private List<ReelView> reels = new List<ReelView>();
        private Dictionary<SymbolName, Sprite> symbolSprites = new Dictionary<SymbolName, Sprite>();
        private const string WIN_TEXT = "Win Amount: ";

        public event Action OnSpinCommand;
        public event Action<int> OnUpdateReelIndex;

        private void Awake()
        {
            CacheSymbolSprites(symbolCollection.SymbolsData);
            spinButton.onClick.AddListener(OnSpinButton);

            slotsGamePresenter = new SlotsGamePresenter(this, new CoroutineProxy(this));
            SetInitialReels(slotsGamePresenter.GetReelAmount(), slotsGamePresenter.GetSymbolsPerReel());
            ResetView();
        }

        public void SetInitialReels(int reelAmount, int symbolsPerReel)
        {
            for (int i = 0; i < reelAmount; i++)
            {
                var reelView = Instantiate(reelViewPrefab, reelsContainer);
                reels.Add(reelView);
                reelView.OnSymbolReachBottom += OnSymbolReachBottomHandler;
                reelView.InitReel(symbolsPerReel, this);
            }
        }

        /// <summary>
        /// Returns the sprite based on the symbol belonging to the index of a reel
        /// </summary>
        /// <param name="index">Symbol index</param>
        /// <param name="reel">Reel that needs to set its symbol</param>
        /// <returns></returns>
        public Sprite GetSymbolSprite(int index, ReelView reel)
        {
            return GetSymbolSprite(slotsGamePresenter.GetSymbolAtIndex(reels.IndexOf(reel), index));
        }

        public void StartSpinningReels(float spinningSpeed)
        {
            for (int i = 0; i < reels.Count; i++)
            {
                reels[i].StartSpinning(spinningSpeed);
            }
        }

        public void StopSpinningReel(int reelIndex)
        {
            reels[reelIndex].StopSpinning();
        }

        /// <summary>
        /// Displays win amount and animates the reels that won
        /// </summary>
        /// <param name="reelAmount">Amount of reels that won</param>
        /// <param name="winAmount">Won amount</param>
        public void ShowWin(int reelAmount, int winAmount)
        {
            spinButton.interactable = true;
            winText.text = string.Concat(WIN_TEXT, winAmount.ToString());

            for (int i = 0; i < reelAmount; i++)
            {
                reels[i].DoWinAnimation();
            }
        }

        /// <summary>
        /// Enables the interaction of the spin button and resets the win amount text
        /// </summary>
        public void ResetView()
        {
            winText.text = WIN_TEXT;
            for (int i = 0; i < reels.Count; i++)
            {
                reels[i].StopWinAnimation();
            }
        }

        /// <summary>
        /// Disables the interaction of the spin button and calls for the spin command 
        /// </summary>
        private void OnSpinButton()
        {
            ResetView();
            OnSpinCommand?.Invoke();
            spinButton.interactable = false;
        }

        /// <summary>
        /// Whenever a symbol reaches its bottom, it lets the presenter know it needs to update this reel's index
        /// </summary>
        /// <param name="symbolImage">Symbol that reached the bottom</param>
        /// <param name="reel">Reel which index needs to be updated</param>
        private void OnSymbolReachBottomHandler(SymbolImage symbolImage, ReelView reel)
        {
            var reelIndex = reels.IndexOf(reel);
            OnUpdateReelIndex?.Invoke(reelIndex);

            //Resets symbol image and updates the sprite
            reel.ResetSymbolImage(symbolImage, GetSymbolSprite(slotsGamePresenter.GetSymbolAtCurrentIndex(reelIndex)));
        }

        private Sprite GetSymbolSprite(SymbolName symbol)
        {
            if (symbolSprites.ContainsKey(symbol))
            {
                return symbolSprites[symbol];
            }
            else
            {
                Debug.LogError($"There is no sprite for symbol {symbol}");
                return null;
            }
        }

        private void CacheSymbolSprites(List<SymbolData> symbolsData)
        {
            for (int i = 0; i < symbolsData.Count; i++)
            {
                symbolSprites.Add(symbolsData[i].SymbolName, symbolsData[i].SymbolSprite);
            }
        }
    }
}
