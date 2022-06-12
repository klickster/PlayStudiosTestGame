using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotsGameView : MonoBehaviour
{
    [SerializeField] private SymbolDataCollection symbolCollection;
    [SerializeField] private Transform[] reelContainers;

    private Dictionary<SymbolName, Sprite> symbolSprites = new Dictionary<SymbolName, Sprite>();

    private void Awake()
    {
        CacheSymbolSprites(symbolCollection.SymbolsData);

        for (int i = 0; i < reelContainers.Length; i++)
        {
            var reelSymbolImages = reelContainers[i].GetComponentsInChildren<Image>();
            for (int j = 0; j < reelSymbolImages.Length; j++)
            {
                reelSymbolImages[j].sprite = GetSymbolSprite(ReelData.GetSymbolAtIndex(i, j));
            }
        }
    }

    public Sprite GetSymbolSprite(SymbolName symbol)
    {
        if(symbolSprites.ContainsKey(symbol))
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
