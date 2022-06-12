using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ReelData
{
    private const string REEL_PATH = "/StreamingAssets/reelstrips.json";

    private static JsonDeserializer<ReelsData> reelStripsDeserializer = new JsonDeserializer<ReelsData>();
    private static ReelsData reelStrips;
    private static List<Reel> reels = new List<Reel>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        LoadReelData();
    }

    private static void LoadReelData()
    {
        reelStrips = reelStripsDeserializer.DeserializeJson(Application.dataPath + REEL_PATH);

        for (int i = 0; i < reelStrips.reelStrips.Count; i++)
        {
            reels.Add(new Reel(reelStrips.reelStrips[i]));
        }
    }

    public static SymbolName GetSymbolAtIndex(int reelIndex, int symbolIndex)
    {
        if(reelIndex >= reels.Count)
        {
            Debug.LogError($"Reel index {reelIndex} is higher than the amount of reels ({reels.Count})");
            return SymbolName.None;
        }

        if (symbolIndex >= reels[reelIndex].ReelNames.Count)
        {
            Debug.LogError($"Symbol index {symbolIndex} is higher than the the symbols in reel {reelIndex}. ({reels[reelIndex].ReelNames.Count})");
            return SymbolName.None;
        }

        return reels[reelIndex].ReelNames[symbolIndex];
    }
}

public class ReelsData
{
    public List<List<string>> reelStrips;
}

public class Reel
{
    public List<SymbolName> ReelNames;

    public Reel(List<string> reelData)
    {
        ReelNames = new List<SymbolName>();
        for (int i = 0; i < reelData.Count; i++)
        {
            SymbolName result;
            if (Enum.TryParse(reelData[i], true, out result))
            {
                ReelNames.Add(result);
            }
            else
            {
                Debug.LogError($"Couldnt parse {reelData[i]} as a symbol");
            }
        }
    }
}
