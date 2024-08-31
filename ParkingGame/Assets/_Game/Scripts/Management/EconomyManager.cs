using UnityEngine;
using _Game.Data;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

namespace _Game.Management
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

        public UnityAction OnCoinAmountChanged;
        public UnityAction OnCoinAmountInsufficient;

        private void Awake()
        {
            RegisterServices();
        }

        ///TEST
        ///
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EarnCoins(1000);
            }
        }
        ///
        ///

        /// <summary>
        /// Registers the EconomyManager as a service using a Service Locator.
        /// </summary>
        private void RegisterServices()
        {
            ServiceLocator.Register(this);
        }

        /// <summary>
        /// Adds coins to the player's balance.
        /// </summary>
        /// <param name="amount">The amount of coins to add.</param>
        public void EarnCoins(int amount)
        {
            _gameData.Coins += amount;
            Debug.Log($"Added {amount} coins. Current balance: {_gameData.Coins}");

            OnCoinAmountChanged?.Invoke();
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

                OnCoinAmountChanged?.Invoke();
                return true;
            }

            OnCoinAmountChanged?.Invoke();
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
            if (!canAfford)
            {
                OnCoinAmountInsufficient?.Invoke();
            }
            Debug.Log(canAfford ? "Player can afford the item." + price : "Player cannot afford the item." + price);
            return canAfford;
        }
    }
}