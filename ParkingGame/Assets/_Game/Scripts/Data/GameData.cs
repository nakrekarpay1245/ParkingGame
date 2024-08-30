using _Game.LevelSystem;
using _Game.Save;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Game Configuration")]
        [Tooltip("List of levels in the game.")]
        [SerializeField]
        private List<Level> _levelList;

        [Header("Economy Configuration")]
        [Tooltip("The player's current amount of coins.")]
        [SerializeField]
        private int _coins;

        // Current level index in the game.
        private int _currentLevelIndex;

        /// <summary>
        /// Property to get or set the current level index. 
        /// When setting, it will save the new value using SaveManager.
        /// When getting, it will load the value from SaveManager.
        /// </summary>
        public int CurrentLevelIndex
        {
            get
            {
                _currentLevelIndex = SaveManager.LoadLevelIndex();
                return _currentLevelIndex % _levelList.Count;
            }
            set
            {
                _currentLevelIndex = value;
                SaveManager.SaveLevelIndex(_currentLevelIndex);
            }
        }

        /// <summary>
        /// Gets the configuration of the current level based on the index.
        /// </summary>
        public Level CurrentLevel => _levelList[CurrentLevelIndex];

        /// <summary>
        /// Gets the index of the previous level.
        /// </summary>
        public int PreviousLevelIndex
        {
            get
            {
                int previousIndex = (CurrentLevelIndex - 1 + _levelList.Count) % _levelList.Count;
                return previousIndex;
            }
        }

        /// <summary>
        /// Gets the configuration of the previous level based on the previous index.
        /// </summary>
        public Level PreviousLevel => _levelList[PreviousLevelIndex];

        /// <summary>
        /// Property to get or set the player's current coin count. 
        /// When setting, it saves the new value via SaveManager.
        /// </summary>
        public int Coins
        {
            get
            {
                _coins = SaveManager.LoadCoins();
                return _coins;
            }
            set
            {
                _coins = value;
                SaveManager.SaveCoins(_coins);
            }
        }
    }
}