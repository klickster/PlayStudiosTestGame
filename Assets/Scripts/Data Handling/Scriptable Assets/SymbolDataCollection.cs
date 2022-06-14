using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Symbols Data", menuName = "PlayStudios/Symbols Data")]
public class SymbolDataCollection : ScriptableObject
{
    [SerializeField] private List<SymbolData> symbolsData;

    public List<SymbolData> SymbolsData => symbolsData;
}
