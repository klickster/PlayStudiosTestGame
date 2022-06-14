using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayStudios.DataManagement
{
    public class Reel
    {
        private int currentSymbolIndex;

        public List<SymbolName> ReelNames;

        public event Action<int> OnUpdateIndex;

        public int CurrentSymbolIndex   
        {   
            get { return currentSymbolIndex; } 
            set 
            {
                currentSymbolIndex = value;

                //Handles if the index goes out of bounds
                if (currentSymbolIndex >= ReelNames.Count)
                    currentSymbolIndex = 0;

                if (currentSymbolIndex < 0)
                    currentSymbolIndex = ReelNames.Count - 1;

                //Invokes the updated index event
                OnUpdateIndex?.Invoke(currentSymbolIndex);
            } 
        }

        public Reel(List<string> reelData)
        {
            ReelNames = new List<SymbolName>();

            //Parses all strings from the json to the enum for better internal handling
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
}
