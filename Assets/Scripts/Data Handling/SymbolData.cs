using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SymbolData
{
    [SerializeField] private SymbolName symbolName;
    [SerializeField] private Sprite symbolSprite;

    public SymbolName SymbolName => symbolName;
    public Sprite SymbolSprite => symbolSprite;
}
