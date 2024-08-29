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
                // Load the level index from SaveManager
                _currentLevelIndex = SaveManager.LoadLevelIndex();
                return _currentLevelIndex % _levelList.Count;
            }
            set
            {
                // Set the value and save it via SaveManager
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
                // If the current level index is greater than 0, return the previous index.
                // Otherwise, return the last level's index in a circular manner.
                int previousIndex = (CurrentLevelIndex - 1 + _levelList.Count) % _levelList.Count;
                return previousIndex;
            }
        }

        /// <summary>
        /// Gets the configuration of the previous level based on the previous index.
        /// </summary>
        public Level PreviousLevel => _levelList[PreviousLevelIndex];
    }
}