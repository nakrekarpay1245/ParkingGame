using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Save
{
    /// <summary>
    /// SaveData holds the data that will be saved and loaded by the SaveSystem.
    /// It includes game progress such as the current level index, player's coin balance, car purchases, and selected car index.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        [Header("Game Progress")]
        [Tooltip("The index of the current level the player is on.")]
        [SerializeField]
        public int CurrentLevelIndex = 0;

        [Header("Player Economy")]
        [Tooltip("The amount of coins the player currently has.")]
        [SerializeField]
        public int Coins = 0;

        [Header("Car System")]
        [Tooltip("Dictionary to track whether each car has been purchased.")]
        [SerializeField]
        public Dictionary<int, bool> CarPurchases = new Dictionary<int, bool>();

        [Tooltip("Index of the currently selected car.")]
        [SerializeField]
        public int SelectedCarIndex = 0;

        // Future data fields can be added here
    }
}