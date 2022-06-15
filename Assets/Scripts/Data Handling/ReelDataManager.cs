using System.Collections.Generic;
using UnityEngine;

namespace PlayStudios.DataManagement
{
    public static class ReelDataManager
    {
        private const string REEL_PATH = "/StreamingAssets/reelstrips.json";

        private static JsonDeserializer<ReelsData> reelStripsDeserializer = new JsonDeserializer<ReelsData>();
        private static ReelsData reelStripsData;
        private static List<Reel> reels = new List<Reel>();

        public static int ReelAmount => reels.Count;

        #region PRIVATE_METHODS
        //Loads initial reel data before the game scene loads
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            LoadReelData();
        }

        /// <summary>
        /// Loads the reel data from the json and creates the reels using the symbol enum
        /// </summary>
        private static void LoadReelData()
        {
            reelStripsData = reelStripsDeserializer.DeserializeJson(string.Concat(Application.dataPath, REEL_PATH));

            for (int i = 0; i < reelStripsData.reelStrips.Count; i++)
            {
                reels.Add(new Reel(reelStripsData.reelStrips[i]));
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public static void DecreaseReelSymbolIndex(int reelIndex)
        {
            if (reelIndex >= reels.Count || reelIndex < 0)
            {
                Debug.LogError($"Reel index {reelIndex} is out of range. Reels amount ({reels.Count})");
                return;
            }

            reels[reelIndex].CurrentSymbolIndex--;
        }

        public static int GetCurrentReelIndex(int reelIndex)
        {
            if (reelIndex >= reels.Count || reelIndex < 0)
            {
                Debug.LogError($"Reel index {reelIndex} is out of range. Reels amount ({reels.Count})");
                return -1;
            }

            return reels[reelIndex].CurrentSymbolIndex;
        }

        public static Reel GetReel(int reelIndex)
        {
            if (reelIndex >= reels.Count || reelIndex < 0)
            {
                Debug.LogError($"Reel index {reelIndex} is out of range. Reels amount ({reels.Count})");
                return null;
            }

            return reels[reelIndex];
        }

        public static SymbolName GetSymbolAtCurrentIndex(int reelIndex)
        {
            return GetSymbolAtIndex(reelIndex, reels[reelIndex].CurrentSymbolIndex);
        }

        public static SymbolName GetSymbolAtIndex(int reelIndex, int symbolIndex)
        {
            if (reelIndex >= reels.Count || reelIndex < 0)
            {
                Debug.LogError($"Reel index {reelIndex} is out of range. Reels amount ({reels.Count})");
                return SymbolName.None;
            }

            if (symbolIndex >= reels[reelIndex].ReelNames.Count || symbolIndex < 0)
            {
                Debug.LogError($"Symbol index {symbolIndex} is out of range in reel {reelIndex}. ({reels[reelIndex].ReelNames.Count})");
                return SymbolName.None;
            }

            return reels[reelIndex].ReelNames[symbolIndex];
        }
        #endregion
    }
}