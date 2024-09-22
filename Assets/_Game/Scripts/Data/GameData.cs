using _Game.Car;
using _Game.LevelSystem;
using _Game.Save;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Data
{
    /// <summary>
    /// Holds game data including levels, economy, and car system.
    /// Ensures data is loaded from and saved to persistent storage using SaveManager.
    /// </summary>
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Game Configuration")]
        [Tooltip("List of levels in the game.")]
        [SerializeField]
        private List<Level> _levelList;
        private int _currentLevelIndex;

        /// <summary>
        /// Gets or sets the current level index, loading/saving it from/to persistent storage.
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
        /// Gets the current level based on the level index.
        /// </summary>
        public Level CurrentLevel => _levelList[CurrentLevelIndex];

        [Header("Economy Configuration")]
        [Tooltip("The player's current amount of coins.")]
        [SerializeField]
        private int _coins;

        /// <summary>
        /// Gets or sets the player's coin count, loading/saving it from/to persistent storage.
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

        [Header("Car System")]
        [Tooltip("List of all available cars in the game.")]
        [SerializeField]
        private List<Car> _carList;

        public List<Car> CarList { get => _carList; set => _carList = value; }

        [Tooltip("Index of the currently selected car.")]
        [SerializeField]
        private int _selectedCarIndex;

        /// <summary>
        /// Gets or sets the index of the currently selected car, loading/saving it from/to persistent storage.
        /// </summary>
        public int SelectedCarIndex
        {
            get
            {
                _selectedCarIndex = SaveManager.LoadSelectedCarIndex();
                return _selectedCarIndex;
            }
            set
            {
                _selectedCarIndex = value;
                SaveManager.SaveSelectedCarIndex(_selectedCarIndex);
            }
        }

        /// <summary>
        /// Retrieves the key for the selected car's prefab to load it dynamically.
        /// </summary>
        public string SelectedCarPrefabKey => _carList[SelectedCarIndex].CarPrefabKey;

        /// <summary>
        /// Retrieves the key for the selected car's model prefab.
        /// </summary>
        public string SelectedCarModelPrefabKey => _carList[SelectedCarIndex].CarModelPrefabKey;

        /// <summary>
        /// Retrieves the selected car's configuration object.
        /// </summary>
        public CarConfigSO SelectedCarConfig
        {
            get
            {
                if (_selectedCarIndex >= 0 && _selectedCarIndex < _carList.Count)
                {
                    return _carList[_selectedCarIndex].CarConfig;
                }
                Debug.LogWarning("SelectedCarIndex is out of range. Returning null.");
                return null; // Or handle appropriately
            }
        }

        /// <summary>
        /// Purchases a car if it has not been purchased already.
        /// </summary>
        /// <param name="carIndex">The index of the car to purchase.</param>
        /// <returns>True if the car was purchased successfully, false otherwise.</returns>
        public bool BuyCar(int carIndex)
        {
            if (carIndex < 0 || carIndex >= _carList.Count)
                return false;

            Car carToBuy = _carList[carIndex];

            if (!carToBuy.IsPurchased)
            {
                carToBuy.IsPurchased = true;
                SaveManager.SaveCarPurchase(carIndex, carToBuy.IsPurchased);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Selects a car if it has been purchased.
        /// </summary>
        /// <param name="carIndex">The index of the car to select.</param>
        public void SelectCar(int carIndex)
        {
            if (carIndex >= 0 && carIndex < _carList.Count && _carList[carIndex].IsPurchased)
            {
                SelectedCarIndex = carIndex;
            }
        }
    }

    [System.Serializable]
    public class Car
    {
        [Header("Car Information")]
        [Tooltip("Name of the car.")]
        [SerializeField]
        private string _name;

        [Tooltip("The price of the car in coins.")]
        [SerializeField]
        private int _price;

        [Tooltip("Indicates whether the car has been purchased.")]
        [SerializeField]
        private bool _isPurchased = false;

        [Header("Car Prefabs (Using String Keys)")]
        [Tooltip("The key used to load the car's prefab dynamically.")]
        [SerializeField]
        private string _carPrefabKey;

        [Tooltip("The key used to load the car's model prefab dynamically.")]
        [SerializeField]
        private string _carModelPrefabKey;

        /// <summary>
        /// Gets the car's name.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Gets the car's price in coins.
        /// </summary>
        public int Price => _price;

        /// <summary>
        /// Indicates if the car has been purchased.
        /// </summary>
        public bool IsPurchased
        {
            get => _isPurchased;
            set => _isPurchased = value;
        }

        /// <summary>
        /// The string key for loading the car's prefab dynamically.
        /// </summary>
        public string CarPrefabKey => _carPrefabKey;

        /// <summary>
        /// The string key for loading the car's model prefab dynamically.
        /// </summary>
        public string CarModelPrefabKey => _carModelPrefabKey;

        /// <summary>
        /// Retrieves the car's configuration object.
        /// </summary>
        public CarConfigSO CarConfig
        {
            get
            {
                // Load the car prefab using the key
                CarController carPrefab = Resources.Load<CarController>(_carPrefabKey);

                // Check if the prefab was loaded successfully
                if (carPrefab == null)
                {
                    Debug.LogError($"Car prefab not found for key: {_carPrefabKey}");
                    return null; // Or handle appropriately, e.g., return a default configuration
                }

                // Return the car's configuration if available
                return carPrefab.CarConfig;
            }
        }
    }
}