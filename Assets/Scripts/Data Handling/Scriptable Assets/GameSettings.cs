using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "PlayStudios/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("View Settings")]
    [SerializeField] private int symbolsPerReel = 5;
    [SerializeField] private float reelsSpinningSpeed = 64f;
    [Header("Game Settings")]
    [SerializeField] private float timeToStopReels = 1f;
    [SerializeField] private float timeBetweenStops = 0.3f;

    public int SymbolsPerReel       => symbolsPerReel;
    public float ReelsSpinningSpeed => reelsSpinningSpeed;
    public float TimeToStopReels    => timeToStopReels;
    public float TimeBetweenStops   => timeBetweenStops;
}
