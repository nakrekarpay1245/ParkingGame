using UnityEngine;
using _Game.Data;

namespace _Game.Economy
{
    /// <summary>
    /// EconomyManager handles coin-related operations such as adding, spending, and checking coin balance.
    /// This class follows SOLID and OOP principles, ensuring high performance and readability.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        [Header("Game Data Reference")]
        [Tooltip("Reference to the GameData ScriptableObject which contains game state information.")]
        [SerializeField]
        private GameData _gameData;

        /// <summary>
        /// Adds coins to the player's balance.
        /// </summary>
        /// <param name="amount">The amount of coins to add.</param>
        public void AddCoins(int amount)
        {
            _gameData.Coins += amount;
            Debug.Log($"Added {amount} coins. Current balance: {_gameData.Coins}");
        }

        /// <summary>
        /// Tries to spend the specified amount of coins. Returns true if successful.
        /// </summary>
        /// <param name="amount">The amount of coins to subtract.</param>
        /// <returns>True if the transaction is successful; otherwise, false.</returns>
        public bool SpendCoins(int amount)
        {
            if (_gameData.Coins >= amount)
            {
                _gameData.Coins -= amount;
                Debug.Log($"Spent {amount} coins. Current balance: {_gameData.Coins}");
                return true;
            }
            Debug.LogWarning("Not enough coins to complete the transaction.");
            return false;
        }

        /// <summary>
        /// Checks if the player has enough coins to purchase an item.
        /// </summary>
        /// <param name="price">The cost of the item.</param>
        /// <returns>True if the player can afford the item; otherwise, false.</returns>
        public bool CanAfford(int price)
        {
            bool canAfford = _gameData.Coins >= price;
            Debug.Log(canAfford ? "Player can afford the item." : "Player cannot afford the item.");
            return canAfford;
        }
    }
}