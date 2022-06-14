using System.Collections;
using UnityEngine;
using PlayStudios.DataManagement;
using PlayStudios.ViewManagement;

namespace PlayStudios.GameManagement
{
    [System.Serializable]
    public class SlotsGamePresenter
    {
        private SlotsGameViewController slotsGameView;
        private CoroutineProxy coroutineProxy;
        private JsonDeserializer<SpinsData> spinDataDeserializer = new JsonDeserializer<SpinsData>();
        private SpinsData spinsData;
        private GameSettings gameSettings;

        private SpinDataHolder currentSpin;
        private int currentReelIndex;
        private int indexStopOffset;
        private int stopOffsetCounter;

        private const string SPINS_PATH = "/StreamingAssets/spins.json";
        private const string GAME_SETTINGS_PATH = "GameSettings";

        public SlotsGamePresenter(SlotsGameViewController slotsGameView, CoroutineProxy coroutineProxy)
        {
            this.slotsGameView = slotsGameView;
            this.coroutineProxy = coroutineProxy;

            Init();
        }

        private void Init()
        {
            //Load required assets
            gameSettings = Resources.Load<GameSettings>(GAME_SETTINGS_PATH);
            spinsData = spinDataDeserializer.DeserializeJson(string.Concat(Application.dataPath, SPINS_PATH));

            indexStopOffset = gameSettings.SymbolsPerReel / 2;
            slotsGameView.OnSpinCommand += OnSpinCommandHandler;
            slotsGameView.OnUpdateReelIndex += OnUpdateReelIndexHandler;
        }

        private void OnUpdateReelIndexHandler(int reelIndex)
        {
            DecreaseReelIndex(reelIndex);
        }

        private void OnSpinCommandHandler()
        {
            currentSpin = GetRandomSpin();
            stopOffsetCounter = 0;
            //Starts spinning reels
            slotsGameView.StartSpinningReels(gameSettings.ReelsSpinningSpeed);

            coroutineProxy.StartCoroutine(WaitToStopCurrentReel(gameSettings.TimeToStopReels));
        }

        private void DecreaseReelIndex(int reelIndex)
        {
            ReelDataManager.DecreaseReelSymbolIndex(reelIndex);
        }

        /// <summary>
        /// Compares current index of current reel with the desired index to stop the reel
        /// </summary>
        /// <param name="index">Current index of current reel</param>
        private void CheckToStopCurrentReel(int index)
        {
            if (index == currentSpin.ReelIndex[currentReelIndex])
            {
                //Once it finds the correct index, it stops waiting for it and counts until it reaches the index offset
                ReelDataManager.GetReel(currentReelIndex).OnUpdateIndex -= CheckToStopCurrentReel;
                ReelDataManager.GetReel(currentReelIndex).OnUpdateIndex += CountForStopOffset;
            }
        }

        private void CountForStopOffset(int index)
        {
            stopOffsetCounter++;
            //Once it reaches the index offset it unsubscribes and stops the current reel
            if (stopOffsetCounter >= indexStopOffset)
            {
                stopOffsetCounter = 0;
                ReelDataManager.GetReel(currentReelIndex).OnUpdateIndex -= CountForStopOffset;
                StopSpinngCurrentReel();
            }
        }

        private void StopSpinngCurrentReel()
        {
            //Stops current reel
            slotsGameView.StopSpinningReel(currentReelIndex);

            currentReelIndex++;

            if (currentReelIndex < GetReelAmount())
            {
                //If there are still reels to stop, it keeps going
                coroutineProxy.StartCoroutine(WaitToStopCurrentReel(gameSettings.TimeBetweenStops));
            }
            else
            {
                //If there are no more reels, it stops and checks if the user won
                currentReelIndex = 0;

                //If the user won it tells the view to show it
                slotsGameView.ShowWin(currentSpin.ActiveReelCount, currentSpin.WinAmount);
            }
        }

        private IEnumerator WaitToStopCurrentReel(float delay)
        {
            yield return new WaitForSeconds(delay);

            ReelDataManager.GetReel(currentReelIndex).OnUpdateIndex += CheckToStopCurrentReel;
        }

        public int GetReelAmount()
        {
            return ReelDataManager.ReelAmount;
        }

        public int GetSymbolsPerReel()
        {
            return gameSettings.SymbolsPerReel;
        }

        public SymbolName GetSymbolAtCurrentIndex(int reelIndex)
        {
            return ReelDataManager.GetSymbolAtCurrentIndex(reelIndex);
        }

        public SymbolName GetSymbolAtIndex(int reelIndex, int symbolIndex)
        {
            return ReelDataManager.GetSymbolAtIndex(reelIndex, symbolIndex);
        }

        public SpinDataHolder GetRandomSpin()
        {
            return spinsData.Spins[Random.Range(0, spinsData.Spins.Length)];
        }
    }
}