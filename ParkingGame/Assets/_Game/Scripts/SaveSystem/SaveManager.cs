namespace _Game.Save
{
    /// <summary>
    /// SaveManager handles the saving and loading of the game's state,
    /// including the current level index and the player's coins. 
    /// It uses the SaveSystem for the actual persistence logic and follows SOLID principles.
    /// </summary>
    public static class SaveManager
    {
        private static SaveData _currentSaveData;

        /// <summary>
        /// Static constructor to load the SaveData when the SaveManager is first accessed.
        /// This ensures that game data is loaded into memory at the start.
        /// </summary>
        static SaveManager()
        {
            _currentSaveData = SaveSystem.LoadGame();
        }

        /// <summary>
        /// Saves the level index to the SaveData and persists it using SaveSystem.
        /// </summary>
        /// <param name="levelIndex">The new level index to save.</param>
        public static void SaveLevelIndex(int levelIndex)
        {
            _currentSaveData.CurrentLevelIndex = levelIndex;
            SaveSystem.SaveGame(_currentSaveData);
        }

        /// <summary>
        /// Loads the saved level index from SaveData.
        /// </summary>
        /// <returns>The saved current level index.</returns>
        public static int LoadLevelIndex()
        {
            return _currentSaveData.CurrentLevelIndex;
        }

        /// <summary>
        /// Saves the player's coin balance to the SaveData and persists it using SaveSystem.
        /// </summary>
        /// <param name="coins">The current amount of coins to save.</param>
        public static void SaveCoins(int coins)
        {
            _currentSaveData.Coins = coins;
            SaveSystem.SaveGame(_currentSaveData);
        }

        /// <summary>
        /// Loads the saved coin balance from SaveData.
        /// </summary>
        /// <returns>The saved amount of coins the player has.</returns>
        public static int LoadCoins()
        {
            return _currentSaveData.Coins;
        }
    }
}