using _Game.Car;
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

        [Header("Economy Configuration")]
        [Tooltip("The player's current amount of coins.")]
        [SerializeField]
        private int _coins;

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

        [Header("Car System")]
        [Tooltip("List of all available cars in the game.")]
        [SerializeField]
        private List<Car> _carList;
        public List<Car> CarList { get => _carList; set => _carList = value; }

        [Tooltip("Index of the currently selected car.")]
        [SerializeField]
        private int _selectedCarIndex;
        public int SelectedCarIndex { get => _selectedCarIndex; set => _selectedCarIndex = value; }

        /// <summary>
        /// Gets the prefab of the currently selected car.
        /// </summary>
        [Tooltip("Prefab of the selected car.")]
        public CarController SelectedCarPrefab => _carList[_selectedCarIndex].CarPrefab;

        /// <summary>
        /// Gets the model prefab of the currently selected car.
        /// </summary>
        [Tooltip("Model prefab of the selected car.")]
        public GameObject SelectedCarModelPrefab => _carList[_selectedCarIndex].CarModelPrefab;

        /// <summary>
        /// Attempts to buy a car if the player has enough coins.
        /// </summary>
        /// <param name="carIndex">The index of the car to buy.</param>
        /// <returns>True if the purchase was successful, false if not enough coins or car already purchased.</returns>
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
        /// Sets the selected car by index, provided the car is purchased.
        /// </summary>
        /// <param name="carIndex">The index of the car to select.</param>
        public void SelectCar(int carIndex)
        {
            if (carIndex >= 0 && carIndex < _carList.Count && _carList[carIndex].IsPurchased)
            {
                _selectedCarIndex = carIndex;
                SaveManager.SaveSelectedCarIndex(_selectedCarIndex);
            }
        }
    }

    /// <summary>
    /// Represents a car in the game, including its purchase status, price, and prefabs.
    /// </summary>
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
        private bool _isPurchased;

        [Header("Car Prefabs")]
        [Tooltip("The main prefab representing the car.")]
        [SerializeField]
        private CarController _carPrefab;

        [Tooltip("The model prefab used for displaying the car in menus or previews.")]
        [SerializeField]
        private GameObject _carModelPrefab;

        public string Name => _name;
        public int Price => _price;
        public bool IsPurchased
        {
            get => _isPurchased;
            set => _isPurchased = value;
        }

        public CarController CarPrefab => _carPrefab;
        public CarConfigSO CarConfig => CarPrefab.CarConfig;
        public GameObject CarModelPrefab => _carModelPrefab;
    }
}